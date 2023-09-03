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

            if (interactive)
            {
                GraphSearchCLI cli = new();
                while (true)
                {
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
            else
            {
                Dictionary<string, HashSet<string>> map = JsonManager.FromFile("data/sk.json");
                var graph = new Graph(map);

                Benchmark benchmark = new();
                int iterations = 5;


                void naive()
                {
                    graph.NaiveFindPath("Univerzita Karlova", "Betón");
                    graph.NaiveFindPath("Havaj", "ChatGPT");
                    graph.NaiveFindPath("Malá Fatra", "Napoleón");
                    graph.NaiveFindPath("Bruce Dickinson", "Zuzana Čaputová");
                    graph.NaiveFindPath("Mačka", "Jozef II.");
                }

                void naive_priority()
                {
                    graph.PriorityFindPath("Univerzita Karlova", "Betón");
                    graph.PriorityFindPath("Havaj", "ChatGPT");
                    graph.PriorityFindPath("Malá Fatra", "Napoleón");
                    graph.PriorityFindPath("Bruce Dickinson", "Zuzana Čaputová");
                    graph.PriorityFindPath("Mačka", "Jozef II.");

                }

                benchmark.Measure(naive, iterations);
                // Display the results
                Console.WriteLine("Naive BFS --------------------------");
                Console.WriteLine("Iterations: " + benchmark.Iterations);
                Console.WriteLine("Total Elapsed Time: " + benchmark.TotalElapsedTime);
                Console.WriteLine("Average Time: " + benchmark.AverageTime + "\n");

                benchmark.Measure(naive_priority, iterations);
                // Display the results
                Console.WriteLine("\nNaive priority by degree BFS-------");
                Console.WriteLine("Iterations: " + benchmark.Iterations);
                Console.WriteLine("Total Elapsed Time: " + benchmark.TotalElapsedTime);
                Console.WriteLine("Average Time: " + benchmark.AverageTime);

            }

        }
    }
}
