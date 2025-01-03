namespace Day16
{
    internal static class MazeInfo
    {
        internal static readonly Dictionary<Direction, Dictionary<Direction, Move[]>> directionPairs = new()
        {
            { Direction.North,
                new Dictionary<Direction, Move[]> {
                    { Direction.North, [] },
                    { Direction.East, [Move.RotateClockwise] },
                    { Direction.South, [Move.RotateClockwise, Move.RotateClockwise] },
                    { Direction.West, [Move.RotateCounterClockwise] },
                }
            },
            { Direction.East,
                new Dictionary<Direction, Move[]> {
                    { Direction.North, [Move.RotateCounterClockwise] },
                    { Direction.East, [] },
                    { Direction.South, [Move.RotateClockwise] },
                    { Direction.West, [Move.RotateClockwise, Move.RotateClockwise] },
                }
            },
            { Direction.South,
                new Dictionary<Direction, Move[]> {
                    { Direction.North, [Move.RotateClockwise, Move.RotateClockwise] },
                    { Direction.East, [Move.RotateCounterClockwise] },
                    { Direction.South, [] },
                    { Direction.West, [Move.RotateClockwise] },
                }
            },
            { Direction.West,
                new Dictionary<Direction, Move[]> {
                    { Direction.North, [Move.RotateClockwise] },
                    { Direction.East, [Move.RotateClockwise, Move.RotateClockwise] },
                    { Direction.South, [Move.RotateCounterClockwise] },
                    { Direction.West, [] },
                }
            },
        };

        internal static readonly Dictionary<Move, int> costs = new()
        {
            { Move.RotateClockwise, 1000 },
            { Move.RotateCounterClockwise, 1000 },
            { Move.MoveNorth, 1 },
            { Move.MoveEast, 1 },
            { Move.MoveSouth, 1 },
            { Move.MoveWest, 1 },
        };

        internal static readonly List<(Direction direction, int y, int x)> possibleMoves =
        [
            (Direction.North, -1, 0),
            (Direction.East, 0, 1),
            (Direction.South, 1, 0),
            (Direction.West, 0, -1)
        ];
    }
}