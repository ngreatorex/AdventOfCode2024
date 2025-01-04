namespace Day17;

public class Operation(Opcode opcode, int operand)
{
    public Opcode Opcode => opcode;
    public int Operand => operand;

    public override string ToString()
    {
        return $"[{Opcode},{Operand}]";
    }
}


