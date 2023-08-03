using System;
using System.Collections.Generic;

namespace wikipedia_metric
{
    public class GraphSearchCLI
    {
        public Graph graph;
        public enum Actions
        {
            LoadGraphFromFile = 1,
            SearchForArticles = 2,
            PrintGraphStats = 3,
            FindPathBetweenTwoArticles = 4,
            ClusterTheGraph = 5,
            ExitTheApplication = 6
        }

        public GraphSearchCLI()
        {
            string asciiArt = @"                                                        
 __      __  _   _     _                   _   _            __  __         _           _      
 \ \    / / (_) | |__ (_)  _ __   ___   __| | (_)  __ _    |  \/  |  ___  | |_   _ _  (_)  __ 
  \ \/\/ /  | | | / / | | | '_ \ / -_) / _` | | | / _` |   | |\/| | / -_) |  _| | '_| | | / _|
   \_/\_/   |_| |_\_\ |_| | .__/ \___| \__,_| |_| \__,_|   |_|  |_| \___|  \__| |_|   |_| \__|
                          |_|                                                                 
            ";
            Console.WriteLine(asciiArt);
            Console.WriteLine("Welcome to the Wikipedia Graph Search Application!");
        }

        public Actions GetAction()
        {
            while (true)
            {
                Console.WriteLine("\n Please select an action:");
                Console.WriteLine("1. Load graph from file");
                Console.WriteLine("2. Search for articles");
                Console.WriteLine("3. Print graph stats");
                Console.WriteLine("4. Find path between two articles");
                Console.WriteLine("5. Cluster the graph");
                Console.WriteLine("6. Exit the application");
                Console.Write("Your choice: ");
                string input = Console.ReadLine().Trim();

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exiting the application...");
                    Environment.Exit(0);
                }

                if (int.TryParse(input, out int choice))
                {
                    if (choice >= 1 && choice <= 6)
                    {
                        if (graph == null && choice != 1 && choice != 6) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Please load the graph first!");
                            Console.ResetColor();
                            continue;
                        }
                        else {
                            Actions action = (Actions)choice;
                            return action;
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input, please try again.");
                Console.ResetColor();
            }
        }
        public void LoadGraphFromFile()
        {
            while (true) {
                Console.Write("Please enter the path to the graph file: ");
                string input = Console.ReadLine().Trim();

                try
                {
                    // Load graph data from the file
                    Dictionary<string, HashSet<string>> map = JsonManager.FromFile(input);
                    graph = new Graph(map);
                    Console.WriteLine("Graph loaded successfully!");
                    break;
                }
                catch (Exception)
                {
                    // Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public void SearchForArticles() {
            Console.Write("Please enter search phrase: ");
            string input = Console.ReadLine().Trim();
            IEnumerable<string> result = graph.SearchArticles(input);
            Console.WriteLine("Search results:");
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        public void PrintGraphStats() {
            graph.PrintGraphStats();
        }

        public void SearchForPathBetweenTwoArticles() {
            Console.Write("Please enter the source vertex: ");
            string sourceVertex = Console.ReadLine().Trim();
            Console.Write("Please enter the destination vertex: ");
            string destinationVertex = Console.ReadLine().Trim();
            // graph.SearchPath(sourceVertex, destinationVertex);
        }

        public void ClusterTheGraph() {
            Console.Write("Please enter number of clusters: ");
            string input = Console.ReadLine().Trim();
            int numberOfClusters = int.Parse(input);
            ClusteringAlgorithms clustering = new(graph);
            clustering.ClusterByDegree(numberOfClusters);
        }

    }
}
