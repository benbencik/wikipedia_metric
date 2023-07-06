using System.Collections.Generic;
using System.Text;


namespace WikipediaMetric
{

	static class JsonManager
	{
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

			return new TMap();
		}

		private static string FormatPair(KeyValuePair<string, List<string>> pair, bool trailingComma)
		{
			var formatted = new StringBuilder();

			formatted.AppendLine("	{");

			formatted.AppendLine($@"		""title"": ""{pair.Key}"",");
			var formatedLinks = FormatLinks(pair.Value);
			if (formatedLinks.Length > 0)
				formatted.AppendLine($@"		""links"": [""{FormatLinks(pair.Value)}""]");
			else
				formatted.AppendLine($@"		""links"": []");

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
			var lastLink = links[^1];
			for (int i = 0; i < links.Count - 1; i++)
			{
				formatted.Append(links[i]);
				formatted.Append(',');
			}
			formatted.Append(lastLink);

			return formatted.ToString();
		}
	}
}
