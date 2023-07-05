using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            // TODO: Move to class level private variables
            var pageTag = "page>";
            var pageBuffer = new StringBuilder();
            var readingPage = false;

            Regex titleRegex = TitleRegex();
            Regex textRegex = TextRegex();
            Regex linksRegex = LinksRegex();

            // Map page title with its links
            var map = new Dictionary<string, IEnumerable<string>>();

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (string line in GetLines(path))
            {
                if (line.Contains(pageTag))
                {
                    // TODO: Refactor this section to functions, eg. put regex initialisations into some __init__
                    // as well as the map variable, than ParseFrom will read lines and in this section
                    // just processPage will be called which will have access to all class variables
                    if (readingPage)
                    {
                        // Process the page
                        var title = titleRegex.Matches(pageBuffer.ToString())[0].Groups[1].Value;
                        var text = textRegex.Matches(pageBuffer.ToString())[0].Groups[1].Value;
                        var links = linksRegex.Matches(text);

                        // Map links from the page to the corresponding title
                        var linksList = from link in links select link.Groups[1].Value;
                        map.Add(title, linksList);

                        // Clear the buffer so we can read to again a new page
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

        [GeneratedRegex(@"<title[^>]*>([^<]*)<\/title>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex TitleRegex();
        [GeneratedRegex(@"<text[^>]*>([^<]*)<\/text>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex TextRegex();
        [GeneratedRegex(@"\[\[(?!File:)([^\[\]|]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex LinksRegex();
    }
}
