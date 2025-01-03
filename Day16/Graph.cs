namespace Day16;


    public record Graph(List<Node> Nodes, Node Start, Node End)
    {
        public LinkedList<(Node Node, Edge? Edge)> GetShortestPath()
        {
            Dijkstra();

            LinkedList<(Node Node, Edge? Edge)> shortestPath = new();
            shortestPath.AddFirst((End, null));

            BuildShortestPath(shortestPath, End);

            while (shortestPath.Last?.Previous != null && shortestPath.Last.Previous.Value.Node.Cell == shortestPath.Last.Value.Node.Cell)
                shortestPath.RemoveLast();
            if (shortestPath.Last != null)
                shortestPath.Last.Value = new(shortestPath.Last.Value.Node, null);

            return shortestPath;
        }
        
        private void Dijkstra()
        {
            Start.MinCostToStart = 0;
            List<Node> prioQueue = [Start];
            var startToEndCost = double.MaxValue;

            do
            {
                prioQueue = [.. prioQueue.OrderBy(x => x.MinCostToStart ?? double.MaxValue)];

                var node = prioQueue.First();
                prioQueue.Remove(node);

                foreach (var cnn in node.Edges.OrderBy(x => x.Weight))
                {
                    var childNode = cnn.Child;

                    if (childNode.Visited)
                        continue;

                    if (childNode.MinCostToStart == null ||
                        node.MinCostToStart + cnn.Weight < childNode.MinCostToStart && node.MinCostToStart + cnn.Weight < startToEndCost)
                    {
                        childNode.MinCostToStart = node.MinCostToStart + cnn.Weight;
                        childNode.NearestToStart = node;
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }

                node.Visited = true;

                if (node == End)
                    startToEndCost = node.MinCostToStart ?? throw new InvalidOperationException("Cost is null");
            } 
            while (prioQueue.Count > 0);
        }

        private static void BuildShortestPath(LinkedList<(Node, Edge?)> list, Node node)
        {
            if (node.NearestToStart == null)
                return;

            list.AddFirst((node.NearestToStart, node.NearestToStart.Edges.Single(x => x.Child == node)));
            BuildShortestPath(list, node.NearestToStart);
        }
    }

    public record Node(Direction Facing, Cell Cell, List<Edge> Edges)
    {
        public double? MinCostToStart { get; set; }
        public Node? NearestToStart { get; set; }
        public bool Visited { get; set; }
        public double StraightLineDistanceToEnd { get; set; }

        public override string ToString() => $"{{Node [{Cell.Y}, {Cell.X}] facing {Facing}}}";
    }

