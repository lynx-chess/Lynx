namespace Lynx;

static partial class EvaluationParams
{
    /// <summary>
    /// <see cref="Utils.Pack(-16, -13)"/>
    /// </summary>
    public const int IsolatedPawnPenalty = -851984;

    /// <summary>
    /// <see cref="Utils.Pack(40, 2)"/>
    /// </summary>
    public const int OpenFileRookBonus = 131112;

    /// <summary>
    /// <see cref="Utils.Pack(15, 7)"/>
    /// </summary>
    public const int SemiOpenFileRookBonus = 458767;

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
    /// <see cref="Utils.Pack(30, 71)"/>
    /// </summary>
    public const int BishopPairBonus = 4653086;

    /// <summary>
    /// <see cref="Utils.Pack(12, 16)"/>
    /// </summary>
    public const int PieceProtectedByPawnBonus = 1048588;

    /// <summary>
    /// <see cref="Utils.Pack(-47, -30)"/>
    /// </summary>
    public const int PieceAttackedByPawnPenalty = -1966127;

}
