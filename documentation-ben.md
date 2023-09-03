# Wikipedia metric

In this project I have implemented modules:

- `CLI.cs`: basic commandline interface
- `Clustering.cs`: module for clustering vertices
- `Graph.cs`: stores all the information about the graph and performs search
- `Logger.cs`: used for logs and loading bar
- `PrettyTable.cs`: does nice printing orderd in table
- `Benchmarking.cs`: basic time measurements

## User Documentation

Upon starting the applicaiton the user can pick one of the following options:

1. Load graph from file
2. Search for articles
3. Print graph stats
4. Find path between two articles
5. Cluster the graph
6. Exit the applicaitondata

### Load graph from file

Before any operation a graph needs to be loaded. The project can be used on arbitrary graph which is stored in a json file. The file should be in the following format:

```json
[
    {
      "title": "Node_1",
      "links": ["Node_i", "Node_j", "Node_k" ...]
    },
    ...
    {
      "title": "Node_n",
      "links": ["Node_p", "Node_q", "Node_r" ...]
    },
]
```

Opening a file can be done either by entering a path relative to the working directory or pick picking some of the file contained in the `data` folder. All the contestns od `data` folder are displayed with corresponding number. The user can enter the number of the file he wants to load or write some arbitrary path.

### Search for articles

It is possible to search for nodes (articles) of the graph. The search is case insensitive and user can specify by `^` or `$` whether the node should start or end with the given string. Multiple search patterns could be provided separated by space.

### Print graph stats

This option prints some basic statistics about the graph, which is number of nodes, edges, average degree of out-degree.

### Find path between two articles

The first prompt is for choosing search algorithm:

- Uninformed BFS
- Expand highest degree BFS

The user then needs to enter valid source and target node, which could be found using option `2`. If some path exists then the function prints the path between them. It is important to note that the graph is directed and while there is path `A -> B` there does not need to exist `B -> A` and the other way.

### Cluster the graph

The user needs to input the number of desired clusters. Then the graph is partitioned into $n$ clusters. The graph is clusted by in degree of the nodes.

## Dev Documentation

Some more infomration about the algoritmhs used.

### Run of the application

The whole project runs in loop in `Main` which lets the user pick an aciton, untill he quits. Handling of the requests is done in `CLI` class.

### Search algorithm

The search algorithm is implemented in `Graph` class. There are two variant, uninformed search and (slightly) informed search. The uninformed one is implemented using basic BFS. The informed is also a BFS the only difference is that it uses a priority queue instead of queue. The priority is assigned by the degree of a node so it will first expand nodes with greater out-degree. This is done in hope that articles with a lot of links might provide a good connection. However according to benchmarks it is much slower, and also there is not guarantee the the path found will be the shortest.

Probably better approach would be to precompute some ionformation in order to speed up the search.

### Searching articles

Searching articles in the graph is done by `System.Text.RegularExpressions` library, so I am just constructing a regex pattern.

### Clustering algorithm

The clustering algorithm is implemented in `Clustering` class. It is a greedy algorithm which picks the node with highest in-degree. After picking the custer centers I am assigning the nodes in the neighbourhood to the cluster. The advantage of this algorithm is that it is very fast, however I cannot say how much value can this type of clustering provide. I have also tried to implement also K-means clustering however running it on the whole wikipedia took insanely long time.

## Limitations & Potential improvements

Structure of wikipedia is complicated and more knowledge about this domain would be very helpful for continuining in this project. In the end we opted for using just Slovak wikipedia since it is much smaller and easier to work with. Besides that there are numerous possibilities for improvements and other statistics.

- **Knowledge gap:** could be an area some sctucture with missing content. Naturally some topics will bne more populated than others, but it might be interesting to study which information is missing at this point. This could be dome by creating cluster of nodes and then lookin connectivity of the cluster.
- **Popular paths:** Besides that another interesting thing to do is run a lot of search quesries and count number of passing on each edge. That way be could get something like a popular path, which might represent user navigating a website.
