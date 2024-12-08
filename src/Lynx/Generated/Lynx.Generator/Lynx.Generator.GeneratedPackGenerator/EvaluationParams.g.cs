namespace Lynx;

static partial class EvaluationParams
{
    /// <summary>
    /// <see cref="Utils.Pack(-17, -13)"/>
    /// </summary>
    public const int IsolatedPawnPenalty = -851985;

    /// <summary>
    /// <see cref="Utils.Pack(40, 0)"/>
    /// </summary>
    public const int OpenFileRookBonus = 40;

    /// <summary>
    /// <see cref="Utils.Pack(16, 6)"/>
    /// </summary>
    public const int SemiOpenFileRookBonus = 393232;

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
    /// <see cref="Utils.Pack(-47, -31)"/>
    /// </summary>
    public const int PieceAttackedByPawnPenalty = -2031663;

}
