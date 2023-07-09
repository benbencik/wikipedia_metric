using System.Collections.Generic;
using System.Text;

namespace WikipediaMetric
{

	static class JsonManager
	{
		private struct Char
		{
			public const char LeftBracket = '[';
			public const char RightBracket = ']';
			public const char CurlyLeftBracket = '{';
			public const char CurlyRightBracket = '}';
			public const char Comma = ',';
			public const char Colon = ':';
			public const char DoubleQuote = '\"';
			public const char NewLine = '\n';
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

		private static readonly Logger _logger;

		static JsonManager()
		{
			_logger = new Logger(nameof(JsonManager));
		}
		// Saves TMap of page titles and corresponding links to a json file
		public static void ToFile(TMap titleLinksMap, string filePath)
		{
			_logger.Info("Saving TMap to the file: " + filePath);

			using var file = FileManager.GetStreamWriter(filePath);
			var titleLinksMapEnumerator = titleLinksMap.GetEnumerator();

			// If Tmap is empty write nothing to the file
			if (!titleLinksMapEnumerator.MoveNext())
			{
				_logger.Warning("TMap is empty.");
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

		// Loads Tmap of page titles and corresponding links from a json file
		public static TMap FromFile(string filePath)
		{
			_logger.Info("Loading TMap from the file: " + filePath);

			using var file = FileManager.GetStreamReader(filePath);
			var map = new TMap();
			var links = new List<string>();
			var title = new StringBuilder();
			var link = new StringBuilder();
			var state = State.Discard;
			var end = false;

			while (file.Peek() >= 0)
			{
				if (end)
					break;
				var ch = (char)file.Read();
				switch (ch)
				{
					case Char.LeftBracket:
						if (state == State.Discard)
							state = State.Map;
						else if (state == State.LinksTagRead)
						{
							state = State.LinksArray;
						}
						//map.Clear();
						break;
					case Char.CurlyLeftBracket:
						state = State.PairTitle;
						break;
					case Char.CurlyRightBracket:
						if (state == State.Map)
						{
							state = State.Map;
							map.Add(title.ToString(), links);

							title.Clear();
							links = new List<string>();
						}
						break;
					case Char.DoubleQuote:
						if (state == State.PairTitle)
							state = State.TitleTag;
						else if (state == State.TitleTag)
							state = State.TitleTagRead;
						else if (state == State.TitleTagRead)
							state = State.TitleValue;
						else if (state == State.TitleValue)
							state = State.TitleValueRead;

						else if (state == State.PairLinks)
							state = State.LinksTag;
						else if (state == State.LinksTag)
							state = State.LinksTagRead;
						else if (state == State.LinksArray)
							state = State.LinkValue;
						else if (state == State.LinkValue)
						{
							state = State.LinkValueRead;
							links.Add(link.ToString());
							link.Clear();
						}
						break;
					case Char.Colon:
						break;
					case Char.Comma:
						if (state == State.TitleValueRead)
							state = State.PairLinks;
						else if (state == State.LinkValueRead)
						{
							state = State.LinksArray;
						}
						else if (state == State.TitleValue)
							title.Append(ch);
						else if (state == State.LinkValue)
							link.Append(ch);
						break;
					case Char.RightBracket:
						if (state == State.Map)
							state = State.Discard;
						else if (state == State.LinkValueRead)
							state = State.Map;
						// return map
						break;
					default:
						if (state == State.TitleValue)
							title.Append(ch);
						else if (state == State.LinkValue)
							link.Append(ch);
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

			if (trailingComma)
				formatted.AppendLine("	},");
			else
				formatted.AppendLine("	}");

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
			for (int i = 0; i < links.Count - 1; i++)
			{
				formatted.Append("\"" + links[i] + "\"");
				formatted.Append(',');
			}
			formatted.Append(lastLink);

			return formatted.ToString();
		}
	}
}
