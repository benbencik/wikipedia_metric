using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace WikipediaMetric
{
    static partial class WikimediaParser
    {
        private static Logger _logger;

        private static string _pageTag;
        private static StringBuilder _pageBuffer;
        // true if reading contents of (inside of) a page (<page> contents </page>)
        private static bool _readingPage;

        private static Regex _titleRegex;
        private static Regex _textRegex;
        private static Regex _linksRegex;

        // Map page title with its links
        private static TMap _titleLinksMap;

        static WikimediaParser()
        {
            _logger = new Logger(nameof(WikimediaParser));

            _pageTag = "page>";
            _pageBuffer = new();
            _readingPage = false;

            _titleRegex = titleRegex();
            _textRegex = textRegex();
            _linksRegex = linksRegex();

            _titleLinksMap = new();
        }

        public static TMap ParseFrom(string path)
        {
            _logger.Info("Reading file: " + path);

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (string currentLine in FileManager.GetLines(path))
            {
                // Detect start and end of a page
                // We are detecting only `page>` since wikisource dumps are
                // valid XML files so there is always only one opening and
                // one corresponding closing tag
                if (currentLine.Contains(_pageTag))
                {
                    ProcessPage();
                    _readingPage = !_readingPage;
                    continue;
                }

                if (_readingPage)
                {
                    // Read the page content to a buffer
                    // Page content is between <page> and </page> tags
                    _pageBuffer.Append(currentLine);
                }
            }
            stopwatch.Stop();
            _logger.Info($"Finished in {stopwatch.ElapsedMilliseconds}ms.");

            return _titleLinksMap;
        }

        private static void ProcessPage()
        {
            // We detected _pageTag and we are _readingPage so it must be
            // the closing tag, hence we process the page in the _pageBuffer
            if (_readingPage)
            {
                // Get page title
                var title = _titleRegex.Matches(_pageBuffer.ToString())[0].Groups[1].Value;
                // Get page text so we can extract links
                var text = _textRegex.Matches(_pageBuffer.ToString())[0].Groups[1].Value;
                // Extract links from the page text
                var links = _linksRegex.Matches(text);

                // Map links from the page to the corresponding title
                var linksList = from link in links select link.Groups[1].Value;
                _titleLinksMap.Add(title, linksList);

                // Clear the buffer so we can read to again a new page
                _pageBuffer.Clear();
            }
        }

        [GeneratedRegex(@"<title[^>]*>([^<]*)<\/title>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex titleRegex();
        [GeneratedRegex(@"<text[^>]*>([^<]*)<\/text>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex textRegex();
        [GeneratedRegex(@"\[\[(?!File:)([^\[\]|]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex linksRegex();
    }
}
