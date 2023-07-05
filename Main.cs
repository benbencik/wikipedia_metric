namespace WikipediaMetric
{
    class Program
    {
        static void Main(string[] args)
        {
            WikimediaParser.ParseFrom("data/wikisource_dummy.txt");
            // WikimediaParser.ParseFrom("data/enwiki-20230401-pages-articles-multistream1.xml-p1p41242");
        }
    }
}
