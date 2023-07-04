using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WikipediaMetric
{
    partial class WikimediaParser
    {
        private static IEnumerable GetLines(string path)
        {
            try
            {
                return File.ReadLines(path);
            }
            catch (Exception)
            {
                Console.WriteLine("An error occured during reading the wikimedia file.");
                throw;
            }
        }
        public static void ParseFrom(string path)
        {
            Console.WriteLine("Reading file: " + path);

            var pageTag = "page>";
            var pageBuffer = new StringBuilder();
            var readingPage = false;


            Regex titleRegex = TitleRegex();
            Regex textRegex = TextRegex();

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (string line in GetLines(path))
            {
                if (line.Contains(pageTag))
                {
                    if (readingPage)
                    {
                        // Process page
                        var title = titleRegex.Matches(pageBuffer.ToString())[0];
                        var text = textRegex.Matches(pageBuffer.ToString())[0];
                        pageBuffer.Clear();

                    }
                    readingPage = !readingPage;
                    continue;
                }

                if (readingPage)
                {
                    pageBuffer.Append(line);
                }
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

        }

        [GeneratedRegex("<title.*>.*</title>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex TitleRegex();
        [GeneratedRegex("<text.*>.*</text>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex TextRegex();
    }
}
