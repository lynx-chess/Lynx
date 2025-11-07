using Lynx.Model;
using System.Collections.Frozen;

namespace Lynx;

#pragma warning disable IDE0055 // Discard formatting in this region

public static class Constants
{
    /// <summary>
    /// 0xFFFFFFFFFFFFFFFFUL
    /// 8   1 1 1 1 1 1 1 1
    /// 7   1 1 1 1 1 1 1 1
    /// 6   1 1 1 1 1 1 1 1
    /// 5   1 1 1 1 1 1 1 1
    /// 4   1 1 1 1 1 1 1 1
    /// 3   1 1 1 1 1 1 1 1
    /// 2   1 1 1 1 1 1 1 1
    /// 1   1 1 1 1 1 1 1 1
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard FullBoard = ulong.MaxValue;

    /// <summary>
    /// 8   1 0 0 0 0 0 0 0
    /// 7   1 0 0 0 0 0 0 0
    /// 6   1 0 0 0 0 0 0 0
    /// 5   1 0 0 0 0 0 0 0
    /// 4   1 0 0 0 0 0 0 0
    /// 3   1 0 0 0 0 0 0 0
    /// 2   1 0 0 0 0 0 0 0
    /// 1   1 0 0 0 0 0 0 0
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard AFile = 0x101010101010101;

    /// <summary>
    /// 8   0 1 1 1 1 1 1 1
    /// 7   0 1 1 1 1 1 1 1
    /// 6   0 1 1 1 1 1 1 1
    /// 5   0 1 1 1 1 1 1 1
    /// 4   0 1 1 1 1 1 1 1
    /// 3   0 1 1 1 1 1 1 1
    /// 2   0 1 1 1 1 1 1 1
    /// 1   0 1 1 1 1 1 1 1
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard NotAFile = 0xFEFEFEFEFEFEFEFE;

    /// <summary>
    /// 8   0 0 0 0 0 0 0 1
    /// 7   0 0 0 0 0 0 0 1
    /// 6   0 0 0 0 0 0 0 1
    /// 5   0 0 0 0 0 0 0 1
    /// 4   0 0 0 0 0 0 0 1
    /// 3   0 0 0 0 0 0 0 1
    /// 2   0 0 0 0 0 0 0 1
    /// 1   0 0 0 0 0 0 0 1
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard HFile = 0x8080808080808080;

    /// <summary>
    /// 8   1 1 1 1 1 1 1 0
    /// 7   1 1 1 1 1 1 1 0
    /// 6   1 1 1 1 1 1 1 0
    /// 5   1 1 1 1 1 1 1 0
    /// 4   1 1 1 1 1 1 1 0
    /// 3   1 1 1 1 1 1 1 0
    /// 2   1 1 1 1 1 1 1 0
    /// 1   1 1 1 1 1 1 1 0
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard NotHFile = 0x7F7F7F7F7F7F7F7F;

    /// <summary>
    /// 8   1 1 1 1 1 1 0 0
    /// 7   1 1 1 1 1 1 0 0
    /// 6   1 1 1 1 1 1 0 0
    /// 5   1 1 1 1 1 1 0 0
    /// 4   1 1 1 1 1 1 0 0
    /// 3   1 1 1 1 1 1 0 0
    /// 2   1 1 1 1 1 1 0 0
    /// 1   1 1 1 1 1 1 0 0
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard NotHGFiles = 0x3F3F3F3F3F3F3F3F;

    /// <summary>
    /// 8   0 0 1 1 1 1 1 1
    /// 7   0 0 1 1 1 1 1 1
    /// 6   0 0 1 1 1 1 1 1
    /// 5   0 0 1 1 1 1 1 1
    /// 4   0 0 1 1 1 1 1 1
    /// 3   0 0 1 1 1 1 1 1
    /// 2   0 0 1 1 1 1 1 1
    /// 1   0 0 1 1 1 1 1 1
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard NotABFiles = 0xFCFCFCFCFCFCFCFC;

    public const BitBoard Corners = 0x8100000000000081;

    public static readonly string[] Coordinates =
    [
        "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",
        "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
        "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
        "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
        "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
        "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
        "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
        "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1"
    ];

    public static readonly char[][] CoordinatesCharArray =
    [
        ['a','8'], ['b','8'], ['c','8'], ['d','8'], ['e','8'], ['f','8'], ['g','8'], ['h','8'],
        ['a','7'], ['b','7'], ['c','7'], ['d','7'], ['e','7'], ['f','7'], ['g','7'], ['h','7'],
        ['a','6'], ['b','6'], ['c','6'], ['d','6'], ['e','6'], ['f','6'], ['g','6'], ['h','6'],
        ['a','5'], ['b','5'], ['c','5'], ['d','5'], ['e','5'], ['f','5'], ['g','5'], ['h','5'],
        ['a','4'], ['b','4'], ['c','4'], ['d','4'], ['e','4'], ['f','4'], ['g','4'], ['h','4'],
        ['a','3'], ['b','3'], ['c','3'], ['d','3'], ['e','3'], ['f','3'], ['g','3'], ['h','3'],
        ['a','2'], ['b','2'], ['c','2'], ['d','2'], ['e','2'], ['f','2'], ['g','2'], ['h','2'],
        ['a','1'], ['b','1'], ['c','1'], ['d','1'], ['e','1'], ['f','1'], ['g','1'], ['h','1']
    ];

    /// <summary>
    /// Following <see cref="Piece"/> order
    /// </summary>
    public const string AsciiPieces = "PNBRQKpnbrqk";

    /// <summary>
    /// Following <see cref="Piece"/> order
    /// </summary>
    public const string AsciiPiecesLowercase = "pnbrqkpnbrqk";

    /// <summary>
    /// Following <see cref="Piece"/> order.
    /// Don't work stright away in Windows
    /// </summary>
    public static readonly string[] UnicodePieces =
    [
        "♙", "♘", "♗", "♖", "♕", "♔",   // White
        "♟︎", "♞", "♝", "♜", "♛", "♚",   // Black
        "-"
    ];

    /// <summary>
    /// Covert ASCII character pieces to encoded constants
    /// </summary>
#pragma warning disable S2386,S3887 // Mutable fields should not be "public static"
    public static readonly FrozenDictionary<char, Piece> PiecesByChar = new Dictionary<char, Piece>(12)
#pragma warning restore S2386,S3887 // Mutable fields should not be "public static"
    {
        ['P'] = Piece.P,
        ['N'] = Piece.N,
        ['B'] = Piece.B,
        ['R'] = Piece.R,
        ['Q'] = Piece.Q,
        ['K'] = Piece.K,

        ['p'] = Piece.p,
        ['n'] = Piece.n,
        ['b'] = Piece.b,
        ['r'] = Piece.r,
        ['q'] = Piece.q,
        ['k'] = Piece.k,
    }.ToFrozenDictionary();

    /// <summary>
    /// Relevant bishop occupancy bit count per square
    /// </summary>
    public static ReadOnlySpan<int> BishopRelevantOccupancyBits =>
    [
        6, 5, 5, 5, 5, 5, 5, 6,
        5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 7, 7, 7, 7, 5, 5,
        5, 5, 7, 9, 9, 7, 5, 5,
        5, 5, 7, 9, 9, 7, 5, 5,
        5, 5, 7, 7, 7, 7, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5,
        6, 5, 5, 5, 5, 5, 5, 6,
    ];

    /// <summary>
    /// Relevant rook occupancy bit count per square
    /// </summary>
    public static ReadOnlySpan<int> RookRelevantOccupancyBits =>
    [
        12, 11, 11, 11, 11, 11, 11, 12,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        12, 11, 11, 11, 11, 11, 11, 12
    ];

    public static ReadOnlySpan<BitBoard> RookMagicNumbers =>
    [
        0x8080104000208000,   0xa240004020021004,   0x880200080081000,    0x2080058010010800,   0x1001001020408008,   0x4200011042000884,   0x6001c020002c805,    0x2000100802a0044,
        0x20800020804000,     0x5000c00020005000,   0x801000802002,       0x8005001001002208,   0x2000412000820,      0x23000218140100,     0x100a002600081401,   0x2520800060800100,
        0x2081050020800840,   0x4001010040002080,   0x848020001000,       0x8090008008001481,   0x8801010004100800,   0x4004040020100,      0x4004040042080110,   0xc008120000806411,
        0x830802880004000,    0x10004240012000,     0x10001080200088,     0x100100100200900,    0x6102002200080410,   0x4102a008010410c0,   0x2020400100801,      0x2000044a00010394,
        0x8040002040800080,   0x4040100024200800,   0x108881000802002,    0x600080080801000,    0x308008008800400,    0x2002000802000410,   0x8800200800100,      0x8084442492000143,
        0x8220400080008020,   0x402010084004,       0xa01001020010044,    0xc808080010008080,   0x802800800400800a,   0x1000204010008,      0x102020188040010,    0x280040080420001,
        0x121002080104100,    0x8140401020008080,   0x50040020080020,     0x800800100480,       0x110008000500,       0x8c000200410040,     0x8000881002018400,   0x88a00044b1040600,
        0x8096800102104025,   0x8004204204110082,   0xd042200011000843,   0x422208c0900f001,    0x61001002080005,     0x4002001028218422,   0x2040104088020104,   0x300108b104064082,
    ];

    public static ReadOnlySpan<BitBoard> BishopMagicNumbers =>
    [
        0x10440804a02200,     0x2002241404004050,   0x808008c10800204,    0x8204848201002,      0x8411104001420404,   0x422080208050010,    0x8002020120880e44,   0x3084800880800,
        0x1904210042080,      0x1004103018888888,   0x20264802404a0000,   0x21042402800002,     0x8400840420400002,   0x4102042220901800,   0x840004108201004,    0x40008404010501,
        0x4500850100209,      0x2084101110021042,   0x8004004089001101,   0x2005040104200,      0x2860829400a00800,   0x202c10080800,       0xa00104012088,       0x1000800a0a510844,
        0x4024510120021010,   0x8010081602020c24,   0x10410408080300,     0x42880204a020200,    0x8840080802000,      0xa010002081040100,   0x142091000a080200,   0xc8a0000222200,
        0x41041000401022,     0xca80822082080808,   0x1004104282800,      0x1002008020020200,   0x408020020200,       0x102208101020044,    0x18812800a0010420,   0x8004084200205110,
        0x40020160a0820808,   0x8882402009000,      0xc3000c0048000400,   0x3000a018001100,     0x2003020202010410,   0x20a0381000408020,   0x9010010040011c,     0x2230032202205182,
        0x2044008c30084820,   0x900848090311020,    0x400a244404040102,   0x4120300820880140,   0x2020004084884400,   0x47059850010820,     0xa90042128020100,    0x2011100892808000,
        0x120802118202420,    0x1224c0808401a005,   0x88004210841102,     0xe00000000840420,    0x900001441904110,    0x480040285025c201,   0x840080801082200,    0x2110810800948200,
    ];

    public const string InitialPositionFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    /// <summary>
    /// Kiwipete
    /// </summary>
    public const string TrickyTestPositionFEN = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
    public const string TrickyTestPositionReversedFEN = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1";

    public const string KillerTestPositionFEN = "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1";
    public const string CmkTestPositionFEN = "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9";

    public const string ComplexPositionFEN = "rq2k2r/ppp2pb1/2n1pnpp/1Q1p1b2/3P1B2/2N1PNP1/PPP2PBP/R3K2R w KQkq - 0 1";

    // 'Search explosion'
    // https://www.chessprogramming.org/Lasker-Reichhelm_Position
    public const string TTPositionFEN = "8/k7/3p4/p2P1p2/P2P1P2/8/8/K7 w - - 0 1";

    public const string EmptyBoardFEN = "8/8/8/8/8/8/8/8 w - - 0 1";

    public const string CaptureTrainPositionFEN = "r2q1rk1/bppb1pp1/p2p2np/2PPp3/1P2P1n1/P3BN2/2Q1BPPP/RN3RK1 w - - 2 15";

    public const int InitialWhiteKingSquare = (int)BoardSquare.e1;
    public const int InitialBlackKingSquare = (int)BoardSquare.e8;

    public const int InitialWhiteKingsideRookSquare = (int)BoardSquare.h1;
    public const int InitialWhiteQueensideRookSquare = (int)BoardSquare.a1;
    public const int InitialBlackKingsideRookSquare = (int)BoardSquare.h8;
    public const int InitialBlackQueensideRookSquare = (int)BoardSquare.a8;

    public const int WhiteKingShortCastleSquare = (int)BoardSquare.g1;
    public const int BlackKingShortCastleSquare = (int)BoardSquare.g8;
    public const int WhiteKingLongCastleSquare = (int)BoardSquare.c1;
    public const int BlackKingLongCastleSquare = (int)BoardSquare.c8;

    public const int WhiteRookShortCastleSquare = (int)BoardSquare.f1;
    public const int BlackRookShortCastleSquare = (int)BoardSquare.f8;
    public const int WhiteRookLongCastleSquare = (int)BoardSquare.d1;
    public const int BlackRookLongCastleSquare = (int)BoardSquare.d8;

    public const string WhiteShortCastle = "e1g1";
    public const string BlackShortCastle = "e8g8";
    public const string WhiteLongCastle = "e1c1";
    public const string BlackLongCastle = "e8c8";

    public static ReadOnlySpan<int> EnPassantCaptureSquares =>
    [
        0, 0, 0, 0, 0, 0, 0, 0,     //  0-7
        0, 0, 0, 0, 0, 0, 0, 0,     //  8-15

        (int)BoardSquare.a6 + 8,    // 16 = a6
        (int)BoardSquare.b6 + 8,
        (int)BoardSquare.c6 + 8,
        (int)BoardSquare.d6 + 8,
        (int)BoardSquare.e6 + 8,
        (int)BoardSquare.f6 + 8,
        (int)BoardSquare.g6 + 8,
        (int)BoardSquare.h6 + 8,    // 23 = h6

        0, 0, 0, 0, 0, 0, 0, 0,     // 24-31
        0, 0, 0, 0, 0, 0, 0, 0,     // 32-39

        (int)BoardSquare.a3 - 8,    // 40 = a3
        (int)BoardSquare.b3 - 8,
        (int)BoardSquare.c3 - 8,
        (int)BoardSquare.d3 - 8,
        (int)BoardSquare.e3 - 8,
        (int)BoardSquare.f3 - 8,
        (int)BoardSquare.g3 - 8,
        (int)BoardSquare.h3 - 8     //47 = h3
    ];

#pragma warning disable RCS1257 // Use enum field explicitly
    public const byte NoUpdateCastlingRight = 15;
    public const byte WhiteKingCastlingRight = 12;
    public const byte WhiteKingSideRookCastlingRight = 14;
    public const byte WhiteQueenSideRookCastlingRight = 13;
    public const byte BlackKingCastlingRight = 3;
    public const byte BlackKingSideRookCastlingRight = 11;
    public const byte BlackQueenSideRookCastlingRight = 7;

    /// <summary>
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/make_move_castling_rights/bbc.c#L1474
    ///                                             CastlingRights  Binary  Decimal
    ///  K & R didn't move                          1111 & 1111  =  1111    15
    ///  White King moved                           1111 & 1100  =  1100    12
    ///  White kingside Rook moved or captured      1111 & 1110  =  1110    14
    ///  White queenside Rook moved or captured     1111 & 1101  =  1101    13
    ///  Black King moved                           1111 & 0011  =  1011    3
    ///  Black kingside Rook moved or captured      1111 & 1011  =  1011    11
    ///  Black queenside Rook moved or captured     1111 & 0111  =  0111    7
    /// </summary>
    [Obsolete("Test only")]
    public static ReadOnlySpan<byte> CastlingRightsUpdateConstants =>
    [
         7, 15, 15, 15,  3, 15, 15, 11,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        13, 15, 15, 15, 12, 15, 15, 14
    ];

    public static readonly int MaxTTArrayLength = 258_111;

    public const int AbsoluteMaxTTSize = 524_288; // 4TB RAM, not bad

    public const int AbsoluteMinTTSize = 1;

    public static readonly int MaxThreadCount = Array.MaxLength + 1;

    public static readonly int MaxMoveOverhead = 60_000;

    /// <summary>
    /// 218 or 224 seems to be the known limit of legal moves
    /// https://www.reddit.com/r/chess/comments/9j70dc/position_with_the_most_number_of_legal_moves/
    /// We generally need to account for a number higher than that due to pseudolegal movegen
    /// Regardless, we want to support positions like kBQQQQQQ/BR5Q/Q6Q/Q6Q/Q6Q/Q6Q/Q6Q/KQQQQQQQ w - - 0 1 (270 pseudolegal and legal moves)
    /// </summary>
    public const int MaxNumberOfPseudolegalMovesInAPosition = 512;

    public const int MaxNumberMovesInAGame = 2048;

    public static readonly int SideLimit = Enum.GetValues<Piece>().Length / 2;

    public static ReadOnlySpan<int> Rank =>
    [
        7, 7, 7, 7, 7, 7, 7, 7,
        6, 6, 6, 6, 6, 6, 6, 6,
        5, 5, 5, 5, 5, 5, 5, 5,
        4, 4, 4, 4, 4, 4, 4, 4,
        3, 3, 3, 3, 3, 3, 3, 3,
        2, 2, 2, 2, 2, 2, 2, 2,
        1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0
    ];

    public static ReadOnlySpan<int> File =>
    [
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7
    ];

    public static ReadOnlySpan<byte> ByteFile =>
    [
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7
    ];

    public static ReadOnlySpan<int> LightSquares =>
    [
        1, 0, 1, 0, 1, 0, 1, 0,
        0, 1, 0, 1, 0, 1, 0, 1,
        1, 0, 1, 0, 1, 0, 1, 0,
        0, 1, 0, 1, 0, 1, 0, 1,
        1, 0, 1, 0, 1, 0, 1, 0,
        0, 1, 0, 1, 0, 1, 0, 1,
        1, 0, 1, 0, 1, 0, 1, 0,
        0, 1, 0, 1, 0, 1, 0, 1,
    ];

    public static ReadOnlySpan<int> DarkSquares =>
    [
        0, 1, 0, 1, 0, 1, 0, 1,
        1, 0, 1, 0, 1, 0, 1, 0,
        0, 1, 0, 1, 0, 1, 0, 1,
        1, 0, 1, 0, 1, 0, 1, 0,
        0, 1, 0, 1, 0, 1, 0, 1,
        1, 0, 1, 0, 1, 0, 1, 0,
        0, 1, 0, 1, 0, 1, 0, 1,
        1, 0, 1, 0, 1, 0, 1, 0,
    ];

    /// <summary>
    /// 8   1 0 1 0 1 0 1 0
    /// 7   0 1 0 1 0 1 0 1
    /// 6   1 0 1 0 1 0 1 0
    /// 5   0 1 0 1 0 1 0 1
    /// 4   1 0 1 0 1 0 1 0
    /// 3   0 1 0 1 0 1 0 1
    /// 2   1 0 1 0 1 0 1 0
    /// 1   0 1 0 1 0 1 0 1
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard LightSquaresBitBoard = 0xAA55AA55AA55AA55UL;

    /// <summary>
    /// 8   0 1 0 1 0 1 0 1
    /// 7   1 0 1 0 1 0 1 0
    /// 6   0 1 0 1 0 1 0 1
    /// 5   1 0 1 0 1 0 1 0
    /// 4   0 1 0 1 0 1 0 1
    /// 3   1 0 1 0 1 0 1 0
    /// 2   0 1 0 1 0 1 0 1
    /// 1   1 0 1 0 1 0 1 0
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard DarkSquaresBitBoard = 0x55AA55AA55AA55AAUL;

    /// <summary>
    /// 8   0 0 1 1 1 1 0 0
    /// 7   0 0 1 1 1 1 0 0
    /// 6   0 0 1 1 1 1 0 0
    /// 5   0 0 1 1 1 1 0 0
    /// 4   0 0 1 1 1 1 0 0
    /// 3   0 0 1 1 1 1 0 0
    /// 2   0 0 1 1 1 1 0 0
    /// 1   0 0 1 1 1 1 0 0
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard CentralFiles = 0x3c3c3c3c3c3c3c3c;

    /// <summary>
    /// e4, d4, e5, d5
    /// </summary>
    public const BitBoard CentralSquares = 0x1818000000;

    /// <summary>
    /// 8   0 0 0 0 0 0 0 0
    /// 7   0 1 1 1 1 1 1 0
    /// 6   0 1 1 1 1 1 1 0
    /// 5   0 1 1 1 1 1 1 0
    /// 4   0 1 1 1 1 1 1 0
    /// 3   0 1 1 1 1 1 1 0
    /// 2   0 1 1 1 1 1 1 0
    /// 1   0 0 0 0 0 0 0 0
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard NotAorH = 0x7e7e7e7e7e7e00;

    /// <summary>
    /// 8   0 0 0 0 0 0 0 0
    /// 7   1 1 1 1 1 1 1 1
    /// 6   1 1 1 1 1 1 1 1
    /// 5   1 1 1 1 1 1 1 1
    /// 4   1 1 1 1 1 1 1 1
    /// 3   1 1 1 1 1 1 1 1
    /// 2   1 1 1 1 1 1 1 1
    /// 1   0 0 0 0 0 0 0 0
    ///     a b c d e f g h
    /// </summary>
    public const BitBoard PawnSquares = 0xffffffffffff00;

    public static ReadOnlySpan<char> FileString => [ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' ];

    public const int AbsoluteMaxDepth = 255;

    /// <summary>
    /// To take into account array access by depth and depth extensions
    /// </summary>
    public const int ArrayDepthMargin = 16;

    /// <summary>
    /// Given mate in <see cref="MateDistanceMarginToStopSearching"/> (so this is full moves)
    /// </summary>
    public const int MateDistanceMarginToStopSearching = 2;

    public const int MaxMateDistanceToStopSearching = 100 -  (2 * MateDistanceMarginToStopSearching);

    /// <summary>
    /// From https://lichess.org/RViT3UWL
    /// Failes input parsing fail if default input buffer size is used (see Lynx.Cli.Listener
    /// </summary>
    public const string LongPositionCommand = "position startpos moves d2d4 g8f6 g1f3 d7d5 b1c3 e7e6 g2g3 c7c5 e2e3 c5c4 f1g2 e6e5 d4e5 f6e4 d1d5 e4c3 d5d8 e8d8 b2c3 b8c6 c1b2 a7a6 e1c1 d8c7 d1e1 a6a5 e3e4 a5a4 a2a3 b7b6 f3d2 b6b5 f2f4 h8g8 d2f3 c8g4 f3g5 f8c5 g5h7 g8h8 h7g5 f7f6 e5f6 g7f6 g5f3 a8b8 f3d4 g4d7 h2h4 h8g8 e1e3 b5b4 a3b4 a4a3 b2a3 c6d4 b4c5 g8g3 e3g3 d4e2 c1d2 e2g3 h1e1 d7c6 a3b4 b8h8 b4a5 c7d7 e1e3 h8g8 d2e1 f6f5 g2h3 d7e7 e4f5 g3e4 e1f1 e7f7 a5b6 g8h8 h3g2 h8h4 b6c7 e4d2 f1g1 h4g4 e3e2 d2f3 g1f1 f3h2 f1g1 h2f3 g1f2 g4g2 f2g2 f3d4 g2f2 d4e2 f2e2 f7f6 e2e3 f6f5 c7e5 f5e6 e3d4 c6f3 d4c4 f3e2 c4d4 e2b5 c3c4 b5e8 d4d3 e8g6 d3c3 e6d7 c3b3 g6e4 c2c3 d7c6 e5d6 c6d7 d6e5 d7c6 b3b4 e4b1 e5d4 b1c2 b4a5 c2f5 a5b4 f5d3 d4e3 d3b1 e3d4 b1e4 d4h8 e4g6 h8g7 g6h7 g7f8 h7e4 f8g7 e4c2 g7f8 c2h7 f8d6 h7d3 d6b8 d3h7 b8e5 h7b1 e5h8 b1e4 h8d4 e4c2 d4e3 c2f5 b4b3 f5g6 b3b4 g6c2 e3c1 c2e4 c1a3 e4g6 a3c1 g6b1 c1e3 b1d3 e3c1 d3b1 c1a3 b1c2 a3b2 c2f5 b2a3 f5d3 b4b3 d3f5 b3a4 f5c2 a4a5 c2f5 a5a4 f5c2 a4a5 c2g6 a5b4 g6c2 a3b2 c2f5 b4b3 c6c5 b2a3 c5c6 b3a4 f5c2 a4a5 c2d3 a5b4 d3f5 a3b2 f5c2 c4c5 c2d3 c3c4 d3f5 b2a3 f5h3 b4c3 h3e6 c3d4 c6c7 a3b2 c7d7 d4c3 e6g4 c3d4 d7e6 d4e3 g4h3 e3f2 h3f5 f2e3 f5h3 b2e5 e6d7 e5d6 h3f5 d6b8 d7c6 b8d6 c6d7 d6b8 d7c6 e3d4 f5e6 b8e5 e6f5 e5d6 f5e6 d6f8 c6c7 f8e7 c7c6 e7d6 c6d7 d6e5 e6h3 d4c3 h3g2 f4f5 g2e4 f5f6 e4g6 c3d4 g6f7 e5d6 d7e6 d6e7 e6d7 d4c3 f7g8 e7d6 g8f7 c3d4 d7e6 d6e5 e6d7 d4d3 d7c6 e5d6 c6d7 d6e5 f7g6 d3e3 g6f7 e3d4 f7g8 d4d3 d7c6 e5d6 c6d7 d3d4 g8f7 d6e7 f7e6 d4c3 e6g8 c3b4 d7c6 e7d6 g8f7 d6e5 f7e6 e5b2 e6f7 b2d4 f7e8 d4c3 c6b7 c3b2 b7c6 b2d4 c6c7 d4e5 c7c6 e5d4 e8f7 d4e5 f7g6 e5b2 g6f7 b2c3 f7h5 c3d4 h5e8 b4b3 e8h5 b3c3 h5e8 c3b2 e8h5 b2c3 h5g6 c3d2 g6f7 d2c3 f7g8 c3b3 g8f7 d4f2 c6d7 b3b4 d7c6 b4c3 f7e6 c3d4 c6d7 f2g3 e6f7 g3e5 d7c6 e5d6 f7e6 d6e7 e6f7 e7d6 f7g8 d6e5 g8e6 f6f7 e6f7 e5f4 f7h5 f4d6 h5g4 d6e5 g4h3 e5d6 c6b7 d6f4 h3f1 f4e5 f1e2 d4d5 e2d1 c5c6 b7c8 c4c5 d1e2 d5d6 e2f3 e5h8 f3d1 h8b2 d1f3 b2e5 f3e4 e5b2 e4h1 b2h8 h1f3 h8e5 f3g4 e5b2 g4h5 b2d4 h5g6 d4b2 g6h5 c6c7 h5f3 b2e5 f3g2 e5b2 g2f3 b2g7 f3h1 g7e5 h1g2 e5d4 g2c6 d4h8 c6f3 h8b2 f3d1 b2e5 d1e2 d6e6 e2b5 e6d6 b5e2 d6e6 e2f3 e6f5 f3b7 f5f6 b7f3 f6e6 f3c6 e6f5 c6g2 f5e6 g2c6 e6f6 c6d7 f6g5 d7h3 g5f6 h3g2 f6f5 g2b7 f5f4 c8d7 f4e3 d7c6 e3d4 b7c8 e5d6 c8b7 d4c4 b7a6 c4d4 a6b7 d6e5 c6d7 d4c4 b7a6 c4d4 a6b7 d4c4 d7c6 e5f4 b7a6 c4d4 a6c8 f4e5 c8b7 e5d6 c6d7 d6e5 d7c6 e5d6 c6d7 d6g3 d7c6 g3f4 b7c8 f4h2 c8b7 h2g3 c6d7 d4c4 d7c6 c4d4 c6d7 g3f4 d7c6 f4h2 c6d7 h2f4 d7c6 d4c4 b7a6 c4b4 a6c8 b4c3 c8a6 c7c8Q a6c8 c3d4 c8b7 d4c4 b7c8 c4d4 c8h3 f4c1 h3f1 c1a3 f1a6 a3b2 a6c8 b2a3 c8h3 d4e5 h3d7 e5d4 d7c8 a3b2 c8d7 d4c4 d7e6 c4d4 e6g4 b2a3 g4c8 a3b2 c8e6 b2c3 e6c8 d4c4 c8a6 c4d4 a6b7 d4c4 b7c8 c3b2 c8h3 b2a1 h3e6 c4d4 e6d5 a1c3 d5g2 d4c4 g2f1 c4d4 f1a6 c3d2 a6f1 d2f4 f1h3 f4h6 h3f1 h6g5 f1h3 d4c4 h3f1 c4d4 f1h3 g5f4 h3f1 f4e5 f1e2 e5d6 e2b5 d6e5 b5f1 e5b8 f1b5 b8d6 b5f1 d6e5 f1a6 e5f4 a6e2 f4e5 e2g4 e5d6 g4e6 d6e5 e6h3 e5f4 h3g2 f4d2 g2d5 d2f4 d5f7 f4e5 f7a2 e5d6 a2g8 d6e7 g8e6 e7d6 e6d7 d4e5 d7h3 e5f6 h3f1";

    /// <summary>
    /// 500 moves, https://www.chess.com/game/live/378106991
    /// </summary>
    public const string SuperLongPositionCommand = "position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 moves e2e4 e7e5 g1f3 g8f6 b1c3 b8c6 f1e2 f8b4 a2a3 b4c3 b2c3 f6e4 e2b5 d7d6 d1e2 e4f6 d2d4 e8g8 d4e5 d6e5 b5c6 b7c6 c1g5 h7h6 g5h4 f8e8 a1d1 d8e7 e1g1 c8g4 h2h3 g4f3 e2f3 e7e6 h4f6 e6f6 f3f6 g7f6 d1d7 a8c8 f1b1 e8d8 d7d3 e5e4 d3d4 d8d4 c3d4 c8d8 c2c3 d8d7 f2f3 e4e3 g1f1 f6f5 f3f4 d7e7 f1e2 h6h5 h3h4 g8g7 b1f1 e7e4 g2g3 g7f6 f1f3 f6e6 f3e3 e4e3 e2e3 e6d5 e3d3 c6c5 c3c4 d5c6 d4d5 c6d6 d3c3 c7c6 d5c6 d6c6 c3b3 c6b6 a3a4 a7a5 b3a3 f7f6 a3b3 b6c6 b3c3 c6d6 c3d3 d6c6 d3e3 c6d6 e3f3 d6e6 f3e3 e6d6 e3f3 d6e6 f3e3 e6d6 e3f2 d6e6 f2e2 e6d6 e2d3 d6e6 d3c3 e6d6 c3b3 d6e6 b3a3 e6d6 a3b3 d6e6 b3c2 e6d6 c2c3 d6e6 c3b2 e6d6 b2c2 d6e6 c2c3 e6e7 c3d3 e7d6 d3d2 d6e6 d2c3 e6e7 c3c2 e7d6 c2d3 d6e6 d3e2 e6d6 e2f3 d6e6 f3e3 e6d6 e3f2 d6e6 f2f3 e6d6 f3g2 d6e6 g2f1 e6d6 f1e2 d6e6 e2d2 e6d6 d2d3 d6e7 d3c2 e7e6 c2c3 e6d6 c3b3 d6d7 b3c2 d7e6 c2d3 e6d6 d3e2 d6e6 e2d2 e6d6 d2d1 d6e6 d1c1 e6d6 c1b1 d6e6 b1a2 e6d6 a2b2 d6e6 b2c3 e6d6 c3d2 d6e6 d2e1 e6d6 e1d1 d6e6 d1e2 e6d6 e2f1 d6e6 f1g1 e6d6 g1h1 d6e6 h1g1 e6d6 g1f1 d6e6 f1e1 e6d6 e1d1 d6e6 d1c1 e6d6 c1b1 d6e6 b1a1 e6d6 a1a2 d6e6 a2b2 e6d6 b2c2 d6e6 c2d2 e6d6 d2e2 d6e6 e2f2 e6d6 f2f3 d6e6 f3e3 e6d6 e3d3 d6e6 d3c3 e6d6 c3b3 d6e6 b3a3 e6f7 a3b2 f7g6 b2c3 g6h6 c3d3 h6g6 d3e3 g6h6 e3f3 h6g6 f3g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2f2 g6h6 f2e3 h6g6 e3d3 g6h6 d3c3 h6g6 c3c2 g6h6 c2d2 h6g6 d2d3 g6h6 d3e3 h6g6 e3e2 g6h6 e2f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f1 h6g6 f1e2 g6h6 e2e1 h6g6 e1d2 g6h6 d2c1 h6g6 c1c2 g6h6 c2d1 h6g6 d1e2 g6h6 e2f1 h6g6 f1f2 g6h6 f2f3 h6g6 f3e3 g6h6 e3d3 h6g6 d3c3 g6h6 c3b3 h6g6 b3a3 g6h6 a3b2 h6g6 b2a2 g6h6 a2a1 h6g6 a1b2 g6h6 b2c1 h6g6 c1c2 g6h6 c2c3 h6g6 c3b3 g6h6 b3b2 h6g6 b2a3 g6h6 a3b3 h6g6 b3c3 g6h6 c3d3 h6g6 d3e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h2 g6h6 h2g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2g1 h6g6 g1f2 g6h6 f2f1 h6g6 f1e2 g6h6 e2e3 h6g6 e3d2 g6h6 d2d3 h6g6 d3e3 g6h6 e3e2 h6g6 e2f3 g6h6 f3f2 h6g6 f2e2 g6h6 e2e3 h6g6 e3f3 g6h6 f3f2 h6g6 f2g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3d3 g6h6 d3d2 h6g6 d2c3 g6h6 c3c2 h6g6 c2d1 g6h6 d1d2 h6g6 d2e1 g6h6 e1e2 h6g6 e2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h1 h6g6 h1g1 g6h6 g1f1 h6g6 f1e1 g6h6 e1d1 h6g6 d1c2 g6h6 c2c3 h6g6 c3d3 g6h6 d3d2 h6g6 d2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f1 g6h6 f1e2 h6g6 e2f3 g6h6 f3e3 h6g6 e3f2 g6h6 f2f1 h6g6 f1g2 g6h6 g2h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2f2 g6h6 f2f3 h6g6 f3e2 g6h6 e2e3 h6g6 e3d3 g6h6 d3c3 h6g6 c3c2 g6h6 c2c3 h6g6 c3c2 g6h6 c2d2 h6g6 d2d3 g6h6 d3e2 h6g6 e2e3 g6h6 e3e2 h6g6 e2f3 g6h6 f3f2 h6g6 f2f1 g6h6 f1g2 h6g6 g2g1 g6h6 g1h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1h2 g6h6 h2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2f1 g6h6 f1e2 h6g6 e2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h2 g6h6 h2h3 h6g6 h3g2 g6h6 g2h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3f2 h6g6 f2g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1g1 g6h6 g1f2 h6g6 f2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2g1 h6g6 g1h1 g6h6 h1h2 h6g6 h2h3 g6h6 h3g2 h6g6 g2f3 g6h6 f3f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2g1 g6h6 g1g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1g2 g6h6 g2h2 h6g6 h2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2g2 h6g6 g2f2 g6h6 f2f3 h6g6 f3e2 g6h6 e2e1 h6g6 e1f2 g6h6 f2g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3e2 g6h6 e2e3 h6g6 e3f3 g6h6 f3f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3f2 h6g6 f2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h1 h6g6 h1h2 g6h6 h2g1 h6g6 g1g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2f2 h6h7 f2f3 h7g7 f3g2 g7g6 g2h3 g6h6 h3h2 h6h7 h2g1 h7h8 g1h1 h8g8 h1g2 g8g7 g2f2 g7g6 f2f1 g6h6 f1e2 h6h7 e2f3 h7h8 f3f2 h8g8 f2f1 g8g7 f1g2 g7g6 g2h2 g6h6 h2h3 h6h7 h3h2 h7h8 h2h1 h8g8 h1g1 g8g7 g1g2 g7g6 g2h2 g6h6 h2g1 h6h7 g1g2 h7h8 g2h1 h8g8 h1h2 g8g7 h2g2 g7g6 g2g1 g6h6 g1g2 h6h7 g2h2 h7h8 h2h3 h8g8 h3h2 g8g7 h2h1 g7g6 h1g1 g6h6 g1g2 h6h7 g2f2 h7h8 f2f3 h8g8 f3g2 g8g7 g2h3 g7g6 h3h2 g6h6 h2h1 h6h7 h1g2 h7h6 g2g1 h6g6 g1g2 g6h6 g2g1 h6h7 g1g2 h7g7 g2g1 g7g6 g1g2 g6h6 g2g1 h6h7 g1g2 h7g7 g2g1 g7h6 g1g2 h6h7 g2g1 h7h8 g1g2 h8g8 g2g1 g8f7 g1g2 f7g6 g2g1 g6h6 g1g2 h6h7 g2g1 h7h8 g1g2 h8g8 g2g1 g8f7 g1g2 f7g6 g2g1 g6g7 g1g2 g7f8 g2g1 f8f7 g1g2 f7g8 g2g1 g8g7 g1g2 g7h8 g2g1 h8h7 g1g2 h7h6 g2g1 h6g7";

    public const string SuperLongPositionCommand_DFRC = "position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 moves e2e4 e7e5 g1f3 g8f6 b1c3 b8c6 f1e2 f8b4 a2a3 b4c3 b2c3 f6e4 e2b5 d7d6 d1e2 e4f6 d2d4 e8h8 d4e5 d6e5 b5c6 b7c6 c1g5 h7h6 g5h4 f8e8 a1d1 d8e7 e1h1 c8g4 h2h3 g4f3 e2f3 e7e6 h4f6 e6f6 f3f6 g7f6 d1d7 a8c8 f1b1 e8d8 d7d3 e5e4 d3d4 d8d4 c3d4 c8d8 c2c3 d8d7 f2f3 e4e3 g1f1 f6f5 f3f4 d7e7 f1e2 h6h5 h3h4 g8g7 b1f1 e7e4 g2g3 g7f6 f1f3 f6e6 f3e3 e4e3 e2e3 e6d5 e3d3 c6c5 c3c4 d5c6 d4d5 c6d6 d3c3 c7c6 d5c6 d6c6 c3b3 c6b6 a3a4 a7a5 b3a3 f7f6 a3b3 b6c6 b3c3 c6d6 c3d3 d6c6 d3e3 c6d6 e3f3 d6e6 f3e3 e6d6 e3f3 d6e6 f3e3 e6d6 e3f2 d6e6 f2e2 e6d6 e2d3 d6e6 d3c3 e6d6 c3b3 d6e6 b3a3 e6d6 a3b3 d6e6 b3c2 e6d6 c2c3 d6e6 c3b2 e6d6 b2c2 d6e6 c2c3 e6e7 c3d3 e7d6 d3d2 d6e6 d2c3 e6e7 c3c2 e7d6 c2d3 d6e6 d3e2 e6d6 e2f3 d6e6 f3e3 e6d6 e3f2 d6e6 f2f3 e6d6 f3g2 d6e6 g2f1 e6d6 f1e2 d6e6 e2d2 e6d6 d2d3 d6e7 d3c2 e7e6 c2c3 e6d6 c3b3 d6d7 b3c2 d7e6 c2d3 e6d6 d3e2 d6e6 e2d2 e6d6 d2d1 d6e6 d1c1 e6d6 c1b1 d6e6 b1a2 e6d6 a2b2 d6e6 b2c3 e6d6 c3d2 d6e6 d2e1 e6d6 e1d1 d6e6 d1e2 e6d6 e2f1 d6e6 f1g1 e6d6 g1h1 d6e6 h1g1 e6d6 g1f1 d6e6 f1e1 e6d6 e1d1 d6e6 d1c1 e6d6 c1b1 d6e6 b1a1 e6d6 a1a2 d6e6 a2b2 e6d6 b2c2 d6e6 c2d2 e6d6 d2e2 d6e6 e2f2 e6d6 f2f3 d6e6 f3e3 e6d6 e3d3 d6e6 d3c3 e6d6 c3b3 d6e6 b3a3 e6f7 a3b2 f7g6 b2c3 g6h6 c3d3 h6g6 d3e3 g6h6 e3f3 h6g6 f3g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2f2 g6h6 f2e3 h6g6 e3d3 g6h6 d3c3 h6g6 c3c2 g6h6 c2d2 h6g6 d2d3 g6h6 d3e3 h6g6 e3e2 g6h6 e2f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f1 h6g6 f1e2 g6h6 e2e1 h6g6 e1d2 g6h6 d2c1 h6g6 c1c2 g6h6 c2d1 h6g6 d1e2 g6h6 e2f1 h6g6 f1f2 g6h6 f2f3 h6g6 f3e3 g6h6 e3d3 h6g6 d3c3 g6h6 c3b3 h6g6 b3a3 g6h6 a3b2 h6g6 b2a2 g6h6 a2a1 h6g6 a1b2 g6h6 b2c1 h6g6 c1c2 g6h6 c2c3 h6g6 c3b3 g6h6 b3b2 h6g6 b2a3 g6h6 a3b3 h6g6 b3c3 g6h6 c3d3 h6g6 d3e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h2 g6h6 h2g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2g1 h6g6 g1f2 g6h6 f2f1 h6g6 f1e2 g6h6 e2e3 h6g6 e3d2 g6h6 d2d3 h6g6 d3e3 g6h6 e3e2 h6g6 e2f3 g6h6 f3f2 h6g6 f2e2 g6h6 e2e3 h6g6 e3f3 g6h6 f3f2 h6g6 f2g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3d3 g6h6 d3d2 h6g6 d2c3 g6h6 c3c2 h6g6 c2d1 g6h6 d1d2 h6g6 d2e1 g6h6 e1e2 h6g6 e2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h1 h6g6 h1g1 g6h6 g1f1 h6g6 f1e1 g6h6 e1d1 h6g6 d1c2 g6h6 c2c3 h6g6 c3d3 g6h6 d3d2 h6g6 d2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f1 g6h6 f1e2 h6g6 e2f3 g6h6 f3e3 h6g6 e3f2 g6h6 f2f1 h6g6 f1g2 g6h6 g2h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2f2 g6h6 f2f3 h6g6 f3e2 g6h6 e2e3 h6g6 e3d3 g6h6 d3c3 h6g6 c3c2 g6h6 c2c3 h6g6 c3c2 g6h6 c2d2 h6g6 d2d3 g6h6 d3e2 h6g6 e2e3 g6h6 e3e2 h6g6 e2f3 g6h6 f3f2 h6g6 f2f1 g6h6 f1g2 h6g6 g2g1 g6h6 g1h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1h2 g6h6 h2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2f1 g6h6 f1e2 h6g6 e2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h2 g6h6 h2h3 h6g6 h3g2 g6h6 g2h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3f2 h6g6 f2g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1g1 g6h6 g1f2 h6g6 f2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2g1 h6g6 g1h1 g6h6 h1h2 h6g6 h2h3 g6h6 h3g2 h6g6 g2f3 g6h6 f3f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2g1 g6h6 g1g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1g2 g6h6 g2h2 h6g6 h2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2g2 h6g6 g2f2 g6h6 f2f3 h6g6 f3e2 g6h6 e2e1 h6g6 e1f2 g6h6 f2g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3e2 g6h6 e2e3 h6g6 e3f3 g6h6 f3f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3f2 h6g6 f2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h1 h6g6 h1h2 g6h6 h2g1 h6g6 g1g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2f2 h6h7 f2f3 h7g7 f3g2 g7g6 g2h3 g6h6 h3h2 h6h7 h2g1 h7h8 g1h1 h8g8 h1g2 g8g7 g2f2 g7g6 f2f1 g6h6 f1e2 h6h7 e2f3 h7h8 f3f2 h8g8 f2f1 g8g7 f1g2 g7g6 g2h2 g6h6 h2h3 h6h7 h3h2 h7h8 h2h1 h8g8 h1g1 g8g7 g1g2 g7g6 g2h2 g6h6 h2g1 h6h7 g1g2 h7h8 g2h1 h8g8 h1h2 g8g7 h2g2 g7g6 g2g1 g6h6 g1g2 h6h7 g2h2 h7h8 h2h3 h8g8 h3h2 g8g7 h2h1 g7g6 h1g1 g6h6 g1g2 h6h7 g2f2 h7h8 f2f3 h8g8 f3g2 g8g7 g2h3 g7g6 h3h2 g6h6 h2h1 h6h7 h1g2 h7h6 g2g1 h6g6 g1g2 g6h6 g2g1 h6h7 g1g2 h7g7 g2g1 g7g6 g1g2 g6h6 g2g1 h6h7 g1g2 h7g7 g2g1 g7h6 g1g2 h6h7 g2g1 h7h8 g1g2 h8g8 g2g1 g8f7 g1g2 f7g6 g2g1 g6h6 g1g2 h6h7 g2g1 h7h8 g1g2 h8g8 g2g1 g8f7 g1g2 f7g6 g2g1 g6g7 g1g2 g7f8 g2g1 f8f7 g1g2 f7g8 g2g1 g8g7 g1g2 g7h8 g2g1 h8h7 g1g2 h7h6 g2g1 h6g7";

    /// <summary>
    /// 64 x 64
    /// </summary>
    public static readonly int[][] ChebyshevDistance =
    [
        [0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7],
        [1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
        [2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
        [3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
        [4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
        [5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
        [6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
        [7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7],
        [1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 6, 6, 6, 6, 6, 6, 6, 7],
        [1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6],
        [2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
        [3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
        [4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
        [5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
        [6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6],
        [7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 6, 6, 6, 6, 6, 6],
        [2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7],
        [2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6],
        [2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5],
        [3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5],
        [4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5],
        [5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5],
        [6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5],
        [7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 5, 5, 5, 5, 5],
        [3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7],
        [3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6],
        [3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5],
        [3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4],
        [4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4],
        [5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4],
        [6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 4, 4, 4, 4, 4],
        [7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 4, 4, 4, 4],
        [4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7],
        [4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6],
        [4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5],
        [4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 4],
        [4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3],
        [5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 3, 3, 3, 3, 3],
        [6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 3, 3, 3, 3],
        [7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 3, 3, 3],
        [5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7],
        [5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6],
        [5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5],
        [5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4],
        [5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 2, 2, 2, 2, 3],
        [5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 2, 2, 2, 2],
        [6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 2, 2, 2],
        [7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 2, 2],
        [6, 6, 6, 6, 6, 6, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7],
        [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6],
        [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5],
        [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4],
        [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3],
        [6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2, 5, 4, 3, 2, 1, 1, 1, 2],
        [6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1, 6, 5, 4, 3, 2, 1, 1, 1],
        [7, 6, 6, 6, 6, 6, 6, 6, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0, 7, 6, 5, 4, 3, 2, 1, 1],
        [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 7, 5, 5, 5, 5, 5, 5, 6, 7, 4, 4, 4, 4, 4, 5, 6, 7, 3, 3, 3, 3, 4, 5, 6, 7, 2, 2, 2, 3, 4, 5, 6, 7, 1, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7],
        [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 4, 4, 4, 4, 4, 4, 5, 6, 3, 3, 3, 3, 3, 4, 5, 6, 2, 2, 2, 2, 3, 4, 5, 6, 1, 1, 1, 2, 3, 4, 5, 6, 1, 0, 1, 2, 3, 4, 5, 6],
        [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 3, 3, 3, 3, 3, 3, 4, 5, 2, 2, 2, 2, 2, 3, 4, 5, 2, 1, 1, 1, 2, 3, 4, 5, 2, 1, 0, 1, 2, 3, 4, 5],
        [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3, 4],
        [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 2, 2, 2, 2, 2, 3, 4, 3, 2, 1, 1, 1, 2, 3, 4, 3, 2, 1, 0, 1, 2, 3],
        [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 3, 3, 3, 3, 3, 3, 5, 4, 3, 2, 2, 2, 2, 2, 5, 4, 3, 2, 1, 1, 1, 2, 5, 4, 3, 2, 1, 0, 1, 2],
        [7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 5, 6, 5, 4, 4, 4, 4, 4, 4, 6, 5, 4, 3, 3, 3, 3, 3, 6, 5, 4, 3, 2, 2, 2, 2, 6, 5, 4, 3, 2, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0, 1],
        [7, 7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 7, 6, 5, 5, 5, 5, 5, 5, 7, 6, 5, 4, 4, 4, 4, 4, 7, 6, 5, 4, 3, 3, 3, 3, 7, 6, 5, 4, 3, 2, 2, 2, 7, 6, 5, 4, 3, 2, 1, 1, 7, 6, 5, 4, 3, 2, 1, 0]
    ];

    /// <summary>
    /// 64 x 64
    /// </summary>
    public static readonly int[][] ManhattanDistance =
    [
        [0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 3, 4, 5, 6, 7, 8, 2, 3, 4, 5, 6, 7, 8, 9, 3, 4, 5, 6, 7, 8, 9, 10, 4, 5, 6, 7, 8, 9, 10, 11, 5, 6, 7, 8, 9, 10, 11, 12, 6, 7, 8, 9, 10, 11, 12, 13, 7, 8, 9, 10, 11, 12, 13, 14, ],
        [1, 0, 1, 2, 3, 4, 5, 6, 2, 1, 2, 3, 4, 5, 6, 7, 3, 2, 3, 4, 5, 6, 7, 8, 4, 3, 4, 5, 6, 7, 8, 9, 5, 4, 5, 6, 7, 8, 9, 10, 6, 5, 6, 7, 8, 9, 10, 11, 7, 6, 7, 8, 9, 10, 11, 12, 8, 7, 8, 9, 10, 11, 12, 13, ],
        [2, 1, 0, 1, 2, 3, 4, 5, 3, 2, 1, 2, 3, 4, 5, 6, 4, 3, 2, 3, 4, 5, 6, 7, 5, 4, 3, 4, 5, 6, 7, 8, 6, 5, 4, 5, 6, 7, 8, 9, 7, 6, 5, 6, 7, 8, 9, 10, 8, 7, 6, 7, 8, 9, 10, 11, 9, 8, 7, 8, 9, 10, 11, 12, ],
        [3, 2, 1, 0, 1, 2, 3, 4, 4, 3, 2, 1, 2, 3, 4, 5, 5, 4, 3, 2, 3, 4, 5, 6, 6, 5, 4, 3, 4, 5, 6, 7, 7, 6, 5, 4, 5, 6, 7, 8, 8, 7, 6, 5, 6, 7, 8, 9, 9, 8, 7, 6, 7, 8, 9, 10, 10, 9, 8, 7, 8, 9, 10, 11, ],
        [4, 3, 2, 1, 0, 1, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, 6, 5, 4, 3, 2, 3, 4, 5, 7, 6, 5, 4, 3, 4, 5, 6, 8, 7, 6, 5, 4, 5, 6, 7, 9, 8, 7, 6, 5, 6, 7, 8, 10, 9, 8, 7, 6, 7, 8, 9, 11, 10, 9, 8, 7, 8, 9, 10, ],
        [5, 4, 3, 2, 1, 0, 1, 2, 6, 5, 4, 3, 2, 1, 2, 3, 7, 6, 5, 4, 3, 2, 3, 4, 8, 7, 6, 5, 4, 3, 4, 5, 9, 8, 7, 6, 5, 4, 5, 6, 10, 9, 8, 7, 6, 5, 6, 7, 11, 10, 9, 8, 7, 6, 7, 8, 12, 11, 10, 9, 8, 7, 8, 9, ],
        [6, 5, 4, 3, 2, 1, 0, 1, 7, 6, 5, 4, 3, 2, 1, 2, 8, 7, 6, 5, 4, 3, 2, 3, 9, 8, 7, 6, 5, 4, 3, 4, 10, 9, 8, 7, 6, 5, 4, 5, 11, 10, 9, 8, 7, 6, 5, 6, 12, 11, 10, 9, 8, 7, 6, 7, 13, 12, 11, 10, 9, 8, 7, 8, ],
        [7, 6, 5, 4, 3, 2, 1, 0, 8, 7, 6, 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3, 2, 10, 9, 8, 7, 6, 5, 4, 3, 11, 10, 9, 8, 7, 6, 5, 4, 12, 11, 10, 9, 8, 7, 6, 5, 13, 12, 11, 10, 9, 8, 7, 6, 14, 13, 12, 11, 10, 9, 8, 7, ],
        [1, 2, 3, 4, 5, 6, 7, 8, 0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 3, 4, 5, 6, 7, 8, 2, 3, 4, 5, 6, 7, 8, 9, 3, 4, 5, 6, 7, 8, 9, 10, 4, 5, 6, 7, 8, 9, 10, 11, 5, 6, 7, 8, 9, 10, 11, 12, 6, 7, 8, 9, 10, 11, 12, 13, ],
        [2, 1, 2, 3, 4, 5, 6, 7, 1, 0, 1, 2, 3, 4, 5, 6, 2, 1, 2, 3, 4, 5, 6, 7, 3, 2, 3, 4, 5, 6, 7, 8, 4, 3, 4, 5, 6, 7, 8, 9, 5, 4, 5, 6, 7, 8, 9, 10, 6, 5, 6, 7, 8, 9, 10, 11, 7, 6, 7, 8, 9, 10, 11, 12, ],
        [3, 2, 1, 2, 3, 4, 5, 6, 2, 1, 0, 1, 2, 3, 4, 5, 3, 2, 1, 2, 3, 4, 5, 6, 4, 3, 2, 3, 4, 5, 6, 7, 5, 4, 3, 4, 5, 6, 7, 8, 6, 5, 4, 5, 6, 7, 8, 9, 7, 6, 5, 6, 7, 8, 9, 10, 8, 7, 6, 7, 8, 9, 10, 11, ],
        [4, 3, 2, 1, 2, 3, 4, 5, 3, 2, 1, 0, 1, 2, 3, 4, 4, 3, 2, 1, 2, 3, 4, 5, 5, 4, 3, 2, 3, 4, 5, 6, 6, 5, 4, 3, 4, 5, 6, 7, 7, 6, 5, 4, 5, 6, 7, 8, 8, 7, 6, 5, 6, 7, 8, 9, 9, 8, 7, 6, 7, 8, 9, 10, ],
        [5, 4, 3, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 1, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, 6, 5, 4, 3, 2, 3, 4, 5, 7, 6, 5, 4, 3, 4, 5, 6, 8, 7, 6, 5, 4, 5, 6, 7, 9, 8, 7, 6, 5, 6, 7, 8, 10, 9, 8, 7, 6, 7, 8, 9, ],
        [6, 5, 4, 3, 2, 1, 2, 3, 5, 4, 3, 2, 1, 0, 1, 2, 6, 5, 4, 3, 2, 1, 2, 3, 7, 6, 5, 4, 3, 2, 3, 4, 8, 7, 6, 5, 4, 3, 4, 5, 9, 8, 7, 6, 5, 4, 5, 6, 10, 9, 8, 7, 6, 5, 6, 7, 11, 10, 9, 8, 7, 6, 7, 8, ],
        [7, 6, 5, 4, 3, 2, 1, 2, 6, 5, 4, 3, 2, 1, 0, 1, 7, 6, 5, 4, 3, 2, 1, 2, 8, 7, 6, 5, 4, 3, 2, 3, 9, 8, 7, 6, 5, 4, 3, 4, 10, 9, 8, 7, 6, 5, 4, 5, 11, 10, 9, 8, 7, 6, 5, 6, 12, 11, 10, 9, 8, 7, 6, 7, ],
        [8, 7, 6, 5, 4, 3, 2, 1, 7, 6, 5, 4, 3, 2, 1, 0, 8, 7, 6, 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3, 2, 10, 9, 8, 7, 6, 5, 4, 3, 11, 10, 9, 8, 7, 6, 5, 4, 12, 11, 10, 9, 8, 7, 6, 5, 13, 12, 11, 10, 9, 8, 7, 6, ],
        [2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7, 8, 0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 3, 4, 5, 6, 7, 8, 2, 3, 4, 5, 6, 7, 8, 9, 3, 4, 5, 6, 7, 8, 9, 10, 4, 5, 6, 7, 8, 9, 10, 11, 5, 6, 7, 8, 9, 10, 11, 12, ],
        [3, 2, 3, 4, 5, 6, 7, 8, 2, 1, 2, 3, 4, 5, 6, 7, 1, 0, 1, 2, 3, 4, 5, 6, 2, 1, 2, 3, 4, 5, 6, 7, 3, 2, 3, 4, 5, 6, 7, 8, 4, 3, 4, 5, 6, 7, 8, 9, 5, 4, 5, 6, 7, 8, 9, 10, 6, 5, 6, 7, 8, 9, 10, 11, ],
        [4, 3, 2, 3, 4, 5, 6, 7, 3, 2, 1, 2, 3, 4, 5, 6, 2, 1, 0, 1, 2, 3, 4, 5, 3, 2, 1, 2, 3, 4, 5, 6, 4, 3, 2, 3, 4, 5, 6, 7, 5, 4, 3, 4, 5, 6, 7, 8, 6, 5, 4, 5, 6, 7, 8, 9, 7, 6, 5, 6, 7, 8, 9, 10, ],
        [5, 4, 3, 2, 3, 4, 5, 6, 4, 3, 2, 1, 2, 3, 4, 5, 3, 2, 1, 0, 1, 2, 3, 4, 4, 3, 2, 1, 2, 3, 4, 5, 5, 4, 3, 2, 3, 4, 5, 6, 6, 5, 4, 3, 4, 5, 6, 7, 7, 6, 5, 4, 5, 6, 7, 8, 8, 7, 6, 5, 6, 7, 8, 9, ],
        [6, 5, 4, 3, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 1, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, 6, 5, 4, 3, 2, 3, 4, 5, 7, 6, 5, 4, 3, 4, 5, 6, 8, 7, 6, 5, 4, 5, 6, 7, 9, 8, 7, 6, 5, 6, 7, 8, ],
        [7, 6, 5, 4, 3, 2, 3, 4, 6, 5, 4, 3, 2, 1, 2, 3, 5, 4, 3, 2, 1, 0, 1, 2, 6, 5, 4, 3, 2, 1, 2, 3, 7, 6, 5, 4, 3, 2, 3, 4, 8, 7, 6, 5, 4, 3, 4, 5, 9, 8, 7, 6, 5, 4, 5, 6, 10, 9, 8, 7, 6, 5, 6, 7, ],
        [8, 7, 6, 5, 4, 3, 2, 3, 7, 6, 5, 4, 3, 2, 1, 2, 6, 5, 4, 3, 2, 1, 0, 1, 7, 6, 5, 4, 3, 2, 1, 2, 8, 7, 6, 5, 4, 3, 2, 3, 9, 8, 7, 6, 5, 4, 3, 4, 10, 9, 8, 7, 6, 5, 4, 5, 11, 10, 9, 8, 7, 6, 5, 6, ],
        [9, 8, 7, 6, 5, 4, 3, 2, 8, 7, 6, 5, 4, 3, 2, 1, 7, 6, 5, 4, 3, 2, 1, 0, 8, 7, 6, 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3, 2, 10, 9, 8, 7, 6, 5, 4, 3, 11, 10, 9, 8, 7, 6, 5, 4, 12, 11, 10, 9, 8, 7, 6, 5, ],
        [3, 4, 5, 6, 7, 8, 9, 10, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7, 8, 0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 3, 4, 5, 6, 7, 8, 2, 3, 4, 5, 6, 7, 8, 9, 3, 4, 5, 6, 7, 8, 9, 10, 4, 5, 6, 7, 8, 9, 10, 11, ],
        [4, 3, 4, 5, 6, 7, 8, 9, 3, 2, 3, 4, 5, 6, 7, 8, 2, 1, 2, 3, 4, 5, 6, 7, 1, 0, 1, 2, 3, 4, 5, 6, 2, 1, 2, 3, 4, 5, 6, 7, 3, 2, 3, 4, 5, 6, 7, 8, 4, 3, 4, 5, 6, 7, 8, 9, 5, 4, 5, 6, 7, 8, 9, 10, ],
        [5, 4, 3, 4, 5, 6, 7, 8, 4, 3, 2, 3, 4, 5, 6, 7, 3, 2, 1, 2, 3, 4, 5, 6, 2, 1, 0, 1, 2, 3, 4, 5, 3, 2, 1, 2, 3, 4, 5, 6, 4, 3, 2, 3, 4, 5, 6, 7, 5, 4, 3, 4, 5, 6, 7, 8, 6, 5, 4, 5, 6, 7, 8, 9, ],
        [6, 5, 4, 3, 4, 5, 6, 7, 5, 4, 3, 2, 3, 4, 5, 6, 4, 3, 2, 1, 2, 3, 4, 5, 3, 2, 1, 0, 1, 2, 3, 4, 4, 3, 2, 1, 2, 3, 4, 5, 5, 4, 3, 2, 3, 4, 5, 6, 6, 5, 4, 3, 4, 5, 6, 7, 7, 6, 5, 4, 5, 6, 7, 8, ],
        [7, 6, 5, 4, 3, 4, 5, 6, 6, 5, 4, 3, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 1, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, 6, 5, 4, 3, 2, 3, 4, 5, 7, 6, 5, 4, 3, 4, 5, 6, 8, 7, 6, 5, 4, 5, 6, 7, ],
        [8, 7, 6, 5, 4, 3, 4, 5, 7, 6, 5, 4, 3, 2, 3, 4, 6, 5, 4, 3, 2, 1, 2, 3, 5, 4, 3, 2, 1, 0, 1, 2, 6, 5, 4, 3, 2, 1, 2, 3, 7, 6, 5, 4, 3, 2, 3, 4, 8, 7, 6, 5, 4, 3, 4, 5, 9, 8, 7, 6, 5, 4, 5, 6, ],
        [9, 8, 7, 6, 5, 4, 3, 4, 8, 7, 6, 5, 4, 3, 2, 3, 7, 6, 5, 4, 3, 2, 1, 2, 6, 5, 4, 3, 2, 1, 0, 1, 7, 6, 5, 4, 3, 2, 1, 2, 8, 7, 6, 5, 4, 3, 2, 3, 9, 8, 7, 6, 5, 4, 3, 4, 10, 9, 8, 7, 6, 5, 4, 5, ],
        [10, 9, 8, 7, 6, 5, 4, 3, 9, 8, 7, 6, 5, 4, 3, 2, 8, 7, 6, 5, 4, 3, 2, 1, 7, 6, 5, 4, 3, 2, 1, 0, 8, 7, 6, 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3, 2, 10, 9, 8, 7, 6, 5, 4, 3, 11, 10, 9, 8, 7, 6, 5, 4, ],
        [4, 5, 6, 7, 8, 9, 10, 11, 3, 4, 5, 6, 7, 8, 9, 10, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7, 8, 0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 3, 4, 5, 6, 7, 8, 2, 3, 4, 5, 6, 7, 8, 9, 3, 4, 5, 6, 7, 8, 9, 10, ],
        [5, 4, 5, 6, 7, 8, 9, 10, 4, 3, 4, 5, 6, 7, 8, 9, 3, 2, 3, 4, 5, 6, 7, 8, 2, 1, 2, 3, 4, 5, 6, 7, 1, 0, 1, 2, 3, 4, 5, 6, 2, 1, 2, 3, 4, 5, 6, 7, 3, 2, 3, 4, 5, 6, 7, 8, 4, 3, 4, 5, 6, 7, 8, 9, ],
        [6, 5, 4, 5, 6, 7, 8, 9, 5, 4, 3, 4, 5, 6, 7, 8, 4, 3, 2, 3, 4, 5, 6, 7, 3, 2, 1, 2, 3, 4, 5, 6, 2, 1, 0, 1, 2, 3, 4, 5, 3, 2, 1, 2, 3, 4, 5, 6, 4, 3, 2, 3, 4, 5, 6, 7, 5, 4, 3, 4, 5, 6, 7, 8, ],
        [7, 6, 5, 4, 5, 6, 7, 8, 6, 5, 4, 3, 4, 5, 6, 7, 5, 4, 3, 2, 3, 4, 5, 6, 4, 3, 2, 1, 2, 3, 4, 5, 3, 2, 1, 0, 1, 2, 3, 4, 4, 3, 2, 1, 2, 3, 4, 5, 5, 4, 3, 2, 3, 4, 5, 6, 6, 5, 4, 3, 4, 5, 6, 7, ],
        [8, 7, 6, 5, 4, 5, 6, 7, 7, 6, 5, 4, 3, 4, 5, 6, 6, 5, 4, 3, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 1, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, 6, 5, 4, 3, 2, 3, 4, 5, 7, 6, 5, 4, 3, 4, 5, 6, ],
        [9, 8, 7, 6, 5, 4, 5, 6, 8, 7, 6, 5, 4, 3, 4, 5, 7, 6, 5, 4, 3, 2, 3, 4, 6, 5, 4, 3, 2, 1, 2, 3, 5, 4, 3, 2, 1, 0, 1, 2, 6, 5, 4, 3, 2, 1, 2, 3, 7, 6, 5, 4, 3, 2, 3, 4, 8, 7, 6, 5, 4, 3, 4, 5, ],
        [10, 9, 8, 7, 6, 5, 4, 5, 9, 8, 7, 6, 5, 4, 3, 4, 8, 7, 6, 5, 4, 3, 2, 3, 7, 6, 5, 4, 3, 2, 1, 2, 6, 5, 4, 3, 2, 1, 0, 1, 7, 6, 5, 4, 3, 2, 1, 2, 8, 7, 6, 5, 4, 3, 2, 3, 9, 8, 7, 6, 5, 4, 3, 4, ],
        [11, 10, 9, 8, 7, 6, 5, 4, 10, 9, 8, 7, 6, 5, 4, 3, 9, 8, 7, 6, 5, 4, 3, 2, 8, 7, 6, 5, 4, 3, 2, 1, 7, 6, 5, 4, 3, 2, 1, 0, 8, 7, 6, 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3, 2, 10, 9, 8, 7, 6, 5, 4, 3, ],
        [5, 6, 7, 8, 9, 10, 11, 12, 4, 5, 6, 7, 8, 9, 10, 11, 3, 4, 5, 6, 7, 8, 9, 10, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7, 8, 0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 3, 4, 5, 6, 7, 8, 2, 3, 4, 5, 6, 7, 8, 9, ],
        [6, 5, 6, 7, 8, 9, 10, 11, 5, 4, 5, 6, 7, 8, 9, 10, 4, 3, 4, 5, 6, 7, 8, 9, 3, 2, 3, 4, 5, 6, 7, 8, 2, 1, 2, 3, 4, 5, 6, 7, 1, 0, 1, 2, 3, 4, 5, 6, 2, 1, 2, 3, 4, 5, 6, 7, 3, 2, 3, 4, 5, 6, 7, 8, ],
        [7, 6, 5, 6, 7, 8, 9, 10, 6, 5, 4, 5, 6, 7, 8, 9, 5, 4, 3, 4, 5, 6, 7, 8, 4, 3, 2, 3, 4, 5, 6, 7, 3, 2, 1, 2, 3, 4, 5, 6, 2, 1, 0, 1, 2, 3, 4, 5, 3, 2, 1, 2, 3, 4, 5, 6, 4, 3, 2, 3, 4, 5, 6, 7, ],
        [8, 7, 6, 5, 6, 7, 8, 9, 7, 6, 5, 4, 5, 6, 7, 8, 6, 5, 4, 3, 4, 5, 6, 7, 5, 4, 3, 2, 3, 4, 5, 6, 4, 3, 2, 1, 2, 3, 4, 5, 3, 2, 1, 0, 1, 2, 3, 4, 4, 3, 2, 1, 2, 3, 4, 5, 5, 4, 3, 2, 3, 4, 5, 6, ],
        [9, 8, 7, 6, 5, 6, 7, 8, 8, 7, 6, 5, 4, 5, 6, 7, 7, 6, 5, 4, 3, 4, 5, 6, 6, 5, 4, 3, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 1, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, 6, 5, 4, 3, 2, 3, 4, 5, ],
        [10, 9, 8, 7, 6, 5, 6, 7, 9, 8, 7, 6, 5, 4, 5, 6, 8, 7, 6, 5, 4, 3, 4, 5, 7, 6, 5, 4, 3, 2, 3, 4, 6, 5, 4, 3, 2, 1, 2, 3, 5, 4, 3, 2, 1, 0, 1, 2, 6, 5, 4, 3, 2, 1, 2, 3, 7, 6, 5, 4, 3, 2, 3, 4, ],
        [11, 10, 9, 8, 7, 6, 5, 6, 10, 9, 8, 7, 6, 5, 4, 5, 9, 8, 7, 6, 5, 4, 3, 4, 8, 7, 6, 5, 4, 3, 2, 3, 7, 6, 5, 4, 3, 2, 1, 2, 6, 5, 4, 3, 2, 1, 0, 1, 7, 6, 5, 4, 3, 2, 1, 2, 8, 7, 6, 5, 4, 3, 2, 3, ],
        [12, 11, 10, 9, 8, 7, 6, 5, 11, 10, 9, 8, 7, 6, 5, 4, 10, 9, 8, 7, 6, 5, 4, 3, 9, 8, 7, 6, 5, 4, 3, 2, 8, 7, 6, 5, 4, 3, 2, 1, 7, 6, 5, 4, 3, 2, 1, 0, 8, 7, 6, 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3, 2, ],
        [6, 7, 8, 9, 10, 11, 12, 13, 5, 6, 7, 8, 9, 10, 11, 12, 4, 5, 6, 7, 8, 9, 10, 11, 3, 4, 5, 6, 7, 8, 9, 10, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7, 8, 0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 3, 4, 5, 6, 7, 8, ],
        [7, 6, 7, 8, 9, 10, 11, 12, 6, 5, 6, 7, 8, 9, 10, 11, 5, 4, 5, 6, 7, 8, 9, 10, 4, 3, 4, 5, 6, 7, 8, 9, 3, 2, 3, 4, 5, 6, 7, 8, 2, 1, 2, 3, 4, 5, 6, 7, 1, 0, 1, 2, 3, 4, 5, 6, 2, 1, 2, 3, 4, 5, 6, 7, ],
        [8, 7, 6, 7, 8, 9, 10, 11, 7, 6, 5, 6, 7, 8, 9, 10, 6, 5, 4, 5, 6, 7, 8, 9, 5, 4, 3, 4, 5, 6, 7, 8, 4, 3, 2, 3, 4, 5, 6, 7, 3, 2, 1, 2, 3, 4, 5, 6, 2, 1, 0, 1, 2, 3, 4, 5, 3, 2, 1, 2, 3, 4, 5, 6, ],
        [9, 8, 7, 6, 7, 8, 9, 10, 8, 7, 6, 5, 6, 7, 8, 9, 7, 6, 5, 4, 5, 6, 7, 8, 6, 5, 4, 3, 4, 5, 6, 7, 5, 4, 3, 2, 3, 4, 5, 6, 4, 3, 2, 1, 2, 3, 4, 5, 3, 2, 1, 0, 1, 2, 3, 4, 4, 3, 2, 1, 2, 3, 4, 5, ],
        [10, 9, 8, 7, 6, 7, 8, 9, 9, 8, 7, 6, 5, 6, 7, 8, 8, 7, 6, 5, 4, 5, 6, 7, 7, 6, 5, 4, 3, 4, 5, 6, 6, 5, 4, 3, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 1, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, ],
        [11, 10, 9, 8, 7, 6, 7, 8, 10, 9, 8, 7, 6, 5, 6, 7, 9, 8, 7, 6, 5, 4, 5, 6, 8, 7, 6, 5, 4, 3, 4, 5, 7, 6, 5, 4, 3, 2, 3, 4, 6, 5, 4, 3, 2, 1, 2, 3, 5, 4, 3, 2, 1, 0, 1, 2, 6, 5, 4, 3, 2, 1, 2, 3, ],
        [12, 11, 10, 9, 8, 7, 6, 7, 11, 10, 9, 8, 7, 6, 5, 6, 10, 9, 8, 7, 6, 5, 4, 5, 9, 8, 7, 6, 5, 4, 3, 4, 8, 7, 6, 5, 4, 3, 2, 3, 7, 6, 5, 4, 3, 2, 1, 2, 6, 5, 4, 3, 2, 1, 0, 1, 7, 6, 5, 4, 3, 2, 1, 2, ],
        [13, 12, 11, 10, 9, 8, 7, 6, 12, 11, 10, 9, 8, 7, 6, 5, 11, 10, 9, 8, 7, 6, 5, 4, 10, 9, 8, 7, 6, 5, 4, 3, 9, 8, 7, 6, 5, 4, 3, 2, 8, 7, 6, 5, 4, 3, 2, 1, 7, 6, 5, 4, 3, 2, 1, 0, 8, 7, 6, 5, 4, 3, 2, 1, ],
        [7, 8, 9, 10, 11, 12, 13, 14, 6, 7, 8, 9, 10, 11, 12, 13, 5, 6, 7, 8, 9, 10, 11, 12, 4, 5, 6, 7, 8, 9, 10, 11, 3, 4, 5, 6, 7, 8, 9, 10, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7, 8, 0, 1, 2, 3, 4, 5, 6, 7, ],
        [8, 7, 8, 9, 10, 11, 12, 13, 7, 6, 7, 8, 9, 10, 11, 12, 6, 5, 6, 7, 8, 9, 10, 11, 5, 4, 5, 6, 7, 8, 9, 10, 4, 3, 4, 5, 6, 7, 8, 9, 3, 2, 3, 4, 5, 6, 7, 8, 2, 1, 2, 3, 4, 5, 6, 7, 1, 0, 1, 2, 3, 4, 5, 6, ],
        [9, 8, 7, 8, 9, 10, 11, 12, 8, 7, 6, 7, 8, 9, 10, 11, 7, 6, 5, 6, 7, 8, 9, 10, 6, 5, 4, 5, 6, 7, 8, 9, 5, 4, 3, 4, 5, 6, 7, 8, 4, 3, 2, 3, 4, 5, 6, 7, 3, 2, 1, 2, 3, 4, 5, 6, 2, 1, 0, 1, 2, 3, 4, 5, ],
        [10, 9, 8, 7, 8, 9, 10, 11, 9, 8, 7, 6, 7, 8, 9, 10, 8, 7, 6, 5, 6, 7, 8, 9, 7, 6, 5, 4, 5, 6, 7, 8, 6, 5, 4, 3, 4, 5, 6, 7, 5, 4, 3, 2, 3, 4, 5, 6, 4, 3, 2, 1, 2, 3, 4, 5, 3, 2, 1, 0, 1, 2, 3, 4, ],
        [11, 10, 9, 8, 7, 8, 9, 10, 10, 9, 8, 7, 6, 7, 8, 9, 9, 8, 7, 6, 5, 6, 7, 8, 8, 7, 6, 5, 4, 5, 6, 7, 7, 6, 5, 4, 3, 4, 5, 6, 6, 5, 4, 3, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 1, 2, 3, ],
        [12, 11, 10, 9, 8, 7, 8, 9, 11, 10, 9, 8, 7, 6, 7, 8, 10, 9, 8, 7, 6, 5, 6, 7, 9, 8, 7, 6, 5, 4, 5, 6, 8, 7, 6, 5, 4, 3, 4, 5, 7, 6, 5, 4, 3, 2, 3, 4, 6, 5, 4, 3, 2, 1, 2, 3, 5, 4, 3, 2, 1, 0, 1, 2, ],
        [13, 12, 11, 10, 9, 8, 7, 8, 12, 11, 10, 9, 8, 7, 6, 7, 11, 10, 9, 8, 7, 6, 5, 6, 10, 9, 8, 7, 6, 5, 4, 5, 9, 8, 7, 6, 5, 4, 3, 4, 8, 7, 6, 5, 4, 3, 2, 3, 7, 6, 5, 4, 3, 2, 1, 2, 6, 5, 4, 3, 2, 1, 0, 1, ],
        [14, 13, 12, 11, 10, 9, 8, 7, 13, 12, 11, 10, 9, 8, 7, 6, 12, 11, 10, 9, 8, 7, 6, 5, 11, 10, 9, 8, 7, 6, 5, 4, 10, 9, 8, 7, 6, 5, 4, 3, 9, 8, 7, 6, 5, 4, 3, 2, 8, 7, 6, 5, 4, 3, 2, 1, 7, 6, 5, 4, 3, 2, 1, 0, ],
    ];

    /// <summary>
    /// 262_144 * Marshal.SizeOf<PawnTableElement>() / 1024 = 4MB
    /// </summary>
    public const int KingPawnHashSize = 262_144;
    public const int KingPawnHashMask = KingPawnHashSize - 1;

    public const int PawnCorrHistoryHashSize = 16_384;
    public const int PawnCorrHistoryHashMask = PawnCorrHistoryHashSize - 1;

    public const int NonPawnCorrHistoryHashSize = 16_384;
    public const int NonPawnCorrHistoryHashMask = NonPawnCorrHistoryHashSize - 1;

    public const int MinorCorrHistoryHashSize = 16_384;
    public const int MinorCorrHistoryHashMask = MinorCorrHistoryHashSize - 1;

    public const int MajorCorrHistoryHashSize = 16_384;
    public const int MajorCorrHistoryHashMask = MajorCorrHistoryHashSize - 1;

    public const string NumberWithSignFormat = "+#;-#;0";
}

#pragma warning restore IDE0055
