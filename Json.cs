using System;
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

            file.WriteLine("[");

            file.Write(FormatPairs(titleLinksMap));

            file.WriteLine("]");
        }

        // Loads Tmap of page titles and corresponding links from a json file
        public static TMap FromFile(string filePath)
        {
            _logger.Info("Loading TMap from the file: " + filePath);

            return new TMap();
        }

        private static string FormatPairs(TMap titleLinksMap)
        {
            var formatted = new StringBuilder();
            foreach (var pair in titleLinksMap)
            {
                formatted.AppendLine("	{");

                formatted.AppendLine($@"		""title"": ""{pair.Key}"",");
                formatted.AppendLine($@"		""links"": [""{FormatLinks(pair.Value)}""]");

                formatted.AppendLine("	},");
            }

            // Remove trailing comma
            formatted = formatted.Remove(formatted.Length - 3, 3);
            formatted.Append(Environment.NewLine);

            return formatted.ToString();
        }

        private static string FormatLinks(IEnumerable<string> links)
        {
            var formatted = new StringBuilder();

            foreach (var link in links)
            {
                formatted.Append(link);
                formatted.Append(',');
            }

            // Remove trailing comma
            formatted = formatted.Remove(formatted.Length - 1, 1);

            return formatted.ToString();
        }
    }
}
