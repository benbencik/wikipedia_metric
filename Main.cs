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

                var paths = new List<Node>()
                {
                    { new Node("NATO", 566, new List<string>() { "Airport" }) },
                    { new Node("Airport", 250, new List<string>() { "Transport", "TEST" }) },
                    { new Node("TEST", 250, new List<string>() { "TEST1" }) },
                    { new Node("TEST1", 250, new List<string>() { "TEST2", "ABC" }) },
                    { new Node("TEST2", 250, new List<string>() { "TEST3" }) },
                    { new Node("TEST3", 250, new List<string>() { "TEST4" }) },
                    { new Node("TEST4", 250, new List<string>() { "TEST5" }) },
                    { new Node("TEST5", 250, new List<string>() { "TEST6" }) },
                    { new Node("TEST6", 250, new List<string>() { "TEST7" }) },
                    { new Node("TEST7", 250, new List<string>() { "TEST8" }) },
                    { new Node("TEST8", 250, new List<string>() { "TEST9" }) },
                    { new Node("TEST9", 250, new List<string>() { "TEST10" }) },
                    { new Node("TEST10", 250, new List<string>() { "TEST11" }) },
                    { new Node("TEST11", 250, new List<string>() { "TEST12" }) },
                    { new Node("TEST12", 250, new List<string>() { }) },
                    { new Node("Transport", 250, new List<string>() { }) },
                    { new Node("ABC", 250, new List<string>() { "ABC1" }) },
                    { new Node("ABC1", 250, new List<string>() { "ABC2" }) },
                    { new Node("ABC2", 250, new List<string>() { "ABC3" }) },
                    { new Node("ABC3", 250, new List<string>() { "ABC4" }) },
                    { new Node("ABC4", 250, new List<string>() { "ABC5" }) },
                    { new Node("ABC5", 250, new List<string>() { "ABC6" }) },
                    { new Node("ABC6", 250, new List<string>() { "ABC7" }) },
                    { new Node("ABC7", 250, new List<string>() { "ABC8" }) },
                    { new Node("ABC8", 250, new List<string>() { "ABC9" }) },
                    { new Node("ABC9", 250, new List<string>() { "ABC10" }) },
                    { new Node("ABC10", 250, new List<string>() { "ABC11" }) },
                    { new Node("ABC11", 250, new List<string>() { "ABC12" }) },
                    { new Node("ABC12", 250, new List<string>() { "ABC13" }) },
                    { new Node("ABC13", 250, new List<string>() { "ABC14" }) },
                    { new Node("ABC14", 250, new List<string>() { "ABC15" }) },
                    { new Node("ABC15", 250, new List<string>() { "ABC16" }) },
                    { new Node("ABC16", 250, new List<string>() { "ABC17" }) },
                    { new Node("ABC17", 250, new List<string>() { "ABC18" }) },
                    { new Node("ABC18", 250, new List<string>() { "ABC19" }) },
                    { new Node("ABC19", 250, new List<string>() { "ABC20" }) },
                    { new Node("ABC20", 250, new List<string>() { "ABC21" }) },
                    { new Node("ABC21", 250, new List<string>() { "ABC22" }) },
                    { new Node("ABC22", 250, new List<string>() { "ABC23" }) },
                    { new Node("ABC23", 250, new List<string>()) },
                };

                var painter = new Painter(1000, 15, 10);
                painter.PaintToImage(paths, "image.png");
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