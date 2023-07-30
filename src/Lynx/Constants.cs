using Lynx.Model;

namespace Lynx;

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

    public static readonly string[] Coordinates = new string[64]
    {
            "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",
            "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
            "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
            "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
            "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
            "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
            "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
            "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1"
    };

    /// <summary>
    /// Following <see cref="Piece"/> order
    /// </summary>
    public const string AsciiPieces = "PNBRQKpnbrqk";

    /// <summary>
    /// Following <see cref="Piece"/> order.
    /// Don't work stright away in Windows
    /// </summary>
    public static readonly string[] UnicodePieces = new string[12]
    {
            "♙", "♘", "♗", "♖", "♕", "♔",   // White
            "♟︎", "♞", "♝", "♜", "♛", "♚"    // Black
    };

    /// <summary>
    /// Covert ASCII character pieces to encoded constants
    /// </summary>
    public static readonly IReadOnlyDictionary<char, Piece> PiecesByChar = new Dictionary<char, Piece>(12)
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
    };

    /// <summary>
    /// Relevant bishop occupancy bit count per square
    /// </summary>
    public static readonly int[] BishopRelevantOccupancyBits = new int[64]
    {
            6, 5, 5, 5, 5, 5, 5, 6,
            5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 7, 7, 7, 7, 5, 5,
            5, 5, 7, 9, 9, 7, 5, 5,
            5, 5, 7, 9, 9, 7, 5, 5,
            5, 5, 7, 7, 7, 7, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5,
            6, 5, 5, 5, 5, 5, 5, 6,
    };

    /// <summary>
    /// Relevant rook occupancy bit count per square
    /// </summary>
    public static readonly int[] RookRelevantOccupancyBits = new int[64]
    {
            12, 11, 11, 11, 11, 11, 11, 12,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            12, 11, 11, 11, 11, 11, 11, 12
    };

    /// <summary>
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/init_magics/bbc.c
    /// </summary>
    public static readonly BitBoard[] RookMagicNumbers = new BitBoard[64]
    {
            0x8a80104000800020UL,
            0x140002000100040UL,
            0x2801880a0017001UL,
            0x100081001000420UL,
            0x200020010080420UL,
            0x3001c0002010008UL,
            0x8480008002000100UL,
            0x2080088004402900UL,
            0x800098204000UL,
            0x2024401000200040UL,
            0x100802000801000UL,
            0x120800800801000UL,
            0x208808088000400UL,
            0x2802200800400UL,
            0x2200800100020080UL,
            0x801000060821100UL,
            0x80044006422000UL,
            0x100808020004000UL,
            0x12108a0010204200UL,
            0x140848010000802UL,
            0x481828014002800UL,
            0x8094004002004100UL,
            0x4010040010010802UL,
            0x20008806104UL,
            0x100400080208000UL,
            0x2040002120081000UL,
            0x21200680100081UL,
            0x20100080080080UL,
            0x2000a00200410UL,
            0x20080800400UL,
            0x80088400100102UL,
            0x80004600042881UL,
            0x4040008040800020UL,
            0x440003000200801UL,
            0x4200011004500UL,
            0x188020010100100UL,
            0x14800401802800UL,
            0x2080040080800200UL,
            0x124080204001001UL,
            0x200046502000484UL,
            0x480400080088020UL,
            0x1000422010034000UL,
            0x30200100110040UL,
            0x100021010009UL,
            0x2002080100110004UL,
            0x202008004008002UL,
            0x20020004010100UL,
            0x2048440040820001UL,
            0x101002200408200UL,
            0x40802000401080UL,
            0x4008142004410100UL,
            0x2060820c0120200UL,
            0x1001004080100UL,
            0x20c020080040080UL,
            0x2935610830022400UL,
            0x44440041009200UL,
            0x280001040802101UL,
            0x2100190040002085UL,
            0x80c0084100102001UL,
            0x4024081001000421UL,
            0x20030a0244872UL,
            0x12001008414402UL,
            0x2006104900a0804UL,
            0x1004081002402UL
    };

    /// <summary>
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/init_magics/bbc.c
    /// </summary>
    public static readonly BitBoard[] BishopMagicNumbers = new BitBoard[64]
    {
            0x40040844404084UL,
            0x2004208a004208UL,
            0x10190041080202UL,
            0x108060845042010UL,
            0x581104180800210UL,
            0x2112080446200010UL,
            0x1080820820060210UL,
            0x3c0808410220200UL,
            0x4050404440404UL,
            0x21001420088UL,
            0x24d0080801082102UL,
            0x1020a0a020400UL,
            0x40308200402UL,
            0x4011002100800UL,
            0x401484104104005UL,
            0x801010402020200UL,
            0x400210c3880100UL,
            0x404022024108200UL,
            0x810018200204102UL,
            0x4002801a02003UL,
            0x85040820080400UL,
            0x810102c808880400UL,
            0xe900410884800UL,
            0x8002020480840102UL,
            0x220200865090201UL,
            0x2010100a02021202UL,
            0x152048408022401UL,
            0x20080002081110UL,
            0x4001001021004000UL,
            0x800040400a011002UL,
            0xe4004081011002UL,
            0x1c004001012080UL,
            0x8004200962a00220UL,
            0x8422100208500202UL,
            0x2000402200300c08UL,
            0x8646020080080080UL,
            0x80020a0200100808UL,
            0x2010004880111000UL,
            0x623000a080011400UL,
            0x42008c0340209202UL,
            0x209188240001000UL,
            0x400408a884001800UL,
            0x110400a6080400UL,
            0x1840060a44020800UL,
            0x90080104000041UL,
            0x201011000808101UL,
            0x1a2208080504f080UL,
            0x8012020600211212UL,
            0x500861011240000UL,
            0x180806108200800UL,
            0x4000020e01040044UL,
            0x300000261044000aUL,
            0x802241102020002UL,
            0x20906061210001UL,
            0x5a84841004010310UL,
            0x4010801011c04UL,
            0xa010109502200UL,
            0x4a02012000UL,
            0x500201010098b028UL,
            0x8040002811040900UL,
            0x28000010020204UL,
            0x6000020202d0240UL,
            0x8918844842082200UL,
            0x4010011029020020UL
};

    public const string InitialPositionFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

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

    public const int WhiteShortCastleKingSquare = (int)BoardSquare.g1;
    public const int BlackShortCastleKingSquare = (int)BoardSquare.g8;
    public const int WhiteLongCastleKingSquare = (int)BoardSquare.c1;
    public const int BlackLongCastleKingSquare = (int)BoardSquare.c8;

    public const int WhiteShortCastleRookSquare = (int)BoardSquare.f1;
    public const int BlackShortCastleRookSquare = (int)BoardSquare.f8;
    public const int WhiteLongCastleRookSquare = (int)BoardSquare.d1;
    public const int BlackLongCastleRookSquare = (int)BoardSquare.d8;

    public const string WhiteShortCastle = "e1g1";
    public const string BlackShortCastle = "e8g8";
    public const string WhiteLongCastle = "e1c1";
    public const string BlackLongCastle = "e8c8";

    public static readonly IReadOnlyDictionary<int, int> EnPassantCaptureSquares = new Dictionary<int, int>(16)
    {
        [(int)BoardSquare.a6] = (int)BoardSquare.a6 + 8,
        [(int)BoardSquare.b6] = (int)BoardSquare.b6 + 8,
        [(int)BoardSquare.c6] = (int)BoardSquare.c6 + 8,
        [(int)BoardSquare.d6] = (int)BoardSquare.d6 + 8,
        [(int)BoardSquare.e6] = (int)BoardSquare.e6 + 8,
        [(int)BoardSquare.f6] = (int)BoardSquare.f6 + 8,
        [(int)BoardSquare.g6] = (int)BoardSquare.g6 + 8,
        [(int)BoardSquare.h6] = (int)BoardSquare.h6 + 8,

        [(int)BoardSquare.a3] = (int)BoardSquare.a3 - 8,
        [(int)BoardSquare.b3] = (int)BoardSquare.b3 - 8,
        [(int)BoardSquare.c3] = (int)BoardSquare.c3 - 8,
        [(int)BoardSquare.d3] = (int)BoardSquare.d3 - 8,
        [(int)BoardSquare.e3] = (int)BoardSquare.e3 - 8,
        [(int)BoardSquare.f3] = (int)BoardSquare.f3 - 8,
        [(int)BoardSquare.g3] = (int)BoardSquare.g3 - 8,
        [(int)BoardSquare.h3] = (int)BoardSquare.h3 - 8,
    };

    /// <summary>
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/make_move_castling_rights/bbc.c#L1474
    ///                                 CastlingRights  Binary  Decimal
    ///  K & R didn't move              1111 & 1111  =  1111    15
    ///  White King moved               1111 & 1100  =  1100    12
    ///  White kingside Rook moved      1111 & 1110  =  1110    14
    ///  White queenside Rook moved     1111 & 1101  =  1101    13
    ///  Black King moved               1111 & 0011  =  1011    3
    ///  Black kingside Rook moved      1111 & 1011  =  1011    11
    ///  Black queenside Rook moved     1111 & 0111  =  0111    7
    /// </summary>
    public static readonly int[] CastlingRightsUpdateConstants = new int[64]
    {
            7, 15, 15, 15,  3, 15, 15, 11,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            13, 15, 15, 15, 12, 15, 15, 14
    };

    /// <summary>
    /// 218 or 224 seems to be the known limit
    /// https://www.reddit.com/r/chess/comments/9j70dc/position_with_the_most_number_of_legal_moves/
    /// </summary>
    public const int MaxNumberOfPossibleMovesInAPosition = 250;

    public static readonly int SideLimit = Enum.GetValues(typeof(Piece)).Length / 2;

    public static readonly int[] Rank = new[]
    {
        7, 7, 7, 7, 7, 7, 7, 7,
        6, 6, 6, 6, 6, 6, 6, 6,
        5, 5, 5, 5, 5, 5, 5, 5,
        4, 4, 4, 4, 4, 4, 4, 4,
        3, 3, 3, 3, 3, 3, 3, 3,
        2, 2, 2, 2, 2, 2, 2, 2,
        1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0
    };

    public static readonly int[] File = new[]
    {
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7,
        0, 1, 2, 3, 4, 5, 6, 7
    };

    public const int AbsoluteMaxDepth = 255;
}
