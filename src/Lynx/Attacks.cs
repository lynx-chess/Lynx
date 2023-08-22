using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx;

public static partial class Attacks
{
    internal static readonly BitBoard[] _bishopOccupancyMasks =
    {
        18049651735527936,  70506452091904,     275415828992,       1075975168,         38021120,           8657588224,         2216338399232,      567382630219776,
        9024825867763712,   18049651735527424,  70506452221952,     275449643008,       9733406720,         2216342585344,      567382630203392,    1134765260406784,
        4512412933816832,   9024825867633664,   18049651768822272,  70515108615168,     2491752130560,      567383701868544,    1134765256220672,   2269530512441344,
        2256206450263040,   4512412900526080,   9024834391117824,   18051867805491712,  637888545440768,    1135039602493440,   2269529440784384,   4539058881568768,
        1128098963916800,   2256197927833600,   4514594912477184,   9592139778506752,   19184279556981248,  2339762086609920,   4538784537380864,   9077569074761728,
        562958610993152,    1125917221986304,   2814792987328512,   5629586008178688,   11259172008099840,  22518341868716544,  9007336962655232,   18014673925310464,
        2216338399232,      4432676798464,      11064376819712,     22137335185408,     44272556441600,     87995357200384,     35253226045952,     70506452091904,
        567382630219776,    1134765260406784,   2832480465846272,   5667157807464448,   11333774449049600,  22526811443298304,  9024825867763712,   18049651735527936
    };

    internal static readonly BitBoard[] _rookOccupancyMasks =
    {
        282578800148862,        565157600297596,        1130315200595066,       2260630401190006,       4521260802379886,       9042521604759646,       18085043209519166,      36170086419038334,
        282578800180736,        565157600328704,        1130315200625152,       2260630401218048,       4521260802403840,       9042521604775424,       18085043209518592,      36170086419037696,
        282578808340736,        565157608292864,        1130315208328192,       2260630408398848,       4521260808540160,       9042521608822784,       18085043209388032,      36170086418907136,
        282580897300736,        565159647117824,        1130317180306432,       2260632246683648,       4521262379438080,       9042522644946944,       18085043175964672,      36170086385483776,
        283115671060736,        565681586307584,        1130822006735872,       2261102847592448,       4521664529305600,       9042787892731904,       18085034619584512,      36170077829103616,
        420017753620736,        699298018886144,        1260057572672512,       2381576680245248,       4624614895390720,       9110691325681664,       18082844186263552,      36167887395782656,
        35466950888980736,      34905104758997504,      34344362452452352,      33222877839362048,      30979908613181440,      26493970160820224,      17522093256097792,      35607136465616896,
        9079539427579068672,    8935706818303361536,    8792156787827803136,    8505056726876686336,    7930856604974452736,    6782456361169985536,    4485655873561051136,    9115426935197958144,
    };

    /// <summary>
    /// [2 (B|W), 64 (Squares)]
    /// </summary>
    public static BitBoard[,] PawnAttacks = new BitBoard[2, 64]
    {
        {
            512,                    1280,                   2560,                   5120,                   10240,                  20480,                  40960,                  16384,
            131072,                 327680,                 655360,                 1310720,                2621440,                5242880,                10485760,               4194304,
            33554432,               83886080,               167772160,              335544320,              671088640,              1342177280,             2684354560,             1073741824,
            8589934592,             21474836480,            42949672960,            85899345920,            171798691840,           343597383680,           687194767360,           274877906944,
            2199023255552,          5497558138880,          10995116277760,         21990232555520,         43980465111040,         87960930222080,         175921860444160,        70368744177664,
            562949953421312,        1407374883553280,       2814749767106560,       5629499534213120,       11258999068426240,      22517998136852480,      45035996273704960,      18014398509481984,
            144115188075855872,     360287970189639680,     720575940379279360,     1441151880758558720,    2882303761517117440,    5764607523034234880,    11529215046068469760,   4611686018427387904,
            0,                      0,                      0,                      0,                      0,                      0,                      0,                      0
        },
        {
            0,                  0,                  0,                  0,                  0,                  0,                  0,                  0,
            2,                  5,                  10,                 20,                 40,                 80,                 160,                64,
            512,                1280,               2560,               5120,               10240,              20480,              40960,              16384,
            131072,             327680,             655360,             1310720,            2621440,            5242880,            10485760,           4194304,
            33554432,           83886080,           167772160,          335544320,          671088640,          1342177280,         2684354560,         1073741824,
            8589934592,         21474836480,        42949672960,        85899345920,        171798691840,       343597383680,       687194767360,       274877906944,
            2199023255552,      5497558138880,      10995116277760,     21990232555520,     43980465111040,     87960930222080,     175921860444160,    70368744177664,
            562949953421312,    1407374883553280,   2814749767106560,   5629499534213120,   11258999068426240,  22517998136852480,  45035996273704960,  18014398509481984
        },
    };

    public static readonly BitBoard[] KnightAttacks =
    {
        132096,                 329728,                 659712,                 1319424,                2638848,                5277696,                10489856,               4202496,
        33816580,               84410376,               168886289,              337772578,              675545156,              1351090312,             2685403152,             1075839008,
        8657044482,             21609056261,            43234889994,            86469779988,            172939559976,           345879119952,           687463207072,           275414786112,
        2216203387392,          5531918402816,          11068131838464,         22136263676928,         44272527353856,         88545054707712,         175990581010432,        70506185244672,
        567348067172352,        1416171111120896,       2833441750646784,       5666883501293568,       11333767002587136,      22667534005174272,      45053588738670592,      18049583422636032,
        145241105196122112,     362539804446949376,     725361088165576704,     1450722176331153408,    2901444352662306816,    5802888705324613632,    11533718717099671552,   4620693356194824192,
        288234782788157440,     576469569871282176,     1224997833292120064,    2449995666584240128,    4899991333168480256,    9799982666336960512,    1152939783987658752,    2305878468463689728,
        1128098930098176,       2257297371824128,       4796069720358912,       9592139440717824,       19184278881435648,      38368557762871296,      4679521487814656,       9077567998918656
    };

    public static readonly BitBoard[] KingAttacks =
    {
        770,                    1797,                   3594,                   7188,                   14376,                  28752,                  57504,                  49216,
        197123,                 460039,                 920078,                 1840156,                3680312,                7360624,                14721248,               12599488,
        50463488,               117769984,              235539968,              471079936,              942159872,              1884319744,             3768639488,             3225468928,
        12918652928,            30149115904,            60298231808,            120596463616,           241192927232,           482385854464,           964771708928,           825720045568,
        3307175149568,          7718173671424,          15436347342848,         30872694685696,         61745389371392,         123490778742784,        246981557485568,        211384331665408,
        846636838289408,        1975852459884544,       3951704919769088,       7903409839538176,       15806819679076352,      31613639358152704,      63227278716305408,      54114388906344448,
        216739030602088448,     505818229730443264,     1011636459460886528,    2023272918921773056,    4046545837843546112,    8093091675687092224,    16186183351374184448,   13853283560024178688,
        144959613005987840,     362258295026614272,     724516590053228544,     1449033180106457088,    2898066360212914176,    5796132720425828352,    11592265440851656704,   4665729213955833856
    };

    /// <summary>
    /// Get Bishop attacks assuming current board occupancy
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard BishopAttacks(int squareIndex, BitBoard occupancy)
    {
        var occ = occupancy & _bishopOccupancyMasks[squareIndex];
        occ *= Constants.BishopMagicNumbers[squareIndex];
        occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

        return _bishopAttacks[squareIndex, occ];
    }

    /// <summary>
    /// Get Rook attacks assuming current board occupancy
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard RookAttacks(int squareIndex, BitBoard occupancy)
    {
        var occ = occupancy & _rookOccupancyMasks[squareIndex];
        occ *= Constants.RookMagicNumbers[squareIndex];
        occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

        return _rookAttacks[squareIndex, occ];
    }

    /// <summary>
    /// Get Queen attacks assuming current board occupancy
    /// Use <see cref="QueenAttacks(BitBoard, BitBoard)"/> if rook and bishop attacks are already calculated
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard QueenAttacks(int squareIndex, BitBoard occupancy)
    {
        return QueenAttacks(
            RookAttacks(squareIndex, occupancy),
            BishopAttacks(squareIndex, occupancy));
    }

    /// <summary>
    /// Get Queen attacks having rook and bishop attacks pre-calculated
    /// </summary>
    /// <param name="rookAttacks"></param>
    /// <param name="bishopAttacks"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard QueenAttacks(BitBoard rookAttacks, BitBoard bishopAttacks)
    {
        return rookAttacks | bishopAttacks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquaredAttackedBySide(int squaredIndex, Position position, Side sideToMove) =>
        IsSquaredAttacked(squaredIndex, sideToMove, position.PieceBitBoards, position.OccupancyBitBoards);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquaredAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);

        // I tried to order them from most to least likely
        return
            IsSquareAttackedByPawns(squareIndex, sideToMoveInt, offset, piecePosition)
            || IsSquareAttackedByKing(squareIndex, offset, piecePosition)
            || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
            || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
            || IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
            || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareInCheck(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);

        // I tried to order them from most to least likely
        return
            IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
            || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
            || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
            || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
            || IsSquareAttackedByPawns(squareIndex, sideToMoveInt, offset, piecePosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByPawns(int squareIndex, int sideToMove, int offset, BitBoard[] pieces)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (PawnAttacks[oppositeColorIndex, squareIndex] & pieces[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
    {
        return (KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
    {
        return (KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard bishopAttacks)
    {
        bishopAttacks = BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
        return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard rookAttacks)
    {
        rookAttacks = RookAttacks(squareIndex, occupancy[(int)Side.Both]);
        return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard[] piecePosition)
    {
        var queenAttacks = QueenAttacks(rookAttacks, bishopAttacks);
        return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
    }
}
