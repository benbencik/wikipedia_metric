using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace wikipedia_metric
{
    public class Graph
    {
        public Dictionary<string, List<string>> adjacencyList;
        public Dictionary<string, Cluster> cluster;
        readonly Logger logger = new("Graph");

        public List<string> nodes;
        
        public Graph(Dictionary<string, List<string>> article_dictionary)
        {
            adjacencyList = article_dictionary;
            nodes = new List<string>(adjacencyList.Keys);
            // ClearNonExistenLinks();
        }

        public void AddVertex(string vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = new List<string>();
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

        public List<string> GetNeighbors(string vertex)
        {
            if (adjacencyList.ContainsKey(vertex))
            {
                return adjacencyList[vertex];
            }
            return new List<string>();
        }

        public bool ContainsVertex(string vertex)
        {
            return adjacencyList.ContainsKey(vertex);
        }

        private void ClearNonExistenLinks() {
            logger.Info("Clearing non existent links");
            foreach (var item in adjacencyList)
            {
                List<String> new_list = new List<string>();
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (adjacencyList.ContainsKey(item.Value[i]))
                    {
                        new_list.Add(item.Value[i]);
                    }
                }
                adjacencyList[item.Key] = new_list;
            }
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
