using System.Collections.Generic;
using System.Text;

namespace wikipedia_metric
{
	internal static class JsonManager
	{
		private struct Char
		{
			public const char LeftBracket = '[';
			public const char RightBracket = ']';
			public const char CurlyLeftBracket = '{';
			public const char CurlyRightBracket = '}';
			public const char Comma = ',';
			public const char DoubleQuote = '\"';
		}

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
			Logger = new Logger(nameof(JsonManager));
		}

		// Saves TMap of page titles and corresponding links to a json file
		public static void ToFile(TMap titleLinksMap, string filePath)
		{
			Logger.Info("Saving TMap to the file: " + filePath);

			using var file = FileManager.GetStreamWriter(filePath);
			using var titleLinksMapEnumerator = titleLinksMap.GetEnumerator();

			// If TMap is empty write nothing to the file
			if (!titleLinksMapEnumerator.MoveNext())
			{
				Logger.Warning("TMap is empty.");
				return;
			}

			// We do not want trailing comma at the end so we choose one pair
			// and append it at the end without the comma, we do not care
			// about the ordering here so we can choose the first one
			// for our last one
			var lastPair = titleLinksMapEnumerator.Current;

			file.WriteLine("[");

			while (titleLinksMapEnumerator.MoveNext())
				file.Write(FormatPair(titleLinksMapEnumerator.Current, true));
			file.Write(FormatPair(lastPair, false));

			file.WriteLine("]");
		}

		// Loads TMap of page titles and corresponding links from a json file
		public static TMap FromFile(string filePath)
		{
			Logger.Info("Loading TMap from the file: " + filePath);

			using var file = FileManager.GetStreamReader(filePath);
			var map = new TMap();
			var links = new List<string>();
			var title = new StringBuilder();
			var link = new StringBuilder();
			var state = State.Discard;

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
								map.Add(title.ToString(), links);

								title.Clear();
								links = new List<string>();
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
								links.Add(link.ToString());
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

			return map;
		}

		private static string FormatPair(KeyValuePair<string, List<string>> pair, bool trailingComma)
		{
			var formatted = new StringBuilder();

			formatted.AppendLine("	{");

			formatted.AppendLine($@"		""title"": ""{pair.Key}"",");
			formatted.AppendLine($@"		""links"": [{FormatLinks(pair.Value)}]");

			formatted.AppendLine(trailingComma ? "	}," : "	}");

			return formatted.ToString();
		}

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