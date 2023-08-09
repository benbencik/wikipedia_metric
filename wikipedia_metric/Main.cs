using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace wikipedia_metric
{
    internal static class Program
    {
        private static void Main(string[] args)
        {

            bool interactive = true;
            // bool verbose = false;
            // bool benchmark = false;
            
            if (interactive) {
                GraphSearchCLI cli = new();
                while (true) {
                    var action = cli.GetAction();
                    switch (action)
                    {
                        case GraphSearchCLI.Actions.LoadGraphFromFile:
                            Console.WriteLine("Loading graph from file...");
                            cli.LoadGraphFromFile();
                            break;
                        case GraphSearchCLI.Actions.SearchForArticles:
                            Console.WriteLine("Searching for articles...");
                            cli.SearchForArticles();
                            break;
                        case GraphSearchCLI.Actions.PrintGraphStats:
                            cli.PrintGraphStats();
                            break;
                        case GraphSearchCLI.Actions.DeleteInvalidLinks:
                            cli.DeleteInvalidLinks();
                            break;
                        case GraphSearchCLI.Actions.FindPathBetweenTwoArticles:
                            cli.SearchForPathBetweenTwoArticles();
                            break;
                        case GraphSearchCLI.Actions.ClusterTheGraph:
                            cli.ClusterTheGraph();
                            break;
                        case GraphSearchCLI.Actions.ExitTheApplication:
                            Console.WriteLine("Exiting the application...");
                            Environment.Exit(0);
                            break;
                        default:
                            break;
                    }
                }

            }
            else {
                Dictionary<string, HashSet<string>> map = JsonManager.FromFile("data/g_multiword_bigger.json");
                var graph = new Graph(map);
                Benchmark benchmark = new();
                int iterations = 100;


                void informed()
                {
                    graph.NaiveFindPath("Machine learning", "Network protocols");
                }

                benchmark.Measure(informed, iterations);
                // Display the results
                Console.WriteLine("Iterations: " + benchmark.Iterations);
                Console.WriteLine("Total Elapsed Time: " + benchmark.TotalElapsedTime);
                Console.WriteLine("Average Time: " + benchmark.AverageTime);
            }

        }        
    }
}
