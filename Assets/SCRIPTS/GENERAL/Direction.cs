public enum Direction { North, South, East, West }

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction d)
    {
        return d switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => Direction.North
        };
    }
}