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
                    { new Blob("Airport", 250, new List<string>() { "Transport", "TEST" }) },
                    { new Blob("TEST", 250, new List<string>() { "TEST1" }) },
                    { new Blob("TEST1", 250, new List<string>() { "TEST2", "ABC" }) },
                    { new Blob("TEST2", 250, new List<string>() { "TEST3" }) },
                    { new Blob("TEST3", 250, new List<string>() { "TEST4" }) },
                    { new Blob("TEST4", 250, new List<string>() { "TEST5" }) },
                    { new Blob("TEST5", 250, new List<string>() { "TEST6" }) },
                    { new Blob("TEST6", 250, new List<string>() { "TEST7" }) },
                    { new Blob("TEST7", 250, new List<string>() { "TEST8" }) },
                    { new Blob("TEST8", 250, new List<string>() { "TEST9" }) },
                    { new Blob("TEST9", 250, new List<string>() { "TEST10" }) },
                    { new Blob("TEST10", 250, new List<string>() { "TEST11" }) },
                    { new Blob("TEST11", 250, new List<string>() { "TEST12" }) },
                    { new Blob("TEST12", 250, new List<string>() { }) },
                    { new Blob("Transport", 250, new List<string>() { }) },
                    { new Blob("ABC", 250, new List<string>() { "ABC1" }) },
                    { new Blob("ABC1", 250, new List<string>() { "ABC2" }) },
                    { new Blob("ABC2", 250, new List<string>() { "ABC3" }) },
                    { new Blob("ABC3", 250, new List<string>() { "ABC4" }) },
                    { new Blob("ABC4", 250, new List<string>() { "ABC5" }) },
                    { new Blob("ABC5", 250, new List<string>() { "ABC6" }) },
                    { new Blob("ABC6", 250, new List<string>() { "ABC7" }) },
                    { new Blob("ABC7", 250, new List<string>() { "ABC8" }) },
                    { new Blob("ABC8", 250, new List<string>() { "ABC9" }) },
                    { new Blob("ABC9", 250, new List<string>() { "ABC10" }) },
                    { new Blob("ABC10", 250, new List<string>() { "ABC11" }) },
                    { new Blob("ABC11", 250, new List<string>() { "ABC12" }) },
                    { new Blob("ABC12", 250, new List<string>() { "ABC13" }) },
                    { new Blob("ABC13", 250, new List<string>() { "ABC14" }) },
                    { new Blob("ABC14", 250, new List<string>() { "ABC15" }) },
                    { new Blob("ABC15", 250, new List<string>() { "ABC16" }) },
                    { new Blob("ABC16", 250, new List<string>() { "ABC17" }) },
                    { new Blob("ABC17", 250, new List<string>() { "ABC18" }) },
                    { new Blob("ABC18", 250, new List<string>() { "ABC19" }) },
                    { new Blob("ABC19", 250, new List<string>() { "ABC20" }) },
                    { new Blob("ABC20", 250, new List<string>() { "ABC21" }) },
                    { new Blob("ABC21", 250, new List<string>() { "ABC22" }) },
                    { new Blob("ABC22", 250, new List<string>() { "ABC23" }) },
                    { new Blob("ABC23", 250, new List<string>()) },
                };

                var painter = new Painter(1000, 15, 10);
                painter.PrepareToDraw(paths);
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