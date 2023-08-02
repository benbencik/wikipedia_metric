using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.RegularExpressions;

namespace wikipedia_metric
{
    public class Graph
    {
        public Dictionary<string, HashSet<string>> adjacencyList;
        public Dictionary<string, Cluster> cluster;
        readonly Logger logger = new("Graph");

        public List<string> nodes;

        private string[] trimPatterns = {"/", ":ru:", "../", "Author:", ":Category:"};
        
        public Graph(Dictionary<string, HashSet<string>> article_dictionary)
        {
            adjacencyList = article_dictionary;
            nodes = new List<string>(adjacencyList.Keys);
            TrimPatterns();
            // ClearNonExistenLinks();
        }

        public void AddVertex(string vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = new HashSet<string>();
            }
        }
        public void AddEdge(string source, string destination)
        {
            if (!adjacencyList.ContainsKey(source))
            {
                AddVertex(source);
            }
            if (!adjacencyList.ContainsKey(destination))
            {
                AddVertex(destination);
            }

            adjacencyList[source].Add(destination);
            adjacencyList[destination].Add(source);
        }

        public HashSet<string> GetNeighbors(string vertex)
        {
            if (adjacencyList.ContainsKey(vertex))
            {
                return adjacencyList[vertex];
            }
            return new HashSet<string>();
        }

        public bool ContainsVertex(string vertex)
        {
            return adjacencyList.ContainsKey(vertex);
        }

        private void TrimPatterns() {
            foreach (var link in adjacencyList)
            {
                foreach (var item in link.Value)
                {
                    if (item.Contains("Wikipedia:"))
                    {
                        link.Value.Remove(item);
                    }
                }
            }
        }

        private void Trimlinks() {
            foreach (var link in adjacencyList)
            {
                foreach (var item in link.Value)
                {
                    foreach (var pattern in trimPatterns)
                    {
                        if (item.Contains(pattern))
                        {
                            link.Value.Remove(item);
                        }
                    }
                }
            }   
        }

        private void ClearNonExistenLinks() {
            logger.Info("Clearing non existent links");
            int counter = 0;
            foreach (var item in adjacencyList)
            {
                HashSet<String> new_links = new HashSet<string>();
                foreach (var link in item.Value)
                {
                    if (adjacencyList.ContainsKey(link))
                    {
                        new_links.Add(link);
                    }
                    else
                    {
                        counter++;
                    }
                }
                adjacencyList[item.Key] = new_links;
            }
            logger.Info($"Removed {counter} non existent links");
        }


        public void SearchArticles(string searchQuery)
        {
            string[] searchTerms = searchQuery.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> regexPatterns = new();

            // Generate regex patterns for each search term
            foreach (string searchTerm in searchTerms)
            {
                string processedTerm = searchTerm;
                bool startWith = processedTerm.StartsWith("^");
                bool endWith = processedTerm.EndsWith("$");
                bool synonym = processedTerm.StartsWith("~");

                if (startWith)
                    processedTerm = processedTerm.TrimStart('^');

                if (endWith)
                    processedTerm = processedTerm.TrimEnd('$');

                if (synonym)
                    processedTerm = processedTerm.TrimStart('~');

                // Escape special characters in the search term
                string regexPattern = @"(?i)\b";

                if (startWith)
                    regexPattern += "^" + Regex.Escape(processedTerm);
                else
                    regexPattern += Regex.Escape(processedTerm);

                regexPattern += @"\w*";

                if (endWith)
                    regexPattern += Regex.Escape(processedTerm) + "$";

                regexPattern += @"\b";

                regexPatterns.Add(regexPattern);
            }

            string combinedRegexPattern = string.Join(".*", regexPatterns);

            var searchResults = nodes.Where(article => Regex.IsMatch(article, combinedRegexPattern));

            Console.WriteLine($"Search Results for '{searchQuery}':");
            foreach (var result in searchResults)
            {
                Console.WriteLine(result);
            }
        }

    }
    
}
