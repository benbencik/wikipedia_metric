using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;

namespace wikipedia_metric
{
    public class Cluster
    {
        readonly Graph graph;
        public string ClusterCenter { get; }
        public List<string> ClusterMembers { get; }

        public Cluster(Graph graph, string center)
        {
            ClusterCenter = center;
            ClusterMembers = new List<string>();
        }

        public void AddMember(string member)
        {
            ClusterMembers.Add(member);
        }

        int AverageNumberOfLinks() {
            if (ClusterMembers.Count == 0) return 0;

            int average = 0;
            for (int i = 0; i < ClusterMembers.Count; i++) {
                if (graph.adjacencyList.ContainsKey(ClusterMembers[i])){
                    average += graph.adjacencyList[ClusterMembers[i]].Count;
                }
            }
            return average / ClusterMembers.Count;
        }

        public string[] GetStats(Dictionary<string, int> inDegree) {
            string[] stats = new string[4];
            stats[0] = ClusterCenter;
            stats[1] = ClusterMembers.Count.ToString();
            stats[2] = inDegree[ClusterCenter].ToString();
            stats[3] = "nic zatial";
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

            if (printTable) {
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

                    
        private List<string> HeapNPick(int n)
        {
            SortedSet<KeyValuePair<string, int>> minHeap = new SortedSet<KeyValuePair<string, int>>(
                Comparer<KeyValuePair<string, int>>.Create((kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value))
            );

            foreach (var kvp in inDegree)
            {
                if (minHeap.Count < n)
                {
                    minHeap.Add(kvp);
                }
                else if (kvp.Value > minHeap.Min.Value)
                {
                    minHeap.Remove(minHeap.Min);
                    minHeap.Add(kvp);
                }
            }

            List<string> topNKeys = new List<string>();
            foreach (var kvp in minHeap)
            {
                topNKeys.Add(kvp.Key);
            }

            return topNKeys;
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
            List<string> topNodes = HeapNPick(numberOfClusters);
            // List<string> topNodes = SortNPick(numberOfClusters);

            // ASSIGNING NODES TO CLUSTERS
            Queue<KeyValuePair<string, Cluster>> queue = new();
            HashSet<string> visited = new();
            foreach (string node in topNodes)
            {
                Cluster newCluster = new(graph, node);
                clusters[node] = newCluster;
                visited.Add(node);
                queue.Enqueue(new KeyValuePair<string, Cluster>(node, newCluster));
            }

            int counter = 0;
            int missed = 0;
            while (queue.Count > 0) {
                if (counter < graph.nodes.Count) counter ++;
                if (counter % 10_000 == 0 && progressBar)
                {
                    logger.LoadingBar(graph.nodes.Count, counter);
                }

                KeyValuePair<string, Cluster> current = queue.Dequeue();
                if (graph.adjacencyList.ContainsKey(current.Key)) {

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
                else {
                    missed ++;
                    // logger.Warning($"Graph does not contain {current.Key}");
                }
            }
        }

        private void PrintClusters()
        {
            string[] columnNames = { "Cluster Center", "Number of Members", "Degree of Center", "Average Number of Links" };
            string[,] table = new string[clusters.Count, 4];
            int counter = 0;
            foreach (Cluster cluster in clusters.Values)
            {
                string[] stats = cluster.GetStats(inDegree);
                table[counter, 0] = stats[0];
                table[counter, 1] = stats[1];
                table[counter, 2] = stats[2];
                table[counter, 3] = stats[3];
                counter++;
            }
            AsciiTablePrinter.PrintTable(columnNames, table);    
        }

        public double EvaluatePerformance()
        {
            // Calculate the sum of squared distances of nodes from their cluster centers
            double sumSquaredDistances = 0;
            foreach (var kvp in clusters)
            {
                string center = kvp.Key;
                List<string> members = kvp.Value.ClusterMembers;

                int centerDegree = GetInDegree(center) ?? 0;
                foreach (string member in members)
                {
                    int memberDegree = GetInDegree(member) ?? 0;
                    sumSquaredDistances += Math.Pow(memberDegree - centerDegree, 2);
                }
            }

            return sumSquaredDistances;
        }
    }
}
