using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace wikipedia_metric
{
    public class GraphSearchCLI
    {
        public Graph graph;
        Logger logger = new("CLI");
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
                Console.WriteLine("\n\nPlease select an action:");
                Console.WriteLine("1. Load graph from file");
                Console.WriteLine("2. Search for articles");
                Console.WriteLine("3. Print graph stats");
                Console.WriteLine("4. Find path between two articles");
                Console.WriteLine("5. Cluster the graph");
                Console.WriteLine("6. Exit the applicaiton");
                Console.Write("Your choice: ");
           
                string input = Console.ReadLine().Trim();

                if (int.TryParse(input, out int choice))
                {
                    if (choice >= 1 && choice <= 6)
                    {
                        if (graph == null && choice != 1 && choice != 6) {
                            logger.Error("Please load the graph first!");
                            continue;
                        }
                        else {
                            Actions action = (Actions)choice;
                            return action;
                        }
                    }
                }
                logger.Error("Invalid input, please try again.");
            }
        }
        public void LoadGraphFromFile()
        {
            string dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
            string[] files = Directory.GetFiles(dataDirectory);
            // sort files by name
            Array.Sort(files, (x, y) => String.Compare(x, y));

            // List the files in the "data" directory
            Console.WriteLine("\nAvailable files:");
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                Console.WriteLine($"{i + 1}. {fileName}");
            }

            while (true) {
                Console.Write("Please enter the path to the graph file: ");
                string input = Console.ReadLine().Trim();

                try
                {
                    if (int.TryParse(input, out int fileNumber))  // Load by choice
                    {
                        // User entered a number, check if it's valid
                        if (fileNumber >= 1 && fileNumber <= files.Length)
                        {
                            // Load graph data from the selected file
                            string selectedFile = files[fileNumber - 1];
                            Dictionary<string, HashSet<string>> map = JsonManager.FromFile(files[fileNumber - 1]);
                            graph = new Graph(map);
                            Console.WriteLine($"Graph loaded from {selectedFile}");
                            break;
                        }
                        else
                        {
                            logger.Error("Invalid file number.");
                            // Console.WriteLine("Invalid file number.");
                        }
                    }
                    else  // Load by path 
                    {
                        Dictionary<string, HashSet<string>> map = JsonManager.FromFile(input);
                        graph = new Graph(map);
                        break;
                    }
                }
                catch (Exception)
                {
                    // Console.WriteLine($"Error: {ex.Message}");
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Graph loaded successfully!");
            Console.ResetColor();
        }

        public void SearchForArticles() {
            Console.WriteLine("Please enter search phrase: ");
            Console.WriteLine("...use ^ to search for articles starting with given phrase:");
            Console.WriteLine("...use $ to search for articles ending with given phrase:");
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
            int i = 0;
            string[] info = {"Enter a valid source vertex: ",
                             "Enter a valid target vertex: "};
            string[] vertices = {"", ""};
            
            while (i < 2) {
                Console.WriteLine(info[i]);
                string input = Console.ReadLine().Trim();
                if (graph.ContainsVertex(input)) {
                    vertices[i] = input;
                    i ++;
                }
                else {
                    // Console.ForegroundColor = ConsoleColor.Red;
                    // Console.WriteLine($"{input} is not contained in the graph");
                    // Console.ResetColor();
                }
            }

            List<string> path = graph.NaiveFindPath(vertices[0], vertices[1]);
            foreach (string n in path) {
                Console.Write($"{n}, "); 
            }
            // graph.SearchPath(vertices[0], vertices[1]);
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
