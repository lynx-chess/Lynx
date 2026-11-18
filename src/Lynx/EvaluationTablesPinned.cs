using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

/// <summary>
/// Pinned copies of the tables that <see cref="Position.StaticEvaluation"/> consumes, exposed as pointers.
/// This converts the address in a JIT constant, baked directly into the compiled code (JIT knows both the index and the length), avoiding bound checks
/// </summary>
public static unsafe class EvaluationTablesPinned
{
    private static readonly List<Array> _keepAlive = [];

    public static readonly int* PieceProtectedByPawnBonus = CopyToPinnedArray(EvaluationParams.PieceProtectedByPawnBonus);
    public static readonly int* TotalKingRingAttacksBonus = CopyToPinnedArray(EvaluationParams.TotalKingRingAttacksBonus);
    public static readonly int* PawnIslandsBonus = CopyToPinnedArray(EvaluationParams.PawnIslandsBonus);
    public static readonly int* KingMobilityBonus = CopyToPinnedArray(EvaluationParams.KingMobilityBonus);
    public static readonly int* VirtualKingMobilityBonus = CopyToPinnedArray(EvaluationParams.VirtualKingMobilityBonus);
    public static readonly int* UnsafeCheckBonus = CopyToPinnedArray(EvaluationParams.UnsafeCheckBonus);
    public static readonly int* SafeCheckBonus = CopyToPinnedArray(EvaluationParams.SafeCheckBonus);
    public static readonly int* RookMobilityBonus = CopyToPinnedArray(EvaluationParams.RookMobilityBonus);
    public static readonly int* QueenMobilityBonus = CopyToPinnedArray(EvaluationParams.QueenMobilityBonus);
    public static readonly int* PawnPhalanxBonus = CopyToPinnedArray(EvaluationParams.PawnPhalanxBonus);
    public static readonly int* PassedPawnPushBonus = CopyToPinnedArray(EvaluationParams.PassedPawnPushBonus);
    public static readonly int* KnightMobilityBonus = CopyToPinnedArray(EvaluationParams.KnightMobilityBonus);
    public static readonly int* IsolatedPawnPenalty = CopyToPinnedArray(EvaluationParams.IsolatedPawnPenalty);
    public static readonly int* FriendlyKingDistanceToPassedPawnBonus = CopyToPinnedArray(EvaluationParams.FriendlyKingDistanceToPassedPawnBonus);
    public static readonly int* EnemyKingDistanceToPassedPawnPenalty = CopyToPinnedArray(EvaluationParams.EnemyKingDistanceToPassedPawnPenalty);
    public static readonly int* DoubledPawnPenalty = CopyToPinnedArray(EvaluationParams.DoubledPawnPenalty);
    public static readonly int* ConnectedRooksBonus = CopyToPinnedArray(EvaluationParams.ConnectedRooksBonus);
    public static readonly int* BishopMobilityBonus = CopyToPinnedArray(EvaluationParams.BishopMobilityBonus);
    public static readonly int* BadBishop_SameColorPawnsPenalty = CopyToPinnedArray(EvaluationParams.BadBishop_SameColorPawnsPenalty);
    public static readonly int* BadBishop_BlockedCentralPawnsPenalty = CopyToPinnedArray(EvaluationParams.BadBishop_BlockedCentralPawnsPenalty);
    public static readonly int* BackwardsPawnPenalty = CopyToPinnedArray(EvaluationParams.BackwardsPawnPenalty);
    public static readonly int* SemiOpenFileRookEnemyBonus = FlattenToPinnedArray(EvaluationParams.SemiOpenFileRookEnemyBonus, stride: 8);
    public static readonly int* SemiOpenFileRookBonus = FlattenToPinnedArray(EvaluationParams.SemiOpenFileRookBonus, stride: 8);
    public static readonly int* SemiOpenFileKingPenalty = FlattenToPinnedArray(EvaluationParams.SemiOpenFileKingPenalty, stride: 8);
    public static readonly int* PassedPawnNoEnemiesAheadEnemyBonus = FlattenToPinnedArray(EvaluationParams.PassedPawnNoEnemiesAheadEnemyBonus, stride: 8);
    public static readonly int* PassedPawnNoEnemiesAheadBonus = FlattenToPinnedArray(EvaluationParams.PassedPawnNoEnemiesAheadBonus, stride: 8);
    public static readonly int* PassedPawnEnemyBonus = FlattenToPinnedArray(EvaluationParams.PassedPawnEnemyBonus, stride: 8);
    public static readonly int* PassedPawnBonus = FlattenToPinnedArray(EvaluationParams.PassedPawnBonus, stride: 8);
    public static readonly int* OpenFileRookEnemyBonus = FlattenToPinnedArray(EvaluationParams.OpenFileRookEnemyBonus, stride: 8);
    public static readonly int* OpenFileRookBonus = FlattenToPinnedArray(EvaluationParams.OpenFileRookBonus, stride: 8);
    public static readonly int* OpenFileKingPenalty = FlattenToPinnedArray(EvaluationParams.OpenFileKingPenalty, stride: 8);
    public static readonly int* DefendedThreatsBonus = FlattenToPinnedArray([[], EvaluationParams.KnightThreatsBonus_Defended, EvaluationParams.BishopThreatsBonus_Defended, EvaluationParams.RookThreatsBonus_Defended, EvaluationParams.QueenThreatsBonus_Defended, EvaluationParams.KingThreatsBonus_Defended], stride: 8);
    public static readonly int* UndefendedThreatsBonus = FlattenToPinnedArray([[], EvaluationParams.KnightThreatsBonus, EvaluationParams.BishopThreatsBonus, EvaluationParams.RookThreatsBonus, EvaluationParams.QueenThreatsBonus, EvaluationParams.KingThreatsBonus], stride: 8);

    public static readonly ulong* KingRing = CopyToPinnedArray(EvaluationConstants.KingRing);

    public static readonly ulong* KingAttacks = CopyToPinnedArray(Attacks.KingAttacks);
    public static readonly ulong* KnightAttacks = CopyToPinnedArray(Attacks.KnightAttacks);

    public static readonly ulong* WhitePassedPawnMasks = CopyToPinnedArray(Masks.WhitePassedPawnMasks);
    public static readonly ulong* BlackPassedPawnMasks = CopyToPinnedArray(Masks.BlackPassedPawnMasks);
    public static readonly ulong* RankMasks = CopyToPinnedArray(Masks.RankMasks);
    public static readonly ulong* IsolatedPawnMasks = CopyToPinnedArray(Masks.IsolatedPawnMasks);

    public static readonly int* ChebyshevDistance = FlattenToPinnedArray(Constants.ChebyshevDistance, stride: 64);

    private static T* CopyToPinnedArray<T>(T[] source)
        where T : unmanaged
    {
        var pinned = GC.AllocateUninitializedArray<T>(source.Length, pinned: true);

        source.CopyTo(pinned, 0);
        _keepAlive.Add(pinned);

        return (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(pinned));
    }

    private static T* FlattenToPinnedArray<T>(T[][] source, int stride)
        where T : unmanaged
    {
        var pinned = GC.AllocateArray<T>(source.Length * stride, pinned: true);
        for (int i = 0; i < source.Length; ++i)
        {
            if (source[i].Length > stride)
            {
                throw new LynxException($"A larger {nameof(stride)} is needed, since inner array length ({source[i].Length}) is greater than the specified one ({stride})");
            }

            source[i].CopyTo(pinned, i * stride);
        }

        _keepAlive.Add(pinned);

        return (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(pinned));
    }
}
