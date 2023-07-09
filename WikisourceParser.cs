using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace wikipedia_metric
{
    internal static partial class WikimediaParser
    {
        private static readonly Logger Logger;

        private static readonly string PageTag;

        private static readonly StringBuilder PageBuffer;

        // true if reading contents of (inside of) a page (<page> contents </page>)
        private static bool _readingPage;

        private static readonly Regex TitleRegex;
        private static readonly Regex TextRegex;
        private static readonly Regex LinksRegex;

        // Map page title with its links
        private static readonly TMap TitleLinksMap;

        static WikimediaParser()
        {
            Logger = new Logger(nameof(WikimediaParser));

            PageTag = "page>";
            PageBuffer = new StringBuilder();
            _readingPage = false;

            TitleRegex = titleRegex();
            TextRegex = textRegex();
            LinksRegex = linksRegex();

            TitleLinksMap = new TMap();
        }

        public static TMap ParseFrom(string path)
        {
            Logger.Info("Reading wikisource file: " + path);

            foreach (string currentLine in FileManager.GetLines(path))
            {
                // Detect start and end of a page
                // We are detecting only `page>` since wikisource dumps are
                // valid XML files so there is always only one opening and
                // one corresponding closing tag
                if (currentLine.Contains(PageTag))
                {
                    ProcessPage();
                    _readingPage = !_readingPage;
                    continue;
                }

                if (_readingPage)
                {
                    // Read the page content to a buffer
                    // Page content is between <page> and </page> tags
                    PageBuffer.Append(currentLine);
                }
            }

            return TitleLinksMap;
        }

        private static void ProcessPage()
        {
            // We detected _pageTag and we are _readingPage so it must be
            // the closing tag, hence we process the page in the _pageBuffer
            if (!_readingPage) return;
            // Get page title
            var title = TitleRegex.Matches(PageBuffer.ToString())[0].Groups[1].Value;
            // Get page text so we can extract links
            var text = TextRegex.Matches(PageBuffer.ToString())[0].Groups[1].Value;
            // Extract links from the page text
            var links = LinksRegex.Matches(text);

            // Map links from the page to the corresponding title
            var linksList = new List<string>(links.Count);
            linksList.AddRange(links.Select(link => link.Groups[1].Value));

            TitleLinksMap.Add(title, linksList);

            // Clear the buffer so we can read to again a new page
            PageBuffer.Clear();
        }

        [GeneratedRegex(@"<title[^>]*>([^<]*)<\/title>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex titleRegex();

        [GeneratedRegex(@"<text[^>]*>([^<]*)<\/text>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex textRegex();

        [GeneratedRegex(@"\[\[(?!File:)([^\[\]|]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex linksRegex();
    }
}