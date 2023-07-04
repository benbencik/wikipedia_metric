using System;
using System.Collections;
using System.IO;
using System.Text;

namespace WikipediaMetric
{
    class WikimediaParser
    {
        private static IEnumerable GetLines(string path)
        {
            try
            {
                return File.ReadLines(@path);
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

            foreach (string line in GetLines(path))
            {
                if (line.Contains(pageTag))
                {
                    readingPage = !readingPage;
                    continue;
                }

                if (readingPage)
                {
                    pageBuffer.Append(line);

                }
            }
            Console.WriteLine(pageBuffer.ToString());
        }
    }
}
