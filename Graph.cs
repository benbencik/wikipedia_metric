using System.Collections.Generic;

namespace wikipedia_metric
{
    public class Graph<T>
    {
        private Dictionary<T, List<T>> adjacencyList;

        public Graph()
        {
            adjacencyList = new Dictionary<T, List<T>>();
        }

        public void AddVertex(T vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = new List<T>();
            }
        }

        public void AddEdge(T source, T destination)
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

        public List<T> GetNeighbors(T vertex)
        {
            if (adjacencyList.ContainsKey(vertex))
            {
                return adjacencyList[vertex];
            }
            return new List<T>();
        }

        public bool ContainsVertex(T vertex)
        {
            return adjacencyList.ContainsKey(vertex);
        }

        public void LoadFromJson(string path)
        {
            
        }
    }
}
