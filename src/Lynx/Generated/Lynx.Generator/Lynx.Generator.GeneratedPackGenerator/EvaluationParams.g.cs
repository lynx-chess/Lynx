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
    /// <see cref="Utils.Pack(-66, 2)"/>
    /// </summary>
    public const int OpenFileKingPenalty = 131006;

    /// <summary>
    /// <see cref="Utils.Pack(23, -3)"/>
    /// </summary>
    public const int KingShieldBonus = -196585;

    /// <summary>
    /// <see cref="Utils.Pack(29, 72)"/>
    /// </summary>
    public const int BishopPairBonus = 4718621;

    /// <summary>
    /// <see cref="Utils.Pack(37, -1)"/>
    /// </summary>
    public const int KnightOutpostBonus = -65499;

    /// <summary>
    /// <see cref="Utils.Pack(42, 7)"/>
    /// </summary>
    public const int KnightOutpostProtectedBonus = 458794;

    /// <summary>
    /// <see cref="Utils.Pack(11, 15)"/>
    /// </summary>
    public const int PieceProtectedByPawnBonus = 983051;

    /// <summary>
    /// <see cref="Utils.Pack(-44, -31)"/>
    /// </summary>
    public const int PieceAttackedByPawnPenalty = -2031660;

}
