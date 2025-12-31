namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public struct PawnTableElement
{
    public ulong Key;

    public BitBoard WhitePassedPawns;
    
    public BitBoard BlackPassedPawns;

    public int PackedScore;

    public void Update(ulong key, int packedScore, ref EvaluationContext evaluationContext)
    {
        Key = key;
        PackedScore = packedScore;
        WhitePassedPawns = evaluationContext.PassedPawns[(int)Side.White];
        BlackPassedPawns = evaluationContext.PassedPawns[(int)Side.Black];
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
