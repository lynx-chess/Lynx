namespace Lynx;

static partial class EvaluationParams
{
    /// <summary>
    /// <see cref="Utils.Pack(-16, -14)"/>
    /// </summary>
    public const int IsolatedPawnPenalty = -917520;

    /// <summary>
    /// <see cref="Utils.Pack(40, 2)"/>
    /// </summary>
    public const int OpenFileRookBonus = 131112;

    /// <summary>
    /// <see cref="Utils.Pack(15, 8)"/>
    /// </summary>
    public const int SemiOpenFileRookBonus = 524303;

    /// <summary>
    /// <see cref="Utils.Pack(-24, 5)"/>
    /// </summary>
    public const int SemiOpenFileKingPenalty = 327656;

    /// <summary>
    /// <see cref="Utils.Pack(-65, 2)"/>
    /// </summary>
    public const int OpenFileKingPenalty = 131007;

    /// <summary>
    /// <see cref="Utils.Pack(23, -3)"/>
    /// </summary>
    public const int KingShieldBonus = -196585;

    /// <summary>
    /// <see cref="Utils.Pack(30, 72)"/>
    /// </summary>
    public const int BishopPairBonus = 4718622;

    /// <summary>
    /// <see cref="Utils.Pack(12, 15)"/>
    /// </summary>
    public const int PieceProtectedByPawnBonus = 983052;

    /// <summary>
    /// <see cref="Utils.Pack(-49, -32)"/>
    /// </summary>
    public const int PieceAttackedByPawnPenalty = -2097201;

}
