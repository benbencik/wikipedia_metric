namespace WikipediaMetric
{
    class Program
    {
        static void Main(string[] args)
        {
            WikimediaParser.ParseFrom("data/wikisource_dummy.txt");
        }
    }
}
