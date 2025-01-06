using Serilog;

namespace Day20;

public record Node(Cell Cell, List<Edge> Edges)
{
    public override string ToString() => $"{{Node [{Cell.Y}, {Cell.X}]}}";
}

public record Edge(Node Child, Move Move);

public record Graph
{
    public static Graph FromMaze(Maze maze)
    {
        var cells = maze.cells;
        var logger = maze.logger;

        var nodes = new Node[cells.GetLength(0), cells.GetLength(1)];
        Node? startNode = null, endNode = null;

        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                var cell = cells[i, j];
                if (cell.ContainsWall)
                    continue;

                nodes[cell.Y, cell.X] = new Node(cell, []);

                if (cell.IsStart)
                    startNode = nodes[cell.Y, cell.X];
                if (cell.IsEnd)
                    endNode = nodes[cell.Y, cell.X];
            }
        }

        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                var cell = cells[i, j];

                if (cell.ContainsWall)
                    continue;

                var neighbours = GetNeighbours(cells, (i, j));
                foreach (var (move, y, x) in neighbours)
                {
                    var neighbourCell = cells[y, x];

                    nodes[cell.Y, cell.X].Edges.Add(new Edge(nodes[neighbourCell.Y, neighbourCell.X], move));
                }
            }
        }

        if (startNode == null || endNode == null)
            throw new InvalidOperationException("Either start or end node is null");

        return new Graph(logger, nodes, startNode, endNode);
    }

    private static (Move move, int y, int x)[] GetNeighbours(Cell[,] cells, (int y, int x) position)
    {
        var pms = possibleMoves.Select(pm => (pm.move, y: position.y + pm.y, x: position.x + pm.x)).ToArray();
        var valid = pms.Where(IsValidMove).ToArray();
        return valid.Select(pm => (pm.move, pm.y, pm.x)).ToArray();

        bool IsValidMove((Move m, int y, int x) move) =>
            IsValidPosition(move.y, move.x)
            && !cells[move.y, move.x].ContainsWall;
        bool IsValidPosition(int y, int x) => y >= 0 && y < cells.GetLength(0) && x >= 0 && x < cells.GetLength(1);
    }

    private static readonly (Move move, int y, int x)[] possibleMoves =
    [
        (Move.MoveNorth, -1, 0),
        (Move.MoveEast, 0, 1),
        (Move.MoveSouth, 1, 0),
        (Move.MoveWest, 0, -1)
    ];

    protected Graph(ILogger logger, Node[,] nodes, Node start, Node end)
    {
        Logger = logger;
        Nodes = nodes;
        Start = start;
        End = end;
    }

    protected ILogger Logger { get; }
    public Node[,] Nodes { get; }
    public Node Start { get; set; }
    public Node End { get; set; }

    public int Cost
    {
        get
        {
            var cost = FindCost();
            Logger.Verbose("Path from {Start} to {End} with no cheats has cost {Cost}", Start, End, cost);
            return cost;
        }
    }

    protected int FindCost()
    {
        var stack = new Stack<(int Cost, Node Current, Node? LastNode)>();
        stack.Push((0, Start, null));

        while (stack.Count > 0)
        {
            var (cost, currentNode, lastNode) = stack.Pop();
            
            if (currentNode == End)
            {
                return cost;
            }
            else
            {
                var edgesThatProgress = currentNode.Edges.Where(edge => edge.Child != lastNode).ToArray();
                foreach (var edge in edgesThatProgress)
                {
                    stack.Push((
                        cost + 1,
                        edge.Child,
                        currentNode
                    ));
                }
            }
        }

        throw new InvalidOperationException("No path found");
    }
}