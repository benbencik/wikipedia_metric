﻿namespace WikipediaMetric
{
    class Program
    {
        static void Main(string[] args)
        {
            var _logger = new Logger(nameof(Main));

            var map = WikimediaParser.ParseFrom("data/wikisource_dummy.txt");
            // var map = WikimediaParser.ParseFrom("data/enwiki-20230401-pages-articles-multistream1.xml-p1p41242");
        }
    }
}
