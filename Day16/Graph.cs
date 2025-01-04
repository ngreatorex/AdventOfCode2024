using Serilog;

namespace Day16;


public record Graph(ILogger Logger, List<Node> Nodes, Node Start, Node End)
{
    public IEnumerable<(IEnumerable<Cell> Cells, IEnumerable<Move> Moves, int Cost)> GetShortestPaths()
    {
        Dijkstra();

        var shortestPaths = BuildShortestPathFrom(new LinkedList<(Node Node, Edge? Edge)>(), End).ToList();

        foreach (var shortestPath in shortestPaths)
        {
            while (shortestPath.Last?.Previous != null && shortestPath.Last.Previous.Value.Node.Cell == shortestPath.Last.Value.Node.Cell)
                shortestPath.RemoveLast();
            if (shortestPath.Last != null)
                shortestPath.Last.Value = new(shortestPath.Last.Value.Node, null);
        }

        var pathsWithScores = shortestPaths.Select(path => (Path: path, Moves: path.Where(p => p.Edge != null).SelectMany(p => p.Edge!.Moves))).Select(path => (Cells: path.Path.Select(t => t.Node.Cell).Distinct(), path.Moves, Cost: path.Moves.Sum(m => MazeInfo.costs[m])));
        var minScore = pathsWithScores.Min(t => t.Cost);
        var minPaths = pathsWithScores.Where(t => t.Cost == minScore);

        return minPaths;
    }

    private static IEnumerable<LinkedList<(Node Node, Edge? Edge)>> BuildShortestPathFrom(LinkedList<(Node Node, Edge? Edge)> path, Node current)
    {
        if (current.CameFrom.Count == 0)
        {
            yield return path;
            yield break;
        }

        foreach (var prevNode in current.CameFrom)
        {
            var newPath = new LinkedList<(Node Node, Edge? Edge)>(path);
            var edge = prevNode.Edges.Single(x => x.Child == current);
            newPath.AddFirst((prevNode, edge));

            foreach (var completePath in BuildShortestPathFrom(newPath, prevNode))
            {
                yield return completePath;
            }
        }
    }

    private void Dijkstra()
    {
        var queue = new PriorityQueue<Node, int>();
        var visited = new HashSet<Node>();
        
        Start.CostToStart = 0;

        queue.Enqueue(Start, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentCost = current.CostToStart ?? int.MaxValue;

            if (current == End)
                return;

            foreach (var edge in current.Edges)
            {
                var neighbour = edge.Child;

                if (visited.Contains(neighbour))
                    continue;

                var alt = currentCost + edge.Weight;

                if (alt <= (neighbour.CostToStart ?? int.MaxValue))
                {
                    neighbour.CameFrom.Add(current);
                    neighbour.CostToStart = alt;

                    queue.Remove(neighbour, out _, out _);
                    queue.Enqueue(neighbour, alt);
                }
            }

            visited.Add(current);
        }
    }
}

public record Node(Direction Facing, Cell Cell, List<Edge> Edges)
{
    public int? CostToStart { get; set; }
    public HashSet<Node> CameFrom { get; set; } = [];

    public override string ToString() => $"{{Node [{Cell.Y}, {Cell.X}] facing {Facing}}}";
}

