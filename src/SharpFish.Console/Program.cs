using SharpFish;
using SharpFish.Internal;
using SharpFish.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using static SharpFish.Model.Move;

//_2_GettingStarted();
//_3_PawnAttacks();
//_4_KnightAttacks();
//_5_KingAttacks();
//_6_Bishop_Occupancy();
//_7_Rook_Occupancy();
//_8_Slider_Pieces_Attacks();
//_9_BitCount();
//_10_SetOccupancy();
//_11_OccupancyBitCountLookupTables();
//_13_GeneratingMagicNumbersCandidates();
//_14_GeneratingMagicNumbersByBruteForce();
//_16_InitializingSliderPiecesAttackTables();
//_17_Defining_variables();
//_18_Printing_Chess_Board();
//_19_Parse_FEN();
//_20_QueenAttacks();
//_21_IsSqureAttacked();
//_22_Generate_Moves();
//_23_Castling_Moves();
//_26_Piece_Moves();
//_27_Move_Encoding();
//_29_Move_List();
//_32_Make_Move();
_42_Perft();


const string TrickyPosition = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
const string TrickyPositionReversed = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1";
const string KillerPosition = "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1";
const string CmkPosition = "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9 ";

static void _2_GettingStarted()
{
    var board = new BitBoard(4UL);
    board.SetBit(BoardSquares.e4);
    board.Print();
    board.PopBit(BoardSquares.e4);
    board.Print();
    board.PopBit(BoardSquares.e4);
    board.Print();

    Console.ReadKey();
}

static void _3_PawnAttacks()
{
    var whiteAttacks = AttacksGenerator.MaskPawnAttacks((int)BoardSquares.e4, isWhite: true);
    var blackAttacks = AttacksGenerator.MaskPawnAttacks((int)BoardSquares.e4, isWhite: false);

    whiteAttacks.Print();
    blackAttacks.Print();

    AttacksGenerator.InitializePawnAttacks();
}

static void _4_KnightAttacks()
{
    var attacks = AttacksGenerator.MaskKnightAttacks((int)BoardSquares.e4);
    attacks.Print();

    attacks = AttacksGenerator.MaskKnightAttacks((int)BoardSquares.h4);
    attacks.Print();

    attacks = AttacksGenerator.MaskKnightAttacks((int)BoardSquares.a4);
    attacks.Print();

    AttacksGenerator.InitializeKnightAttacks();
}

static void _5_KingAttacks()
{
    var attacks = AttacksGenerator.MaskKingAttacks((int)BoardSquares.e4);
    attacks.Print();

    attacks = AttacksGenerator.MaskKingAttacks((int)BoardSquares.h4);
    attacks.Print();

    attacks = AttacksGenerator.MaskKingAttacks((int)BoardSquares.a4);
    attacks.Print();

    AttacksGenerator.InitializeKingAttacks();
}

static void _6_Bishop_Occupancy()
{
    var occupancy = AttacksGenerator.MaskBishopOccupancy((int)BoardSquares.h8);
    occupancy.Print();

    AttacksGenerator.InitializeBishopOccupancy();
}

static void _7_Rook_Occupancy()
{
    var occupancy = AttacksGenerator.MaskRookOccupancy((int)BoardSquares.e4);
    occupancy.Print();

    AttacksGenerator.InitializeRookOccupancy();
}

static void _8_Slider_Pieces_Attacks()
{
    // Occupancy bitboard
    BitBoard block = new();

    block.SetBit(BoardSquares.b6);
    block.SetBit(BoardSquares.g7);
    block.SetBit(BoardSquares.f2);
    block.SetBit(BoardSquares.b2);

    var bishopAttacks = AttacksGenerator.GenerateBishopAttacksOnTheFly((int)BoardSquares.d4, block);

    block.Print();
    bishopAttacks.Print();

    block = new BitBoard(new[] { BoardSquares.d3, BoardSquares.b4, BoardSquares.d7, BoardSquares.h4 });

    var rookAttacks = AttacksGenerator.GenerateRookAttacksOnTheFly((int)BoardSquares.d4, block);
    block.Print();
    rookAttacks.Print();
}

static void _9_BitCount()
{
    BitBoard bitBoard = new(new[] { BoardSquares.d5, BoardSquares.e4 });

    bitBoard.ResetLS1B();
    bitBoard.Print();

    var bb = new BitBoard(BoardSquares.d5, BoardSquares.e4);

    Console.WriteLine(bb.GetLS1BIndex());
    Console.WriteLine(bb.GetLS1BIndex());
    Console.WriteLine(Constants.Coordinates[bb.GetLS1BIndex()]);
}

static void _10_SetOccupancy()
{
    // Mask piece attacks at given square
    var occupancyMask = AttacksGenerator.MaskRookOccupancy((int)BoardSquares.a5);

    var occupancy = AttacksGenerator.SetBishopOrRookOccupancy(4095, occupancyMask);
    occupancy.Print();

    var ind = (int)Math.Pow(2, 6) - 1;
    occupancy = AttacksGenerator.SetBishopOrRookOccupancy(ind, occupancyMask);
    occupancy.Print();

    // Loop over occupancy indexes
    for (int index = 0; index < 4096; ++index)
    {
        occupancy = AttacksGenerator.SetBishopOrRookOccupancy(index, occupancyMask);
        occupancy.Print();
        Console.ReadKey();
    }

    occupancyMask = AttacksGenerator.MaskBishopOccupancy((int)BoardSquares.d4);
    occupancyMask.Print();

    // Loop over occupancy indexes
    for (int index = 0; index < 64; ++index)
    {
        occupancy = AttacksGenerator.SetBishopOrRookOccupancy(index, occupancyMask);
        occupancy.Print();
        Console.ReadKey();
    }
}

static void _11_OccupancyBitCountLookupTables()
{
    for (var rank = 0; rank < 8; ++rank)
    {
        for (var file = 0; file < 8; ++file)
        {
            int square = BitBoard.SquareIndex(rank, file);

            var bishopOccupancy = AttacksGenerator.MaskBishopOccupancy(square);
            Console.Write($"{bishopOccupancy.CountBits()}, ");
        }

        Console.WriteLine();
    }

    for (var rank = 0; rank < 8; ++rank)
    {
        for (var file = 0; file < 8; ++file)
        {
            int square = BitBoard.SquareIndex(rank, file);

            var bishopOccupancy = AttacksGenerator.MaskRookOccupancy(square);
            Console.Write($"{bishopOccupancy.CountBits()}, ");
        }

        Console.WriteLine();
    }
}

static void _13_GeneratingMagicNumbersCandidates()
{
    const int randomlyGeneratedSeed = 1160218972;
    var generator = new Random(randomlyGeneratedSeed);

    var randomNumber = (ulong)generator.Next();
    // Slice upper (from MS1b side) 16 bits
    randomNumber &= 0xFFFF;

    // int(uint really), which is 32 bits -> ulong, 64 bits leaves the seconf half of the board empty
    // The slicing leaves the second quarter empty as well
    var randomBoard = new BitBoard(randomNumber);
    randomBoard.Print();

    var candidate = MagicNumberGenerator.GenerateMagicNumber();

    var board = new BitBoard(candidate);
    board.Print();
}

static void _14_GeneratingMagicNumbersByBruteForce()
{
    MagicNumberGenerator.InitializeMagicNumbers();

    // Should generate something similar to Constants.RookMagicNumbers
}

static void _16_InitializingSliderPiecesAttackTables()
{
    var occupancy = new BitBoard();
    occupancy.SetBit(BoardSquares.e5);
    occupancy.SetBit(BoardSquares.d5);
    occupancy.SetBit(BoardSquares.d8);
    occupancy.Print();

    var bishopAttacks = Attacks.BishopAttacks((int)BoardSquares.d4, occupancy);
    bishopAttacks.Print();

    var rookAttacks = Attacks.RookAttacks((int)BoardSquares.d4, occupancy);
    rookAttacks.Print();
}

static void _17_Defining_variables()
{
    var position = new Position(Constants.EmptyBoardFEN);

    var whitePawnBitBoard = position.PieceBitBoards[(int)Piece.P];
    whitePawnBitBoard.SetBit(BoardSquares.e2);
    whitePawnBitBoard.Print();

    Console.WriteLine($"Piece: {Constants.PiecesByChar['K']}");

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        Console.WriteLine($"Piece: {Constants.UnicodePieces[(int)Piece.K]}");
        // Simulating from FEN
        Console.WriteLine($"Piece: {Constants.UnicodePieces[(int)Constants.PiecesByChar['K']]}");
    }
    else
    {
        Console.WriteLine($"Piece: {Constants.AsciiPieces[(int)Piece.K]}");
        // Simulating from FEN
        Console.WriteLine($"Piece: {Constants.AsciiPieces[(int)Constants.PiecesByChar['K']]}");
    }
}

static void _18_Printing_Chess_Board()
{
    var position = new Position(Constants.InitialPositionFEN);
    position.Print();

    for (int bbIndex = 0; bbIndex < position.PieceBitBoards.Length; ++bbIndex)
    {
        position.PieceBitBoards[bbIndex].Print();
    }
}

static void _19_Parse_FEN()
{
    const string emptyBoard = "8/8/8/8/8/8/8/8 w - - 0 1";
    const string startPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    var position = new Position(CmkPosition);

    position.PieceBitBoards[(int)Piece.Q].Print();

    position.Print();
    position.OccupancyBitBoards[(int)Side.White].Print();
    position.OccupancyBitBoards[(int)Side.Black].Print();
    position.OccupancyBitBoards[(int)Side.Both].Print();
}

static void _20_QueenAttacks()
{
    var position = new Position(Constants.InitialPositionFEN);
    Attacks.QueenAttacks((int)BoardSquares.e4, position.OccupancyBitBoards[(int)Side.Both]).Print();
}

static void _21_IsSquareAttacked()
{
    var position = new Position("8/8/8/3p4/8/8/8/8 w - - 0 1");

    position.PieceBitBoards[(int)Piece.p].Print();

    Attacks.PawnAttacks[(int)Side.White, (int)BoardSquares.e4].Print();

    var and = new BitBoard(
        position.PieceBitBoards[(int)Piece.p].Board & Attacks.PawnAttacks[(int)Side.White,
        (int)BoardSquares.e4].Board);
    and.Print();

    Console.WriteLine("=====================================");

    position = new Position(Constants.EmptyBoardFEN);
    position.PieceBitBoards[(int)Piece.n].SetBit(BoardSquares.c6);
    position.PieceBitBoards[(int)Piece.n].SetBit(BoardSquares.f6);

    position.PrintAttackedSquares(Side.Black);

    Console.WriteLine(Attacks.IsSquaredAttacked((int)BoardSquares.e4, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));

    Console.WriteLine("=====================================");

    position = new Position(Constants.EmptyBoardFEN);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.b7);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.d7);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.f7);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.h7);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.b3);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.d3);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.f3);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.h3);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.d1);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.c4);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.g4);

    position.PrintAttackedSquares(Side.Black);

    Console.WriteLine("=====================================");

    position = new Position(Constants.EmptyBoardFEN);
    position.PieceBitBoards[(int)Piece.K].SetBit(BoardSquares.e4);
    position.PrintAttackedSquares(Side.White);

    Console.WriteLine("=====================================");

    position = new Position(Constants.InitialPositionFEN);
    position.PrintAttackedSquares(Side.White);
    position.PrintAttackedSquares(Side.Black);
}

static void _22_Generate_Moves()
{
    var position = new Position("r1P1k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1 ");
    position.Print();

    //position.OccupancyBitBoards[2].Board |= 0b11100111UL << 8 * 4;
    var moves = MovesGenerator.GenerateAllMoves(position);
    foreach (var move in moves)
    {
        Console.WriteLine(move);
    }

    position = new Position("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1");
    position.PieceBitBoards[0].Print();
    position.PieceBitBoards[6].Print();
    position.Print();
    moves = MovesGenerator.GenerateAllMoves(position);

    foreach (var move in moves)
    {
        Console.WriteLine(move);
    }
}

static void _23_Castling_Moves()
{
    var position = new Position("rn2k2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w KQkq - 0 1");
    position.Print();

    var moves = MovesGenerator.GenerateCastlingMoves(position, Utils.PieceOffset(position.Side)).ToList();

    foreach (var move in moves)
    {
        Console.WriteLine(move);
    }
}

static void _26_Piece_Moves()
{
    var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
    position.Print();

    var moves = MovesGenerator.GenerateKnightMoves(position).ToList();

    moves = MovesGenerator.GenerateAllMoves(position).ToList();
    Console.WriteLine($"Expected 48, found: {moves.Count}");
    foreach (var move in moves)
    {
        Console.WriteLine(move);
    }
}

static void _28_Move_Encoding()
{
    int move = 0;

    // Target square: h1 (63)
    var targetSquare = BoardSquares.h1;

    // Encode move
    move = (move | (int)targetSquare) << 6;
    Console.WriteLine($"{Convert.ToString(move, 2)}");

    // Decode move
    int square = (move & 0xFC0) >> 6;
    Console.WriteLine(Constants.Coordinates[square]);
}

static void _29_Move_List()
{
    var position = new Position(TrickyPosition);
    var moves = MovesGenerator.GenerateAllMoves(position);
    PrintMoveList(moves);

    position = new Position(TrickyPositionReversed);
    moves = MovesGenerator.GenerateAllMoves(position);
    PrintMoveList(moves);

    position = new Position(KillerPosition);
    position.Print();
    moves = MovesGenerator.GenerateAllMoves(position);
    PrintMoveList(moves);
}

static void _32_Make_Move()
{
    // Arrange
    var position = new Position("r3k2r/1r6/8/3B4/8/8/8/R3K2R w KQkq - 0 1");
    position.Print();

    position = new Position("r3k2r/8/8/3b4/8/8/6R1/R3K2R b KQkq - 0 1");
    position.Print();

    var game = new Game(TrickyPosition);
    var reversedGame = new Game(TrickyPositionReversed);
    var gameWithPromotion = new Game("r3k2r/pPppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
    var reversedGameWithPromotionAndCapture = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PpPBBPPP/R3K2R w KQkq - 0 1");
    //GeneralMoveTest(game);
    //CastlingRightsTest(game);
    //CastlingRightsTest(reversedGame);

    PrintMoveList(gameWithPromotion.GetAllMoves());

    GeneralMoveTest(gameWithPromotion);

    static void GeneralMoveTest(Game game)
    {

        foreach (var move in game.GetAllMoves())
        {
            game.CurrentPosition.Print();

            move.Print();
            game.MakeMove(move);
            game.CurrentPosition.Print();

            Console.WriteLine("White occupancy:");
            game.CurrentPosition.OccupancyBitBoards[(int)Side.White].Print();

            Console.WriteLine("Black occupancy:");
            game.CurrentPosition.OccupancyBitBoards[(int)Side.Black].Print();

            game.RevertLastMove();
        }
    }

    static void CastlingRightsTest(Game game)
    {
        foreach (var move in game.GetAllMoves())
        {
            if (move.Piece() == (int)Piece.R || (move.Piece() == (int)Piece.r)
             || move.Piece() == (int)Piece.K || (move.Piece() == (int)Piece.k))
            {
                game.CurrentPosition.Print();

                move.Print();
                game.MakeMove(move);
                game.CurrentPosition.Print();

                game.RevertLastMove();
            }
        }
    }
}

static void _42_Perft()
{
    // Leaf nodes (number of positions) reached dring the test if the move generator at a given depth

    var pos = new Position(TrickyPosition);

    for (int depth = 0; depth < 7; ++depth)
    {
        var sw = new Stopwatch();
        sw.Start();
        var nodes = MovesGenerator.Perft(pos, depth);
        sw.Stop();

        Console.WriteLine($"Depth {depth}\tNodes: {nodes}\tTime: {sw.ElapsedMilliseconds}ms");
    }
}