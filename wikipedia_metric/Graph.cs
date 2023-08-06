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
        public int num_of_edges;

        readonly private string[] prefixes = { 
            ":ru:", "../", "Author:", ":Category:", "w:",
            "Page:", "Wikipedia:", "File:", "Template:",
        };

        readonly private string[] suffixes = {
            ".jpg", ".png", ".pdf", ".svg", ".ogg", 
            ".mp3", ".jpeg", ".gif"
        };

        public Graph(Dictionary<string, HashSet<string>> article_dictionary)
        {
            num_of_edges = 0;
            adjacencyList = article_dictionary;
            nodes = new List<string>(adjacencyList.Keys);
            logger.Info($"Formating links");
            TrimInvalidLinks();
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

            num_of_edges++;
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


        private void TrimInvalidLinks()
        {
            // itterate through all pages
            foreach (var links in adjacencyList)
            {
                num_of_edges += links.Value.Count;
                HashSet<string> new_links = new HashSet<string>();
                // itterate through all links in page
                foreach (var item in links.Value)
                {
                    string trimmed = item;
                    // itterate through trimming patterns
                    foreach (var pattern in prefixes)
                    {
                        if (pattern.Length <= trimmed.Length) {
                            bool patternFound = true;
                            for (int i = 0; i < pattern.Length-1; i++)
                            {
                                if (trimmed[i] != pattern[i]) {
                                    patternFound = false;
                                    break;
                                }
                            }
                            // if pattern is found the substring not including the pattern
                            if (patternFound) {
                                trimmed = trimmed.Substring(pattern.Length, trimmed.Length - pattern.Length);
                            }
                        }
                    }
                    new_links.Add(trimmed);
                }
                adjacencyList[links.Key] = new_links;
            }
        }

        private void ClearNonExistenLinks()
        {
            // clear jpg, png, pdf etc
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

        public void PrintGraphStats()
        {
            string[] columns = {"#Nodes", "#Edges"};
            string[,] values = {{nodes.Count.ToString(), num_of_edges.ToString()}};
            AsciiTablePrinter.PrintTable(columns, values);
        }


        public IEnumerable<string> SearchArticles(string searchQuery)
        {
            string[] searchTerms = searchQuery.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> regexPatterns = new();

            // Generate regex patterns for each search term
            foreach (string searchTerm in searchTerms)
            {
                string processedTerm = searchTerm;
                bool startWith = processedTerm.StartsWith("^");
                bool endWith = processedTerm.EndsWith("$");

                if (startWith)
                    processedTerm = processedTerm.TrimStart('^');

                if (endWith)
                    processedTerm = processedTerm.TrimEnd('$');

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
            return searchResults;
        }

        static List<string> ReconstructPath(string node, Dictionary<string, string> previous)
        {
            // reconstruct path by following predecessors of each node
            List<string> path = new();
            while (previous.ContainsKey(node))
            {
                path.Add(node);
                node = previous[node];
            }
            path.Add(node);
            path.Reverse();
            return path;
        }

        public List<string> NaiveFindPath(string start, string end)
        {
            // implementation of uninformed search algorithm
            Dictionary<string, int> distances = new();
            Dictionary<string, string> previous = new();
            Queue<string> queue = new();
            queue.Enqueue(start);
            distances.Add(start, 0);
            
            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                if (current == end) { break; }

                foreach (string neighbor in GetNeighbors(current))
                {
                    if (!distances.ContainsKey(neighbor))
                    {
                        distances.Add(neighbor, distances[current] + 1);
                        previous.Add(neighbor, current);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return ReconstructPath(end, previous);
        }
    }


}
