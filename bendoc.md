# Wikipedia metric

In this project I have implemented modules:

- `CLI.cs`: basic commandline interface
- `Clustering.cs`: module for clustering vertices
- `Graph.cs`: stores all the information about the graph and performs search
- `Logger.cs`: used for logs and loading bar
- `PrettyTable.cs`: does nice printing orderd in table

## User Documentation

Upon starting the applicaiton the user can pick one of the following options:

1. Load graph from file
2. Search for articles
3. Print graph stats
4. Find path between two articles
5. Cluster the graph
6. Exit the applicaitondata/b_letterrs.json

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

This option prints some basic statistics about the graph, which is number of nodes, edges, average degree of out-degree and number of connected components.

### Find path between two articles

Basic seach is performed on the graph.

### Cluster the graph

Graph is clusterd into n by picking the nodes with highest in-degree as clusters.

## Dev Documentation

## Limitations & Potential improvements

<!-- ## Potential improvements  -->