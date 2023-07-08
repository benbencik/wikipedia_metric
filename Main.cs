﻿global using TMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>;
using System;
using System.Diagnostics;


namespace WikipediaMetric
{
    class Program
    {
        static void Main(string[] args)
        {
            var _logger = new Logger(nameof(Main));
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                // var map = WikimediaParser.ParseFrom("data/wikisource_dummy.txt");
                // JsonManager.ToFile(map, "data/enwiki-dummy.json");
                // var map = WikimediaParser.ParseFrom("data/enwiki-20230401-pages-articles-multistream1.xml-p1p41242");
                // JsonManager.ToFile(map, "data/enwiki-20230401.json");

                // var map = JsonManager.FromFile("data/enwiki-dummy.json");
                // JsonManager.ToFile(map, "data/enwiki-dummy-test.json");
                var map = JsonManager.FromFile("data/enwiki-20230401.json");
                JsonManager.ToFile(map, "data/enwiki-20230401-test.json");

                // var map = WikimediaParser.ParseFrom("data/albedo_test.txt");
                // JsonManager.ToFile(map, "data/albedo_test.json");
                // var map = JsonManager.FromFile("data/albedo_test.json");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            stopwatch.Stop();
            _logger.Info($"Finished in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}

