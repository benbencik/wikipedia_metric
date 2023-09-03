using System;
using System.Collections.Generic;

namespace wikipedia_metric
{
    public class Cluster
    {
        public string ClusterCenter { get; }
        public List<string> ClusterMembers { get; }

        public Cluster(string center)
        {
            ClusterCenter = center;
            ClusterMembers = new List<string>();
        }

        public void AddMember(string member)
        {
            ClusterMembers.Add(member);
        }

        public string[] GetStats(Dictionary<string, int> inDegree)
        {
            string[] stats = new string[4];
            stats[0] = ClusterCenter;
            stats[1] = ClusterMembers.Count.ToString();
            stats[2] = inDegree[ClusterCenter].ToString();
            return stats;
        }

    }

    public class ClusteringAlgorithms
    {
        readonly Graph graph;
        readonly Logger logger = new("ClusteringAlgorithms");

        public Dictionary<string, int> inDegree = new();
        Dictionary<string, Cluster> clusters = new();
        public ClusteringAlgorithms(Graph graph)
        {
            this.graph = graph;
        }

        public void ClusterByDegree(int numberOfClusters, bool printTable = true)
        {
            // method for calling clustering by in-degree of nodes in the ghraph
            logger.Info("Clustering by degree");
            ClusterCenterByDegree(numberOfClusters);

            if (printTable && clusters.Count > 0)
            {
                PrintClusters();
            }
        }

        public void CalculateInDegrees()
        {
            foreach (string node in graph.nodes)
            {
                foreach (string neighbor in graph.adjacencyList[node])
                {
                    if (!inDegree.ContainsKey(neighbor))
                    {
                        inDegree[neighbor] = 1;
                    }
                    else
                    {
                        inDegree[neighbor]++;
                    }
                }
            }
        }

        public int? GetInDegree(string vertex)
        {
            if (inDegree.ContainsKey(vertex))
            {
                return inDegree[vertex];
            }
            else
            {
                return null;
            }
        }


        private List<string> SortNPick(int n)
        {
            List<string> sortedNodes = new List<string>(inDegree.Keys);
            sortedNodes.Sort((a, b) => inDegree[b].CompareTo(inDegree[a]));

            List<string> topNodes = sortedNodes.GetRange(0, Math.Min(n, sortedNodes.Count));
            return topNodes;
        }

        void ClusterCenterByDegree(int numberOfClusters, bool progressBar = true)
        {
            // the algorithm starts with sorting of the nodes by their in-degree
            // and then pick the top n which will represent cluster center

            // PICKING TOP N NODES
            if (inDegree.Count == 0)
            {
                CalculateInDegrees();
            }

            if (inDegree.Count < numberOfClusters)
            {
                logger.Warning($"Number of clusters is greater than number of nodes in the graph...");
                return;
            }

            List<string> topNodes = SortNPick(numberOfClusters);
            Console.WriteLine($"Picked {topNodes.Count} nodes as cluster centers");

            // ASSIGNING NODES TO CLUSTERS
            Queue<KeyValuePair<string, Cluster>> queue = new();
            HashSet<string> visited = new();
            foreach (string node in topNodes)
            {
                Cluster newCluster = new(node);
                clusters[node] = newCluster;
                visited.Add(node);
                queue.Enqueue(new KeyValuePair<string, Cluster>(node, newCluster));
            }

            int counter = 0;
            int missed = 0;
            while (queue.Count > 0)
            {
                if (counter < graph.nodes.Count) counter++;
                if (counter % 10_000 == 0 && progressBar)
                {
                    logger.LoadingBar(graph.nodes.Count, counter);
                }

                KeyValuePair<string, Cluster> current = queue.Dequeue();
                visited.Add(current.Key);
                if (graph.adjacencyList.ContainsKey(current.Key))
                {

                    foreach (string neighbor in graph.adjacencyList[current.Key])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            current.Value.AddMember(neighbor);  // add article to a cluster
                            // graph.cluster[neighbor] = current.Value;
                            visited.Add(neighbor);
                            queue.Enqueue(new KeyValuePair<string, Cluster>(neighbor, current.Value));

                        }
                    }
                }
                else
                {
                    missed++;
                    logger.Warning($"Graph does not contain {current.Key}");
                }
            }

        }

        private void PrintClusters()
        {
            string[] columnNames = { "Cluster Center", "Number of Members", "Degree of Center" };
            string[,] table = new string[clusters.Count, 3];
            int counter = 0;
            foreach (Cluster cluster in clusters.Values)
            {
                string[] stats = cluster.GetStats(inDegree);
                table[counter, 0] = stats[0];
                table[counter, 1] = stats[1];
                table[counter, 2] = stats[2];
                counter++;
            }
            AsciiTablePrinter.PrintTable(columnNames, table);
        }
    }
}
