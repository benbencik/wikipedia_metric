global using TitleLinksMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>>;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace wikipedia_metric
{
    internal static partial class WikimediaParser
    {
        private static readonly Logger Logger;

        private static readonly string PageTag;

        private static readonly StringBuilder PageBuffer;

        // true if reading contents of (inside of) a page (<page> contents </page>)
        private static bool _readingPage;

        private static readonly Regex TitleRegex;
        private static readonly Regex TextRegex;
        private static readonly Regex LinksRegex;

        // Map page title with its links
        private static readonly TitleLinksMap TitleLinksMap;

        static WikimediaParser()
        {
            Logger = new Logger(nameof(WikimediaParser));

            PageTag = "page>";
            PageBuffer = new StringBuilder();
            _readingPage = false;

            // Load precompiled regexes
            TitleRegex = titleRegex();
            TextRegex = textRegex();
            LinksRegex = linksRegex();

            TitleLinksMap = new TitleLinksMap();
        }

        // Goes line by line (one line is always short for the Wikisource dumps
        // so they will fit in memory) in the xml file and parses out
        // PageTitle-PageLinks mapping for every page in the dump.
        public static TitleLinksMap ParseFrom(string path)
        {
            Logger.Info("Reading wikisource file: " + path);

            foreach (string currentLine in FileManager.GetLines(path))
            {
                // Detect start and end of a page
                // We are detecting only `page>` since Wikisource dumps are
                // valid XML files so there is always only one opening and
                // one corresponding closing tag
                if (currentLine.Contains(PageTag))
                {
                    ProcessPage();
                    _readingPage = !_readingPage;
                    continue;
                }

                if (_readingPage)
                {
                    // Read the page content to a buffer
                    // Page content is between <page> and </page> tags
                    PageBuffer.Append(currentLine);
                }
            }

            return TitleLinksMap;
        }

        private static void ProcessPage()
        {
            // We detected _pageTag and we are _readingPage so it must be
            // the closing tag, hence we process the page in the _pageBuffer
            if (!_readingPage) return;
            // Get page title
            var title = TitleRegex.Matches(PageBuffer.ToString())[0].Groups[1].Value;

            // Get page text so we can extract links
            string text;
            try
            {
                text = TextRegex.Matches(PageBuffer.ToString())[0].Groups[1].Value;
            }
            catch (ArgumentOutOfRangeException)
            {
                // There very rarely is just <text bytes="0"/> tag without any text in it
                // and without the explicit closing tag
                text = "";
            }

            // Extract links from the page text
            var links = new HashSet<string>(LinksRegex.Matches(text).Select(link => link.Groups[1].Value));

            // Map links from the page to the corresponding title
            if (!TitleLinksMap.TryAdd(title, links))
                // Some titles are redundant in the whole wikipedia dump
                TitleLinksMap[title].UnionWith(links);

            // Clear the buffer so we can read to again a new page
            PageBuffer.Clear();
        }

        // Filters out CONTENT from following structure: <title ANYTHING> CONTENT </title>
        [GeneratedRegex(@"<title[^>]*>([^<]*)<\/title>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex titleRegex();

        // Filters out CONTENT from following structure: <text ANYTHING> CONTENT </text>
        [GeneratedRegex(@"<text[^>]*>([^<]*)<\/text>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex textRegex();

        // Filters out CONTENT from following structure: [[CONTENT]] except if there is [[File:ANYTHING]]
        // Links are stored in [[LINK]] format but there is also this [[File:...]] thing for linking
        // files which we do not want to have in our data
        [GeneratedRegex(@"\[\[(?!File:)([^\[\]|]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex linksRegex();
    }
}