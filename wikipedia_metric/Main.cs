using System;
using System.Collections.Generic;
using System.Diagnostics;

// System.CommandLine.NamingConventionBinder;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace wikipedia_metric
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            bool interactive = true;
            // bool verbose = false;
            // bool stoptime = false;
            
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
                            Console.WriteLine("Printing graph stats...");
                            break;
                        case GraphSearchCLI.Actions.FindPathBetweenTwoArticles:
                            Console.WriteLine("Finding path between two articles...");
                            break;
                        case GraphSearchCLI.Actions.ClusterTheGraph:
                            Console.WriteLine("Clustering the graph...");
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
                Dictionary<string, HashSet<string>> map = JsonManager.FromFile("data/large-wiki.json");
                var graph = new Graph(map);

            }

        }        
    }
}
