using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace wikipedia_metric
{
    internal static class Program
    {


        private static void Main(string[] args)
        {
            var logger = new Logger(nameof(Main));
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // var map = WikimediaParser.ParseFrom("data/wikisource_dummy.txt");
                // JsonManager.ToFile(map, "data/enwiki-dummy-new.json");
                // var map = WikimediaParser.ParseFrom("data/enwikisource-20230701-pages-articles.xml");
                // JsonManager.ToFile(map, "data/enwikisource-20230701-pages-articles.json");
                // var map = JsonManager.FromFile("data/enwikisource-20230701-pages-articles.json");

                // var map = JsonManager.FromFile("data/enwiki-dummy.json");
                // JsonManager.ToFile(map, "data/enwiki-dummy-test.json");
                // var map = JsonManager.FromFile("data/enwiki-20230401-new.json");
                // JsonManager.ToFile(map, "data/enwiki-20230401-new-test.json");

                var paths = new List<Blob>()
                {
                    { new Blob("NATO", 566, new List<string>() { "Airport" }) },
                    { new Blob("Airport", 250, new List<string>() { "NATO", "Transport", "TEST" }) },
                    { new Blob("TEST", 250, new List<string>() { "Transport", "Airport" }) },
                    { new Blob("Transport", 1, new List<string>() { "Airport", "TEST" }) }
                };

                var painter = new Painter(1000, 15, 10);
                painter.Print(paths);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            stopwatch.Stop();
            logger.Info($"Finished in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}