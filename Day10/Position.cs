namespace Day10;

public record Position(int Height)
{
    public override string ToString()
    {
        return Height.ToString();
    }
}
