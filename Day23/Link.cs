namespace Day23;

public record Link(Computer A, Computer B)
{
    public Computer? GetOtherEndOfLinkFrom(Computer c)
    {
        return A == c ? B : B == c ? A : null;
    }
}