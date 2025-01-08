using Priority_Queue;
using Serilog;

namespace Day21;

public record Edge(Node Child, Move Direction);
public record Node(Key Key, List<Edge> Edges);

public record Graph
{
    public Node[,] Nodes { get; }
    public Node Start { get; protected set; }
    public Node? End { get; protected set; }

    protected Graph(Node[,] nodes, Node start)
    {
        Nodes = nodes;
        Start = start;
    }

    public Graph WithStart(Key start)
    {
        return this with { Start = Nodes[start.Y, start.X] };
    }

    public Graph WithEnd(Key end)
    {
        return this with { End = Nodes[end.Y, end.X] };
    }

    public List<Move> Solve()
    {
        var path = AStar();
        if (path.Count < 2)
        {
            return [];
        }

        var results = new List<Move>(path.Count);
        
        var current = path.First;
        var next = current!.Next;

        while (next != null)
        {
            results.Add(current.Value.Edges.Single(e => e.Child == next.Value).Direction);
            current = next;
            next = current.Next;
        }

        return results;
    }

    protected static int GetHeuristic(Node goal, Node current) => Math.Abs(goal.Key.X - current.Key.X) + Math.Abs(goal.Key.Y - current.Key.Y);

    protected LinkedList<Node> AStar()
    {
        if (End == null)
            throw new InvalidOperationException("End goal is not set");

        SimplePriorityQueue<Node> frontier = new();
        Dictionary<Node, Node?> cameFrom = [];
        Dictionary<Node, int> costSoFar = new()
        {
            { Start, 0 }
        };

        frontier.Enqueue(Start, 0);
        cameFrom.Add(Start, null);

        while (frontier.Count != 0)
        {
            var current = frontier.Dequeue();

            if (current == End)
                break;

            foreach (var edge in current.Edges)
            {
                var neighbor = edge.Child;
                costSoFar.TryGetValue(current, out int newCost);
                newCost += 1;

                if (!costSoFar.TryGetValue(neighbor, out int value) || newCost < value)
                {
                    if (!costSoFar.TryAdd(neighbor, newCost))
                        costSoFar[neighbor] = newCost;

                    float priority = newCost + GetHeuristic(End, neighbor);

                    frontier.Enqueue(neighbor, priority);

                    if (!cameFrom.TryAdd(neighbor, current))
                        cameFrom[neighbor] = current;
                }
            }
        }

        return ConstructPath(cameFrom, End);
    }

    protected static LinkedList<Node> ConstructPath(Dictionary<Node, Node?> cameFrom, Node? goal)
    {
        if (goal == null || !cameFrom.ContainsKey(goal))
            return [];

        LinkedList<Node> path = new();
        Node? current = goal;

        while (current != null)
        {
            path.AddFirst(current);
            cameFrom.TryGetValue(current, out current);
        }

        return path;
    }


    public static Graph FromKeypad(Keypad keypad)
    {
        var nodes = new Node[keypad.Height, keypad.Width];

        for (var i = 0; i < keypad.Keys.GetLength(0); i++)
        {
            for (var j = 0; j < keypad.Keys.GetLength(1); j++)
            {
                if (keypad.Keys[i, j] == null)
                {
                    continue;
                }

                nodes[i, j] = new Node(keypad.Keys[i, j]!, []);
            }
        }

        for (var i = 0; i < keypad.Keys.GetLength(0); i++)
        {
            for (var j = 0; j < keypad.Keys.GetLength(1); j++)
            {
                if (keypad.Keys[i, j] == null)
                {
                    continue;
                }

                if (IsValidIndex(0, i - 1) && nodes[i - 1, j] != null)
                {
                    nodes[i, j].Edges.Add(new Edge(nodes[i - 1, j]!, Move.North));
                }
                if (IsValidIndex(0, i + 1) && nodes[i + 1, j] != null)
                {
                    nodes[i, j].Edges.Add(new Edge(nodes[i + 1, j]!, Move.South));
                }
                if (IsValidIndex(1, j - 1) && nodes[i, j - 1] != null)
                {
                    nodes[i, j].Edges.Add(new Edge(nodes[i, j - 1]!, Move.West));
                }
                if (IsValidIndex(1, j + 1) && nodes[i, j + 1] != null)
                {
                    nodes[i, j].Edges.Add(new Edge(nodes[i, j + 1]!, Move.East));
                }
            }
        }

        return new Graph(nodes, nodes[keypad.Current.Y, keypad.Current.X]);

        bool IsValidIndex(int rank, int index) => index >= 0 && index < keypad.Keys.GetLength(rank);
    }
}
