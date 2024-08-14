namespace Orbit.Server.Mesh.Graph;

/// <summary>
///     A simple, lightweight bidirectional graph optimized around Dijkstra's shortest path
/// </summary>
public sealed class Graph
{
    private readonly int _edgesToNodesCapacity;
    private int _lastIndex;
    private int _nodeCapacity;
    private List<(int target, float weight)>[] _nodesAndEdges;

    /// <summary>
    ///     Create a new graph with some initial capacity
    /// </summary>
    /// <param name="nodeCapacity">The expected number of nodes this graph will contain</param>
    /// <param name="edgesToNodesCapacity">The expected number of outgoing edges that each node might have</param>
    public Graph(int nodeCapacity, int edgesToNodesCapacity = 8)
    {
        if (nodeCapacity <= 0)
        {
            throw new Exception("NodeCapacity must be greater than 0");
        }

        if (edgesToNodesCapacity <= 0)
        {
            throw new Exception("EdgesToNodesCapacity must be greater than 0");
        }

        _lastIndex = nodeCapacity - 1;
        _nodesAndEdges = new List<(int target, float weight)>[nodeCapacity];
        _edgesToNodesCapacity = edgesToNodesCapacity;
        _nodeCapacity = nodeCapacity;
    }

    /// <summary>
    ///     The amount of nodes this graph can currently hold
    /// </summary>
    public int Capacity => _nodesAndEdges.Length;

    /// <summary>
    ///     Total number of edges in the graph
    /// </summary>
    public int EdgesCount { get; private set; }

    /// <summary>
    ///     Total number of nodes in the graph
    /// </summary>
    public int NodesCount { get; private set; }

    /// <summary>
    ///     Add a bidirectional edge between between two nodes to the graph
    /// </summary>
    /// <param name="sourceNode">Source node</param>
    /// <param name="targetNode">Target node</param>
    public void AddEdge(int sourceNode, int targetNode, float weight = 1.0f)
    {
        if (sourceNode < 0)
        {
            throw new Exception("Source node cannot be a negative number");
        }

        if (targetNode < 0)
        {
            throw new Exception("Target node cannot be a negative number");
        }

        if (sourceNode == targetNode)
        {
            throw new Exception("Source node and target node are the same");
        }

        if (weight < 0.0f)
        {
            throw new Exception("Weight cannot be negative");
        }

        if (sourceNode > _lastIndex || targetNode > _lastIndex)
        {
            var newCapacity = sourceNode > targetNode ? sourceNode + 1 : targetNode + 1;
            Array.Resize(ref _nodesAndEdges, newCapacity);
            _nodeCapacity = newCapacity;
            _lastIndex = newCapacity - 1;
        }

        if (_nodesAndEdges[sourceNode] == null)
        {
            _nodesAndEdges[sourceNode] = new List<(int target, float weight)>(_edgesToNodesCapacity);
            NodesCount++;
        }

        if (_nodesAndEdges[targetNode] == null)
        {
            _nodesAndEdges[targetNode] = new List<(int target, float weight)>(_edgesToNodesCapacity);
            NodesCount++;
        }

        if (!_nodesAndEdges[sourceNode].Any(e => e.target == targetNode))
        {
            _nodesAndEdges[sourceNode].Add((targetNode, weight));
            EdgesCount++;
        }

        if (!_nodesAndEdges[targetNode].Any(e => e.target == sourceNode))
        {
            _nodesAndEdges[targetNode].Add((sourceNode, weight));
            EdgesCount++;
        }
    }

    /// <summary>
    ///     Removes an bidirectional edge between two nodes
    /// </summary>
    /// <param name="sourceNode">Source node</param>
    /// <param name="targetNode">Target node</param>
    public void RemoveEdge(int sourceNode, int targetNode)
    {
        if (!_nodesAndEdges[sourceNode].Any(n => n.target == targetNode))
        {
            throw new Exception($"Edge from {sourceNode} to {targetNode} was not found");
        }

        if (!_nodesAndEdges[targetNode].Any(n => n.target == sourceNode))
        {
            throw new Exception($"Edge from {targetNode} to {targetNode} was not found");
        }

        var edge1 = _nodesAndEdges[sourceNode].Where(n => n.target == targetNode).Single();
        var edge2 = _nodesAndEdges[targetNode].Where(n => n.target == sourceNode).Single();
        _nodesAndEdges[sourceNode].Remove(edge1);
        _nodesAndEdges[targetNode].Remove(edge2);
        EdgesCount -= 2;
    }

    /// <summary>
    ///     Check if two nodes are connected
    /// </summary>
    /// <param name="nodeA">First node to be checked</param>
    /// <param name="nodeB">Second node to be checked</param>
    /// <returns>True if the two nodes are connected</returns>
    public bool IsConnected(int nodeA, int nodeB)
    {
        if (_nodesAndEdges[nodeA] != null && _nodesAndEdges[nodeB] != null)
        {
            return _nodesAndEdges[nodeA].Any(k => k.target == nodeB) &&
                   _nodesAndEdges[nodeB].Any(k => k.target == nodeA);
        }

        if (_nodesAndEdges[nodeA] == null)
        {
            throw new Exception($"Node {nodeA} does not exist");
        }

        if (_nodesAndEdges[nodeB] == null)
        {
            throw new Exception($"Node {nodeB} does not exist");
        }

        return false;
    }

    /// <summary>
    ///     Find a route between two nodes using Dijkstra's shortest path
    /// </summary>
    /// <param name="startNode">Start node</param>
    /// <param name="endNode">End node</param>
    /// ///
    /// <returns>A collection of nodes containing the shortest path and the total distance from startNode to endNode</returns>
    public (IEnumerable<int> nodes, float distance) GetRoute(int startNode, int endNode)
    {
        var predecessors = new int[_nodeCapacity];
        var priorityQueue = new List<(int node, float distance)>(_edgesToNodesCapacity);
        var distances = GetDistanceArray(_nodeCapacity, startNode);
        priorityQueue.Add((startNode, 0));
        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue[0].node;
            if (currentNode == endNode)
            {
                break;
            }

            priorityQueue.Remove(priorityQueue[0]);
            for (var outgoingEdge = 0; outgoingEdge < _nodesAndEdges[currentNode].Count; outgoingEdge++)
            {
                var outgoingNode = _nodesAndEdges[currentNode][outgoingEdge].target;
                var oldDistance = distances[outgoingNode];
                var newDistance = distances[currentNode] + _nodesAndEdges[currentNode][outgoingEdge].weight;
                if (oldDistance > newDistance)
                {
                    predecessors[outgoingNode] = currentNode;
                    distances[outgoingNode] = newDistance;
                    var queueIndex = priorityQueue.IndexOf((outgoingNode, oldDistance));
                    if (queueIndex != -1)
                    {
                        priorityQueue[queueIndex] = (outgoingNode, newDistance);
                    }
                    else
                    {
                        priorityQueue.Add((outgoingNode, newDistance));
                    }

                    priorityQueue.Sort((k, v) => k.distance.CompareTo(v.distance));
                }
            }
        }

        var shortestPath = new Stack<int>(_nodeCapacity);
        shortestPath.Push(endNode);
        var pathNode = endNode;
        while (distances[pathNode] != 0)
        {
            shortestPath.Push(predecessors[pathNode]);
            pathNode = predecessors[pathNode];
        }

        return (shortestPath, distances[endNode]);
    }

    private static float[] GetDistanceArray(int nodeCapacity, int startNode)
    {
        var distances = new float[nodeCapacity];
        for (var i = 0; i < nodeCapacity; i++)
        {
            distances[i] = float.MaxValue;
        }

        distances[startNode] = 0;
        return distances;
    }
}

public class DefaultDirectedGraph<T>
{
    public Graph Graph;
    public List<T> Vertexs = new();

    public void BuildVertex()
    {
        Graph = new Graph(Vertexs.Count);
    }

    public bool ContainsVertex(T sourceNode)
    {
        return Vertexs.Contains(sourceNode);
    }

    public void AddVertex(T nodeId)
    {
        Vertexs.Add(nodeId);
    }


    public void AddEdge(T nodeId, T visibleNode)
    {
        Graph.AddEdge(Vertexs.IndexOf(nodeId), Vertexs.IndexOf(visibleNode));
    }


    public List<T> FindPathBetween(T source, T sink)
    {
        var graphPath = new List<T>();
        if (!ContainsVertex(source))
        {
            throw new Exception("GRAPH_MUST_CONTAIN_THE_SOURCE_VERTEX");
        }

        if (!ContainsVertex(sink))
        {
            throw new Exception("GRAPH_MUST_CONTAIN_THE_SINK_VERTEX");
        }

        if (source.Equals(sink))
        {
            graphPath = new List<T>
            {
                source
            };
            return graphPath;
        }

        var startNode = Vertexs.IndexOf(source);
        var endNode = Vertexs.IndexOf(sink);
        graphPath = Graph.GetRoute(startNode, endNode).nodes.Select(n => Vertexs[n]).ToList();
        return graphPath;
    }
}