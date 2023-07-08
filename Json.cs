using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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

			var linesEnumerator = FileManager.GetLines(filePath).GetEnumerator();

			if (!linesEnumerator.MoveNext())
			{
				_logger.Error("Given json file is empty");
				throw new JsonException();
			}

			var map = new TMap();

			// We discarded the first line since it is the '[' opening char
			// and also each '}' closing chars as well as the final last ']'
			while (linesEnumerator.MoveNext())
			{
				if (linesEnumerator.Current.ToString().Trim() == "{")
				{
					// Read `"title": "TITLE"` line
					linesEnumerator.MoveNext();
					var titleString = linesEnumerator.Current.ToString().Split(": ")[1];
					// Remove the quotes from "TITLE"
					var title = Encoding.UTF8.GetString(Convert.FromBase64String(titleString[1..^2]));

					// Read `"links": ["LINK1","LINK2"]` line
					linesEnumerator.MoveNext();
					var linksArray = linesEnumerator.Current.ToString().Split(": ")[1];
					// Remove the `[` from the start and `],` from the end
					linksArray = linksArray[1..^1];

					var links = new List<string>();
					foreach (var link in linksArray.Split(","))
					{
						// Remove quotes from each `"LINKn"`
						if (link.Length > 1)
							links.Add(Encoding.UTF8.GetString(Convert.FromBase64String(link[1..^1])));
					}
					map.Add(title, links);
				}
			}
			return map;
		}

		private static string FormatPair(KeyValuePair<string, List<string>> pair, bool trailingComma)
		{
			var formatted = new StringBuilder();

			formatted.AppendLine("	{");

			formatted.AppendLine($@"		""title"": ""{Convert.ToBase64String(Encoding.UTF8.GetBytes(pair.Key))}"",");
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
			var lastLink = "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(links[^1])) + "\"";
			for (int i = 0; i < links.Count - 1; i++)
			{
				formatted.Append("\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(links[i])) + "\"");
				formatted.Append(',');
			}
			formatted.Append(lastLink);

			return formatted.ToString();
		}
	}
}
