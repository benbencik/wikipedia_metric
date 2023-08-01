using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace wikipedia_metric
{
    internal static class Program
    {


        private static void Main(string[] args)
        {   
            var logger = new Logger(nameof(Main));
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Dictionary<string, HashSet<string>> map = JsonManager.FromFile("data/large-wiki.json");
                // convert HashSet to List                
                var graph = new Graph(map);
                var clustering = new ClusteringAlgorithms(graph);
                clustering.ClusterByDegree(100);

                

                
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            stopwatch.Stop();
            logger.Info($"Finished in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}