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
    public static readonly FrozenDictionary<char, Piece> PiecesByChar = new Dictionary<char, Piece>(12)
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
    public static readonly int[] BishopRelevantOccupancyBits =
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
    public static readonly int[] RookRelevantOccupancyBits =
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

    /// <summary>
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/init_magics/bbc.c
    /// </summary>
    public static readonly BitBoard[] RookMagicNumbers =
    [
        0x8a80104000800020UL, 0x140002000100040UL, 0x2801880a0017001UL, 0x100081001000420UL, 0x200020010080420UL, 0x3001c0002010008UL, 0x8480008002000100UL, 0x2080088004402900UL,
        0x800098204000UL, 0x2024401000200040UL, 0x100802000801000UL, 0x120800800801000UL, 0x208808088000400UL, 0x2802200800400UL, 0x2200800100020080UL, 0x801000060821100UL,
        0x80044006422000UL, 0x100808020004000UL, 0x12108a0010204200UL, 0x140848010000802UL, 0x481828014002800UL, 0x8094004002004100UL, 0x4010040010010802UL, 0x20008806104UL,
        0x100400080208000UL, 0x2040002120081000UL, 0x21200680100081UL, 0x20100080080080UL, 0x2000a00200410UL, 0x20080800400UL, 0x80088400100102UL, 0x80004600042881UL,
        0x4040008040800020UL, 0x440003000200801UL, 0x4200011004500UL, 0x188020010100100UL, 0x14800401802800UL, 0x2080040080800200UL, 0x124080204001001UL, 0x200046502000484UL,
        0x480400080088020UL, 0x1000422010034000UL, 0x30200100110040UL, 0x100021010009UL, 0x2002080100110004UL, 0x202008004008002UL, 0x20020004010100UL, 0x2048440040820001UL,
        0x101002200408200UL, 0x40802000401080UL, 0x4008142004410100UL, 0x2060820c0120200UL, 0x1001004080100UL, 0x20c020080040080UL, 0x2935610830022400UL, 0x44440041009200UL,
        0x280001040802101UL, 0x2100190040002085UL, 0x80c0084100102001UL, 0x4024081001000421UL, 0x20030a0244872UL, 0x12001008414402UL, 0x2006104900a0804UL, 0x1004081002402UL
    ];

    /// <summary>
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/init_magics/bbc.c
    /// </summary>
    public static readonly BitBoard[] BishopMagicNumbers =
    [
        0x40040844404084UL, 0x2004208a004208UL, 0x10190041080202UL, 0x108060845042010UL, 0x581104180800210UL, 0x2112080446200010UL, 0x1080820820060210UL, 0x3c0808410220200UL,
        0x4050404440404UL, 0x21001420088UL, 0x24d0080801082102UL, 0x1020a0a020400UL, 0x40308200402UL, 0x4011002100800UL, 0x401484104104005UL, 0x801010402020200UL,
        0x400210c3880100UL, 0x404022024108200UL, 0x810018200204102UL, 0x4002801a02003UL, 0x85040820080400UL, 0x810102c808880400UL, 0xe900410884800UL, 0x8002020480840102UL,
        0x220200865090201UL, 0x2010100a02021202UL, 0x152048408022401UL, 0x20080002081110UL, 0x4001001021004000UL, 0x800040400a011002UL, 0xe4004081011002UL, 0x1c004001012080UL,
        0x8004200962a00220UL, 0x8422100208500202UL, 0x2000402200300c08UL, 0x8646020080080080UL, 0x80020a0200100808UL, 0x2010004880111000UL, 0x623000a080011400UL, 0x42008c0340209202UL,
        0x209188240001000UL, 0x400408a884001800UL, 0x110400a6080400UL, 0x1840060a44020800UL, 0x90080104000041UL, 0x201011000808101UL, 0x1a2208080504f080UL, 0x8012020600211212UL,
        0x500861011240000UL, 0x180806108200800UL, 0x4000020e01040044UL, 0x300000261044000aUL, 0x802241102020002UL, 0x20906061210001UL, 0x5a84841004010310UL, 0x4010801011c04UL,
        0xa010109502200UL, 0x4a02012000UL, 0x500201010098b028UL, 0x8040002811040900UL, 0x28000010020204UL, 0x6000020202d0240UL, 0x8918844842082200UL, 0x4010011029020020UL
    ];

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

    public const int WhiteKingSourceSquare = (int)BoardSquare.e1;
    public const int BlackKingSourceSquare = (int)BoardSquare.e8;

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


    public static readonly int[] EnPassantCaptureSquares =
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
    public static readonly byte[] CastlingRightsUpdateConstants =
    [
        7, 15, 15, 15, 3, 15, 15, 11,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        13, 15, 15, 15, 12, 15, 15, 14
    ];

    /// <summary>
    /// 218 or 224 seems to be the known limit
    /// https://www.reddit.com/r/chess/comments/9j70dc/position_with_the_most_number_of_legal_moves/
    /// </summary>
    public const int MaxNumberOfPossibleMovesInAPosition = 250;

    public const int MaxNumberMovesInAGame = 1024;

    public static readonly int SideLimit = Enum.GetValues(typeof(Piece)).Length / 2;

    public static readonly int[] Rank =
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

    public static readonly int[] File =
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

    public const int AbsoluteMaxDepth = 255;

    /// <summary>
    /// From https://lichess.org/RViT3UWL
    /// Failes input parsing fail if default input buffer size is used (see Lynx.Cli.Listener
    /// </summary>
    public const string LongPositionCommand = "position startpos moves d2d4 g8f6 g1f3 d7d5 b1c3 e7e6 g2g3 c7c5 e2e3 c5c4 f1g2 e6e5 d4e5 f6e4 d1d5 e4c3 d5d8 e8d8 b2c3 b8c6 c1b2 a7a6 e1c1 d8c7 d1e1 a6a5 e3e4 a5a4 a2a3 b7b6 f3d2 b6b5 f2f4 h8g8 d2f3 c8g4 f3g5 f8c5 g5h7 g8h8 h7g5 f7f6 e5f6 g7f6 g5f3 a8b8 f3d4 g4d7 h2h4 h8g8 e1e3 b5b4 a3b4 a4a3 b2a3 c6d4 b4c5 g8g3 e3g3 d4e2 c1d2 e2g3 h1e1 d7c6 a3b4 b8h8 b4a5 c7d7 e1e3 h8g8 d2e1 f6f5 g2h3 d7e7 e4f5 g3e4 e1f1 e7f7 a5b6 g8h8 h3g2 h8h4 b6c7 e4d2 f1g1 h4g4 e3e2 d2f3 g1f1 f3h2 f1g1 h2f3 g1f2 g4g2 f2g2 f3d4 g2f2 d4e2 f2e2 f7f6 e2e3 f6f5 c7e5 f5e6 e3d4 c6f3 d4c4 f3e2 c4d4 e2b5 c3c4 b5e8 d4d3 e8g6 d3c3 e6d7 c3b3 g6e4 c2c3 d7c6 e5d6 c6d7 d6e5 d7c6 b3b4 e4b1 e5d4 b1c2 b4a5 c2f5 a5b4 f5d3 d4e3 d3b1 e3d4 b1e4 d4h8 e4g6 h8g7 g6h7 g7f8 h7e4 f8g7 e4c2 g7f8 c2h7 f8d6 h7d3 d6b8 d3h7 b8e5 h7b1 e5h8 b1e4 h8d4 e4c2 d4e3 c2f5 b4b3 f5g6 b3b4 g6c2 e3c1 c2e4 c1a3 e4g6 a3c1 g6b1 c1e3 b1d3 e3c1 d3b1 c1a3 b1c2 a3b2 c2f5 b2a3 f5d3 b4b3 d3f5 b3a4 f5c2 a4a5 c2f5 a5a4 f5c2 a4a5 c2g6 a5b4 g6c2 a3b2 c2f5 b4b3 c6c5 b2a3 c5c6 b3a4 f5c2 a4a5 c2d3 a5b4 d3f5 a3b2 f5c2 c4c5 c2d3 c3c4 d3f5 b2a3 f5h3 b4c3 h3e6 c3d4 c6c7 a3b2 c7d7 d4c3 e6g4 c3d4 d7e6 d4e3 g4h3 e3f2 h3f5 f2e3 f5h3 b2e5 e6d7 e5d6 h3f5 d6b8 d7c6 b8d6 c6d7 d6b8 d7c6 e3d4 f5e6 b8e5 e6f5 e5d6 f5e6 d6f8 c6c7 f8e7 c7c6 e7d6 c6d7 d6e5 e6h3 d4c3 h3g2 f4f5 g2e4 f5f6 e4g6 c3d4 g6f7 e5d6 d7e6 d6e7 e6d7 d4c3 f7g8 e7d6 g8f7 c3d4 d7e6 d6e5 e6d7 d4d3 d7c6 e5d6 c6d7 d6e5 f7g6 d3e3 g6f7 e3d4 f7g8 d4d3 d7c6 e5d6 c6d7 d3d4 g8f7 d6e7 f7e6 d4c3 e6g8 c3b4 d7c6 e7d6 g8f7 d6e5 f7e6 e5b2 e6f7 b2d4 f7e8 d4c3 c6b7 c3b2 b7c6 b2d4 c6c7 d4e5 c7c6 e5d4 e8f7 d4e5 f7g6 e5b2 g6f7 b2c3 f7h5 c3d4 h5e8 b4b3 e8h5 b3c3 h5e8 c3b2 e8h5 b2c3 h5g6 c3d2 g6f7 d2c3 f7g8 c3b3 g8f7 d4f2 c6d7 b3b4 d7c6 b4c3 f7e6 c3d4 c6d7 f2g3 e6f7 g3e5 d7c6 e5d6 f7e6 d6e7 e6f7 e7d6 f7g8 d6e5 g8e6 f6f7 e6f7 e5f4 f7h5 f4d6 h5g4 d6e5 g4h3 e5d6 c6b7 d6f4 h3f1 f4e5 f1e2 d4d5 e2d1 c5c6 b7c8 c4c5 d1e2 d5d6 e2f3 e5h8 f3d1 h8b2 d1f3 b2e5 f3e4 e5b2 e4h1 b2h8 h1f3 h8e5 f3g4 e5b2 g4h5 b2d4 h5g6 d4b2 g6h5 c6c7 h5f3 b2e5 f3g2 e5b2 g2f3 b2g7 f3h1 g7e5 h1g2 e5d4 g2c6 d4h8 c6f3 h8b2 f3d1 b2e5 d1e2 d6e6 e2b5 e6d6 b5e2 d6e6 e2f3 e6f5 f3b7 f5f6 b7f3 f6e6 f3c6 e6f5 c6g2 f5e6 g2c6 e6f6 c6d7 f6g5 d7h3 g5f6 h3g2 f6f5 g2b7 f5f4 c8d7 f4e3 d7c6 e3d4 b7c8 e5d6 c8b7 d4c4 b7a6 c4d4 a6b7 d6e5 c6d7 d4c4 b7a6 c4d4 a6b7 d4c4 d7c6 e5f4 b7a6 c4d4 a6c8 f4e5 c8b7 e5d6 c6d7 d6e5 d7c6 e5d6 c6d7 d6g3 d7c6 g3f4 b7c8 f4h2 c8b7 h2g3 c6d7 d4c4 d7c6 c4d4 c6d7 g3f4 d7c6 f4h2 c6d7 h2f4 d7c6 d4c4 b7a6 c4b4 a6c8 b4c3 c8a6 c7c8Q a6c8 c3d4 c8b7 d4c4 b7c8 c4d4 c8h3 f4c1 h3f1 c1a3 f1a6 a3b2 a6c8 b2a3 c8h3 d4e5 h3d7 e5d4 d7c8 a3b2 c8d7 d4c4 d7e6 c4d4 e6g4 b2a3 g4c8 a3b2 c8e6 b2c3 e6c8 d4c4 c8a6 c4d4 a6b7 d4c4 b7c8 c3b2 c8h3 b2a1 h3e6 c4d4 e6d5 a1c3 d5g2 d4c4 g2f1 c4d4 f1a6 c3d2 a6f1 d2f4 f1h3 f4h6 h3f1 h6g5 f1h3 d4c4 h3f1 c4d4 f1h3 g5f4 h3f1 f4e5 f1e2 e5d6 e2b5 d6e5 b5f1 e5b8 f1b5 b8d6 b5f1 d6e5 f1a6 e5f4 a6e2 f4e5 e2g4 e5d6 g4e6 d6e5 e6h3 e5f4 h3g2 f4d2 g2d5 d2f4 d5f7 f4e5 f7a2 e5d6 a2g8 d6e7 g8e6 e7d6 e6d7 d4e5 d7h3 e5f6 h3f1";

    /// <summary>
    /// 500 moves, https://www.chess.com/game/live/378106991
    /// </summary>
    public const string SuperLongPositionCommand = "position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 moves e2e4 e7e5 g1f3 g8f6 b1c3 b8c6 f1e2 f8b4 a2a3 b4c3 b2c3 f6e4 e2b5 d7d6 d1e2 e4f6 d2d4 e8g8 d4e5 d6e5 b5c6 b7c6 c1g5 h7h6 g5h4 f8e8 a1d1 d8e7 e1g1 c8g4 h2h3 g4f3 e2f3 e7e6 h4f6 e6f6 f3f6 g7f6 d1d7 a8c8 f1b1 e8d8 d7d3 e5e4 d3d4 d8d4 c3d4 c8d8 c2c3 d8d7 f2f3 e4e3 g1f1 f6f5 f3f4 d7e7 f1e2 h6h5 h3h4 g8g7 b1f1 e7e4 g2g3 g7f6 f1f3 f6e6 f3e3 e4e3 e2e3 e6d5 e3d3 c6c5 c3c4 d5c6 d4d5 c6d6 d3c3 c7c6 d5c6 d6c6 c3b3 c6b6 a3a4 a7a5 b3a3 f7f6 a3b3 b6c6 b3c3 c6d6 c3d3 d6c6 d3e3 c6d6 e3f3 d6e6 f3e3 e6d6 e3f3 d6e6 f3e3 e6d6 e3f2 d6e6 f2e2 e6d6 e2d3 d6e6 d3c3 e6d6 c3b3 d6e6 b3a3 e6d6 a3b3 d6e6 b3c2 e6d6 c2c3 d6e6 c3b2 e6d6 b2c2 d6e6 c2c3 e6e7 c3d3 e7d6 d3d2 d6e6 d2c3 e6e7 c3c2 e7d6 c2d3 d6e6 d3e2 e6d6 e2f3 d6e6 f3e3 e6d6 e3f2 d6e6 f2f3 e6d6 f3g2 d6e6 g2f1 e6d6 f1e2 d6e6 e2d2 e6d6 d2d3 d6e7 d3c2 e7e6 c2c3 e6d6 c3b3 d6d7 b3c2 d7e6 c2d3 e6d6 d3e2 d6e6 e2d2 e6d6 d2d1 d6e6 d1c1 e6d6 c1b1 d6e6 b1a2 e6d6 a2b2 d6e6 b2c3 e6d6 c3d2 d6e6 d2e1 e6d6 e1d1 d6e6 d1e2 e6d6 e2f1 d6e6 f1g1 e6d6 g1h1 d6e6 h1g1 e6d6 g1f1 d6e6 f1e1 e6d6 e1d1 d6e6 d1c1 e6d6 c1b1 d6e6 b1a1 e6d6 a1a2 d6e6 a2b2 e6d6 b2c2 d6e6 c2d2 e6d6 d2e2 d6e6 e2f2 e6d6 f2f3 d6e6 f3e3 e6d6 e3d3 d6e6 d3c3 e6d6 c3b3 d6e6 b3a3 e6f7 a3b2 f7g6 b2c3 g6h6 c3d3 h6g6 d3e3 g6h6 e3f3 h6g6 f3g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2f2 g6h6 f2e3 h6g6 e3d3 g6h6 d3c3 h6g6 c3c2 g6h6 c2d2 h6g6 d2d3 g6h6 d3e3 h6g6 e3e2 g6h6 e2f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f1 h6g6 f1e2 g6h6 e2e1 h6g6 e1d2 g6h6 d2c1 h6g6 c1c2 g6h6 c2d1 h6g6 d1e2 g6h6 e2f1 h6g6 f1f2 g6h6 f2f3 h6g6 f3e3 g6h6 e3d3 h6g6 d3c3 g6h6 c3b3 h6g6 b3a3 g6h6 a3b2 h6g6 b2a2 g6h6 a2a1 h6g6 a1b2 g6h6 b2c1 h6g6 c1c2 g6h6 c2c3 h6g6 c3b3 g6h6 b3b2 h6g6 b2a3 g6h6 a3b3 h6g6 b3c3 g6h6 c3d3 h6g6 d3e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h2 g6h6 h2g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2g1 h6g6 g1f2 g6h6 f2f1 h6g6 f1e2 g6h6 e2e3 h6g6 e3d2 g6h6 d2d3 h6g6 d3e3 g6h6 e3e2 h6g6 e2f3 g6h6 f3f2 h6g6 f2e2 g6h6 e2e3 h6g6 e3f3 g6h6 f3f2 h6g6 f2g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2g1 h6g6 g1h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3d3 g6h6 d3d2 h6g6 d2c3 g6h6 c3c2 h6g6 c2d1 g6h6 d1d2 h6g6 d2e1 g6h6 e1e2 h6g6 e2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h1 h6g6 h1g1 g6h6 g1f1 h6g6 f1e1 g6h6 e1d1 h6g6 d1c2 g6h6 c2c3 h6g6 c3d3 g6h6 d3d2 h6g6 d2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f1 g6h6 f1e2 h6g6 e2f3 g6h6 f3e3 h6g6 e3f2 g6h6 f2f1 h6g6 f1g2 g6h6 g2h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g2 h6g6 g2f2 g6h6 f2f3 h6g6 f3e2 g6h6 e2e3 h6g6 e3d3 g6h6 d3c3 h6g6 c3c2 g6h6 c2c3 h6g6 c3c2 g6h6 c2d2 h6g6 d2d3 g6h6 d3e2 h6g6 e2e3 g6h6 e3e2 h6g6 e2f3 g6h6 f3f2 h6g6 f2f1 g6h6 f1g2 h6g6 g2g1 g6h6 g1h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1h2 g6h6 h2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2f1 g6h6 f1e2 h6g6 e2e3 g6h6 e3f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h2 g6h6 h2h3 h6g6 h3g2 g6h6 g2h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3f2 h6g6 f2g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1g1 g6h6 g1f2 h6g6 f2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2f3 h6g6 f3f2 g6h6 f2g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2g1 h6g6 g1h1 g6h6 h1h2 h6g6 h2h3 g6h6 h3g2 h6g6 g2f3 g6h6 f3f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2g1 g6h6 g1g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2h3 h6g6 h3h2 g6h6 h2g2 h6g6 g2g1 g6h6 g1h1 h6g6 h1g2 g6h6 g2h2 h6g6 h2g1 g6h6 g1h1 h6g6 h1h2 g6h6 h2g2 h6g6 g2f2 g6h6 f2f3 h6g6 f3e2 g6h6 e2e1 h6g6 e1f2 g6h6 f2g2 h6g6 g2g1 g6h6 g1f2 h6g6 f2f3 g6h6 f3e3 h6g6 e3e2 g6h6 e2e3 h6g6 e3f3 g6h6 f3f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2h3 g6h6 h3h2 h6g6 h2h1 g6h6 h1g1 h6g6 g1g2 g6h6 g2f2 h6g6 f2f3 g6h6 f3f2 h6g6 f2f1 g6h6 f1f2 h6g6 f2f3 g6h6 f3g2 h6g6 g2h3 g6h6 h3h2 h6g6 h2g2 g6h6 g2h1 h6g6 h1h2 g6h6 h2g1 h6g6 g1g2 g6h6 g2h3 h6g6 h3h2 g6h6 h2h1 h6g6 h1g2 g6h6 g2f2 h6h7 f2f3 h7g7 f3g2 g7g6 g2h3 g6h6 h3h2 h6h7 h2g1 h7h8 g1h1 h8g8 h1g2 g8g7 g2f2 g7g6 f2f1 g6h6 f1e2 h6h7 e2f3 h7h8 f3f2 h8g8 f2f1 g8g7 f1g2 g7g6 g2h2 g6h6 h2h3 h6h7 h3h2 h7h8 h2h1 h8g8 h1g1 g8g7 g1g2 g7g6 g2h2 g6h6 h2g1 h6h7 g1g2 h7h8 g2h1 h8g8 h1h2 g8g7 h2g2 g7g6 g2g1 g6h6 g1g2 h6h7 g2h2 h7h8 h2h3 h8g8 h3h2 g8g7 h2h1 g7g6 h1g1 g6h6 g1g2 h6h7 g2f2 h7h8 f2f3 h8g8 f3g2 g8g7 g2h3 g7g6 h3h2 g6h6 h2h1 h6h7 h1g2 h7h6 g2g1 h6g6 g1g2 g6h6 g2g1 h6h7 g1g2 h7g7 g2g1 g7g6 g1g2 g6h6 g2g1 h6h7 g1g2 h7g7 g2g1 g7h6 g1g2 h6h7 g2g1 h7h8 g1g2 h8g8 g2g1 g8f7 g1g2 f7g6 g2g1 g6h6 g1g2 h6h7 g2g1 h7h8 g1g2 h8g8 g2g1 g8f7 g1g2 f7g6 g2g1 g6g7 g1g2 g7f8 g2g1 f8f7 g1g2 f7g8 g2g1 g8g7 g1g2 g7h8 g2g1 h8h7 g1g2 h7h6 g2g1 h6g7";
}

#pragma warning restore IDE0055
