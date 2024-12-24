namespace Day23;

public record Link(Computer A, Computer B)
{
    public bool LinkIsBetween(Computer a, Computer b) => A == a && B == b || A == b && B == a;

    public Computer? GetOtherEndOfLinkFrom(Computer c)
    {
        return A == c ? B : B == c ? A : null;
    }
}