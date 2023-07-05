namespace WikipediaMetric
{

    static class Json
    {
        private static Logger _logger;
        static Json()
        {
            _logger = new Logger(nameof(Json));
        }
        // Saves TMap of page titles and corresponding links to a json file
        public static void ToFile(TMap titleLinksMap, string filePath)
        {
            _logger.Info("Saving TMap to the file: " + filePath);
            
        }

        // Loads Tmap of page titles and corresponding links from a json file
        public static TMap FromFile(string filePath)
        {
            _logger.Info("Loading TMap from the file: " + filePath);

            return new TMap();
        }
    }
}
