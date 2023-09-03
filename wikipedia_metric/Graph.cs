using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace wikipedia_metric
{
    public class Graph
    {
        public Dictionary<string, HashSet<string>> adjacencyList;
        public Dictionary<string, Cluster> cluster;
        public List<string> nodes;

        readonly Logger logger = new("Graph");

        readonly private string[] prefixes = { "Wikipédia:" };

        public Graph(Dictionary<string, HashSet<string>> article_dictionary)
        {
            adjacencyList = article_dictionary;
            logger.Info($"Graph successfully loaded");

            FilterData();
            DeleteInvalidLinks();
            nodes = new List<string>(adjacencyList.Keys);
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

        private void FilterData()
        {
            logger.Info("Filtering data...");
            int n = adjacencyList.Count;
            int counter = 0;
            foreach (var article in adjacencyList)
            {
                if (counter % 10 == 0)
                {
                    logger.LoadingBar(n, counter);
                }
                counter++;

                // remove aricles without links
                if (article.Value.Count == 0)
                {
                    adjacencyList.Remove(article.Key);
                }
                else
                {
                    // remove articles with invalid prefixes
                    bool deleted = false;
                    foreach (string prefix in prefixes)
                    {
                        if (article.Key.StartsWith(prefix))
                        {
                            adjacencyList.Remove(article.Key);
                            deleted = true;
                            break;
                        }
                    }
                    if (deleted) continue;

                    // remove articles with non-exitent links and invalid prefixes from links
                    foreach (var link in article.Value)
                    {
                        foreach (string prefix in prefixes)
                        {
                            if (link.StartsWith(prefix))
                            {
                                adjacencyList[article.Key].Remove(link);
                            }
                        }
                    }
                }
            }
        }


        public void DeleteInvalidLinks()
        {
            // clear jpg, png, pdf etc
            logger.Info("Clearing non existent links");
            int counter = 0;
            int counter_bar = 0;
            foreach (var item in adjacencyList)
            {
                if (counter_bar % 100 == 0)
                {
                    logger.LoadingBar(adjacencyList.Count, counter_bar);
                }
                counter_bar++;

                HashSet<String> new_links = new();
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
            int sum = 0;
            foreach (var item in adjacencyList)
            {
                sum += item.Value.Count;
            }
            double average = (double)sum / (double)adjacencyList.Count;
            string averageDeg = average.ToString("#.0000");

            string[] columns = { "#Nodes", "#Edges", "Average degree" };
            string[,] values = { { adjacencyList.Count.ToString(), sum.ToString(), averageDeg } };
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
            // Sort the search results by length
            searchResults = searchResults.OrderByDescending(article => article.Length);
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

        public List<string> NaiveFindPath(string source, string target)
        {
            // implementation of uninformed search algorithm
            Dictionary<string, int> distances = new();
            Dictionary<string, string> previous = new();
            Queue<string> queue = new();
            queue.Enqueue(source);
            distances.Add(source, 0);

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                if (current == target) { break; }

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
            return ReconstructPath(target, previous);
        }

        public List<string> PriorityFindPath(string source, string target)
        {
            // Initialize a priority queue based on node degrees
            PriorityQueue<string, int> priorityQueue = new PriorityQueue<string, int>();

            // Initialize dictionaries for distances and previous nodes
            Dictionary<string, int> distances = new();
            Dictionary<string, string> previous = new();

            // Add the source node with a degree of 0
            priorityQueue.Enqueue(source, GetNeighbors(source).Count);
            distances.Add(source, 0);

            while (priorityQueue.Count > 0)
            {
                var currentNode = priorityQueue.Dequeue();
                if (currentNode == target) { break; }

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    int newDegree = GetNeighbors(neighbor).Count;
                    if (!distances.ContainsKey(neighbor))
                    {
                        distances.Add(neighbor, distances[currentNode] + 1);
                        previous.Add(neighbor, currentNode);
                        priorityQueue.Enqueue(neighbor, newDegree);
                    }
                }
            }

            return ReconstructPath(target, previous);
        }

    }

}

