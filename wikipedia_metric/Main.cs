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

                var tree = new List<Node>()
                {
                    new Node("NATO", 20, new List<string>() { "Airport" }),
                    new Node("Airport", 30, new List<string>() { "Transport", "TEST" }),
                    new Node("TEST", 10, new List<string>() { "TEST1" }),
                    new Node("TEST1", 30, new List<string>() { "TEST2", "ABC" }),
                    new Node("TEST2", 20, new List<string>() { "TEST3" }),
                    new Node("TEST3", 20, new List<string>() { "TEST4" }),
                    new Node("TEST4", 20, new List<string>() { "TEST5" }),
                    new Node("TEST5", 20, new List<string>() { "TEST6" }),
                    new Node("TEST6", 20, new List<string>() { "TEST7" }),
                    new Node("TEST7", 20, new List<string>() { "TEST8" }),
                    new Node("TEST8", 20, new List<string>() { "TEST9" }),
                    new Node("TEST9", 20, new List<string>() { "TEST10" }),
                    new Node("TEST10", 20, new List<string>() { "TEST11" }),
                    new Node("TEST11", 20, new List<string>() { "TEST12" }),
                    new Node("TEST12", 10, new List<string>() { }),
                    new Node("Transport", 10, new List<string>() { }),
                    new Node("ABC", 20, new List<string>() { "ABC1" }),
                    new Node("ABC1", 20, new List<string>() { "ABC2" }),
                    new Node("ABC2", 20, new List<string>() { "ABC3" }),
                    new Node("ABC3", 20, new List<string>() { "ABC4" }),
                    new Node("ABC4", 20, new List<string>() { "ABC5" }),
                    new Node("ABC5", 20, new List<string>() { "ABC6" }),
                    new Node("ABC6", 20, new List<string>() { "ABC7" }),
                    new Node("ABC7", 20, new List<string>() { "ABC8" }),
                    new Node("ABC8", 20, new List<string>() { "ABC9" }),
                    new Node("ABC9", 20, new List<string>() { "ABC10" }),
                    new Node("ABC10", 50, new List<string>() { "ABC11", "ABC23", "ABC22" }),
                    new Node("ABC11", 20, new List<string>() { "ABC12" }),
                    new Node("ABC12", 20, new List<string>() { "ABC13" }),
                    new Node("ABC13", 20, new List<string>() { "ABC14" }),
                    new Node("ABC14", 20, new List<string>() { "ABC15" }),
                    new Node("ABC15", 20, new List<string>() { "ABC16" }),
                    new Node("ABC16", 20, new List<string>() { "ABC17" }),
                    new Node("ABC17", 20, new List<string>() { "ABC18" }),
                    new Node("ABC18", 20, new List<string>() { "ABC19" }),
                    new Node("ABC19", 20, new List<string>() { "ABC20" }),
                    new Node("ABC20", 20, new List<string>() { "ABC21" }),
                    new Node("ABC21", 10, new List<string>()),
                    new Node("ABC22", 20, new List<string>() { "ABC24" }),
                    new Node("ABC23", 10, new List<string>()),
                    new Node("ABC24", 10, new List<string>()),
                };

                var painter = new Painter(1000, 15, 10);
                painter.PaintToImage(tree, "image.png");
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