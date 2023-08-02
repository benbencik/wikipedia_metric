// using System;
// using System.Collections.Generic;
// using System.Globalization;

// namespace wikipedia_metric
// {
//     /* Class for finding closes distance of two articles */
//     public class Search
//     {
//         Graph<string> graph = new();
//         public Search()
//         {
//             // graph inicialization
//             graph.AddVertex("A");
//             graph.AddVertex("B");
//             graph.AddVertex("C");
//             graph.AddVertex("D");
//             graph.AddEdge("A", "B");
//             graph.AddEdge("A", "C");
//             graph.AddEdge("B", "C");
//             graph.AddEdge("B", "D");


//         }

//         static List<string> ReconstructPath(string node, Dictionary<string, string> previous)
//         {
//             // reconstruct path by following predecessors of each node
//             List<string> path = new();
//             while (previous.ContainsKey(node))
//             {
//                 path.Add(node);
//                 node = previous[node];
//             }
//             path.Add(node);
//             path.Reverse();
//             return path;
//         }

//         public List<string> NaiveFindPath(string start, string end)
//         {
//             // implementation of uninformed search algorithm
//             Dictionary<string, int> distances = new();
//             Dictionary<string, string> previous = new();
//             Queue<string> queue = new();
//             queue.Enqueue(start);
//             distances.Add(start, 0);

//             while (queue.Count > 0)
//             {
//                 string current = queue.Dequeue();
//                 if (current == end) { break; }

//                 foreach (string neighbor in graph.GetNeighbors(current))
//                 {
//                     if (!distances.ContainsKey(neighbor))
//                     {
//                         distances.Add(neighbor, distances[current] + 1);
//                         previous.Add(neighbor, current);
//                         queue.Enqueue(neighbor);
//                     }
//                 }
//             }
//             return ReconstructPath(end, previous);
//         }

//         public List<string> InformedFindPath(string start, string end)
//         {
//             // TODO
//             List<string> path = new();
//             return path;
//         }
//     }
// }
