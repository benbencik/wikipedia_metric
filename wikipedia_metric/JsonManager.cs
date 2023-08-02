using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wikipedia_metric
{
    internal static class JsonManager
    {
        // Struct to hold constant characters used for JSON parsing
        private struct Char
        {
            public const char LeftBracket = '[';
            public const char RightBracket = ']';
            public const char CurlyLeftBracket = '{';
            public const char CurlyRightBracket = '}';
            public const char Comma = ',';
            public const char DoubleQuote = '\"';
        }

        // Enum to represent different states during JSON parsing
        private enum State
        {
            Discard,
            Map,
            PairTitle,
            PairLinks,
            TitleTag,
            TitleTagRead,
            TitleValue,
            TitleValueRead,
            LinksTag,
            LinksTagRead,
            LinksArray,
            LinkValue,
            LinkValueRead
        }

        private static readonly Logger Logger;

        static JsonManager()
        {
            // Initialize the logger
            Logger = new Logger(nameof(JsonManager));
        }

        // Saves TitleLinksMap of page titles and corresponding links to a json file
        public static void ToFile(TitleLinksMap titleLinksMap, string filePath)
        {
            Logger.Info("Saving TitleLinksMap to the file: " + filePath);

            using var file = FileManager.GetStreamWriter(filePath);
            using var titleLinksMapEnumerator = titleLinksMap.GetEnumerator();

            // If TitleLinksMap is empty write nothing to the file
            if (!titleLinksMapEnumerator.MoveNext())
            {
                Logger.Warning("TitleLinksMap is empty.");
                return;
            }

            // We do not want trailing comma at the end so we choose one pair
            // and append it at the end without the comma, we do not care
            // about the ordering here so we can choose the first one
            // for our last one
            var lastPair = titleLinksMapEnumerator.Current;

            // Start writing the JSON array
            file.WriteLine("[");

            // Write each key-value pair to the file
            while (titleLinksMapEnumerator.MoveNext())
                file.Write(FormatPair(titleLinksMapEnumerator.Current, true));
            // Write the last key-value pair without the trailing comma
            file.Write(FormatPair(lastPair, false));

            // End the JSON array
            file.WriteLine("]");
        }

        // Loads TitleLinksMap of page titles and corresponding links from a json file
        public static TitleLinksMap FromFile(string filePath)
        {
            Logger.Info("Loading TitleLinksMap from the file: " + filePath);

            using var file = FileManager.GetStreamReader(filePath); // Open a stream reader to read the file
            var map = new TitleLinksMap(); // Create a new TitleLinksMap object to store the loaded data
            var links = new HashSet<string>(); // List to temporarily store the links for a title
            var title = new StringBuilder(); // StringBuilder to build the title string
            var link = new StringBuilder(); // StringBuilder to build the link string
            var state = State.Discard; // Initial state is Discard

            // Read characters until the end of the file is reached
            while (file.Peek() >= 0)
            {
                var ch = (char)file.Read();
                switch (ch)
                {
                    case Char.LeftBracket:
                        state = state switch
                        {
                            State.Discard => State.Map,
                            State.LinksTagRead => State.LinksArray,
                            _ => state
                        };
                        break;
                    case Char.CurlyLeftBracket:
                        switch (state)
                        {
                            case State.Map:
                                state = State.PairTitle;
                                break;
                            case State.TitleValue:
                                title.Append(ch);
                                break;
                            case State.LinkValue:
                                link.Append(ch);
                                break;
                        }

                        break;
                    case Char.CurlyRightBracket:
                        switch (state)
                        {
                            case State.Map:
                                state = State.Map;
                                // Add the title and links to the map
                                map.Add(title.ToString(), links);
                                if (!map.TryAdd(title.ToString(), links))
                                    map[title.ToString()].UnionWith(links);
                                // Clear the title StringBuilder for the next title
                                title.Clear();
                                // Create a new list for the next set of links
                                links = new HashSet<string>();
                                break;
                            case State.TitleValue:
                                title.Append(ch);
                                break;
                            case State.LinkValue:
                                link.Append(ch);
                                break;
                        }

                        break;
                    case Char.DoubleQuote:
                        switch (state)
                        {
                            case State.PairTitle:
                                state = State.TitleTag;
                                break;
                            case State.TitleTag:
                                state = State.TitleTagRead;
                                break;
                            case State.TitleTagRead:
                                state = State.TitleValue;
                                break;
                            case State.TitleValue:
                                state = State.TitleValueRead;
                                break;
                            case State.PairLinks:
                                state = State.LinksTag;
                                break;
                            case State.LinksTag:
                                state = State.LinksTagRead;
                                break;
                            case State.LinksArray:
                                state = State.LinkValue;
                                break;
                            case State.LinkValue:
                                state = State.LinkValueRead;
                                // Add the link to the list of links
                                links.Add(link.ToString());
                                // Clear the StringBuilder for the next link
                                link.Clear();
                                break;
                        }

                        break;
                    case Char.Comma:
                        switch (state)
                        {
                            case State.TitleValue:
                                title.Append(ch);
                                break;
                            case State.TitleValueRead:
                                state = State.PairLinks;
                                break;
                            case State.LinkValue:
                                link.Append(ch);
                                break;
                            case State.LinkValueRead:
                                state = State.LinksArray;
                                break;
                        }

                        break;
                    case Char.RightBracket:
                        state = state switch
                        {
                            State.Map => State.Discard,
                            State.LinksArray => State.Map,
                            State.LinkValueRead => State.Map,
                            _ => state
                        };
                        break;
                    default:
                        switch (state)
                        {
                            case State.TitleValue:
                                title.Append(ch);
                                break;
                            case State.LinkValue:
                                link.Append(ch);
                                break;
                        }

                        break;
                }
            }

            // Return the loaded map
            return map;
        }

        // Formats a key-value pair from the map into a JSON object string, including the title and links
        private static string FormatPair(KeyValuePair<string, HashSet<string>> pair, bool trailingComma)
        {
            var formatted = new StringBuilder();

            formatted.AppendLine("	{");

            formatted.AppendLine($@"		""title"": ""{pair.Key}"",");
            formatted.AppendLine($@"		""links"": [{FormatLinks(pair.Value.ToList())}]");

            formatted.AppendLine(trailingComma ? "	}," : "	}");

            return formatted.ToString();
        }

        // Formats a list of links into a comma-separated string of quoted links
        private static string FormatLinks(List<string> links)
        {
            if (links.Count == 0)
                return "";

            var formatted = new StringBuilder();

            // So we can remove the trailing comma at the end,
            // but we want to preserve the ordering of links
            var lastLink = "\"" + links[^1] + "\"";
            for (var i = 0; i < links.Count - 1; i++)
            {
                formatted.Append("\"" + links[i] + "\"");
                formatted.Append(',');
            }

            formatted.Append(lastLink);

            return formatted.ToString();
        }
    }
}