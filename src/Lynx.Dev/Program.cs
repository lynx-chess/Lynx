using Lynx;
using Lynx.Internal;
using Lynx.Model;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
//_42_Perft();
//_43_Perft();
//_44_ParseUCI();
//_49_Rudimetary_Evaluation();
//_50_MiniMax_AlphaBeta();
//_52_Quiescence_Search();
//_53_MVVLVA();
//_54_ScoreMove();
//ZobristTable();
//PV_Table();
//CountBits();
//GetLS1BIndex();
//FileAndRankMasks();
EnhancedPawnEvaluation();

#pragma warning disable IDE0059 // Unnecessary assignment of a value
const string TrickyPosition = Constants.TrickyTestPositionFEN;
const string TrickyPositionReversed = Constants.TrickyTestPositionReversedFEN;
const string KillerPosition = Constants.KillerTestPositionFEN;
const string CmkPosition = Constants.CmkTestPositionFEN;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

static void _2_GettingStarted()
{
    var board = 4UL;
    board.SetBit(BoardSquare.e4);
    board.Print();
    board.PopBit(BoardSquare.e4);
    board.Print();
    board.PopBit(BoardSquare.e4);
    board.Print();

    Console.ReadKey();
}

static void _3_PawnAttacks()
{
    var whiteAttacks = AttackGenerator.MaskPawnAttacks((int)BoardSquare.e4, isWhite: true);
    var blackAttacks = AttackGenerator.MaskPawnAttacks((int)BoardSquare.e4, isWhite: false);

    whiteAttacks.Print();
    blackAttacks.Print();

    AttackGenerator.InitializePawnAttacks();
}

static void _4_KnightAttacks()
{
    var attacks = AttackGenerator.MaskKnightAttacks((int)BoardSquare.e4);
    attacks.Print();

    attacks = AttackGenerator.MaskKnightAttacks((int)BoardSquare.h4);
    attacks.Print();

    attacks = AttackGenerator.MaskKnightAttacks((int)BoardSquare.a4);
    attacks.Print();

    AttackGenerator.InitializeKnightAttacks();
}

static void _5_KingAttacks()
{
    var attacks = AttackGenerator.MaskKingAttacks((int)BoardSquare.e4);
    attacks.Print();

    attacks = AttackGenerator.MaskKingAttacks((int)BoardSquare.h4);
    attacks.Print();

    attacks = AttackGenerator.MaskKingAttacks((int)BoardSquare.a4);
    attacks.Print();

    AttackGenerator.InitializeKingAttacks();
}

static void _6_Bishop_Occupancy()
{
    var occupancy = AttackGenerator.MaskBishopOccupancy((int)BoardSquare.h8);
    occupancy.Print();

    AttackGenerator.InitializeBishopOccupancy();
}

static void _7_Rook_Occupancy()
{
    var occupancy = AttackGenerator.MaskRookOccupancy((int)BoardSquare.e4);
    occupancy.Print();

    AttackGenerator.InitializeRookOccupancy();
}

static void _8_Slider_Pieces_Attacks()
{
    // Occupancy bitboard
    BitBoard block = default;

    block.SetBit(BoardSquare.b6);
    block.SetBit(BoardSquare.g7);
    block.SetBit(BoardSquare.f2);
    block.SetBit(BoardSquare.b2);

    var bishopAttacks = AttackGenerator.GenerateBishopAttacksOnTheFly((int)BoardSquare.d4, block);

    block.Print();
    bishopAttacks.Print();

    block = BitBoardExtensions.Initialize(new[] { BoardSquare.d3, BoardSquare.b4, BoardSquare.d7, BoardSquare.h4 });

    var rookAttacks = AttackGenerator.GenerateRookAttacksOnTheFly((int)BoardSquare.d4, block);
    block.Print();
    rookAttacks.Print();
}

static void _9_BitCount()
{
    BitBoard bitBoard = BitBoardExtensions.Initialize(new[] { BoardSquare.d5, BoardSquare.e4 });

    bitBoard.ResetLS1B();
    bitBoard.Print();

    var bb = BitBoardExtensions.Initialize(BoardSquare.d5, BoardSquare.e4);

    Console.WriteLine(bb.GetLS1BIndex());
    Console.WriteLine(bb.GetLS1BIndex());
    Console.WriteLine(Constants.Coordinates[bb.GetLS1BIndex()]);
}

static void _10_SetOccupancy()
{
    // Mask piece attacks at given square
    var occupancyMask = AttackGenerator.MaskRookOccupancy((int)BoardSquare.a5);

    var occupancy = AttackGenerator.SetBishopOrRookOccupancy(4095, occupancyMask);
    occupancy.Print();

    var ind = (int)Math.Pow(2, 6) - 1;
    occupancy = AttackGenerator.SetBishopOrRookOccupancy(ind, occupancyMask);
    occupancy.Print();

    // Loop over occupancy indexes
    for (int index = 0; index < 4096; ++index)
    {
        occupancy = AttackGenerator.SetBishopOrRookOccupancy(index, occupancyMask);
        occupancy.Print();
        Console.ReadKey();
    }

    occupancyMask = AttackGenerator.MaskBishopOccupancy((int)BoardSquare.d4);
    occupancyMask.Print();

    // Loop over occupancy indexes
    for (int index = 0; index < 64; ++index)
    {
        occupancy = AttackGenerator.SetBishopOrRookOccupancy(index, occupancyMask);
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
            int square = BitBoardExtensions.SquareIndex(rank, file);

            var bishopOccupancy = AttackGenerator.MaskBishopOccupancy(square);
            Console.Write($"{bishopOccupancy.CountBits()}, ");
        }

        Console.WriteLine();
    }

    for (var rank = 0; rank < 8; ++rank)
    {
        for (var file = 0; file < 8; ++file)
        {
            int square = BitBoardExtensions.SquareIndex(rank, file);

            var bishopOccupancy = AttackGenerator.MaskRookOccupancy(square);
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
    var randomBoard = randomNumber;
    randomBoard.Print();

    var candidate = MagicNumberGenerator.GenerateMagicNumber();

    var board = candidate;
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
    occupancy.SetBit(BoardSquare.e5);
    occupancy.SetBit(BoardSquare.d5);
    occupancy.SetBit(BoardSquare.d8);
    occupancy.Print();

    var bishopAttacks = Attacks.BishopAttacks((int)BoardSquare.d4, occupancy);
    bishopAttacks.Print();

    var rookAttacks = Attacks.RookAttacks((int)BoardSquare.d4, occupancy);
    rookAttacks.Print();
}

static void _17_Defining_variables()
{
    var position = new Position(Constants.EmptyBoardFEN);

    var whitePawnBitBoard = position.PieceBitBoards[(int)Piece.P];
    whitePawnBitBoard.SetBit(BoardSquare.e2);
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
    Attacks.QueenAttacks((int)BoardSquare.e4, position.OccupancyBitBoards[(int)Side.Both]).Print();
}

static void _21_IsSquareAttacked()
{
    var position = new Position("8/8/8/3p4/8/8/8/8 w - - 0 1");

    position.PieceBitBoards[(int)Piece.p].Print();

    Attacks.PawnAttacks[(int)Side.White, (int)BoardSquare.e4].Print();

    var and =
        position.PieceBitBoards[(int)Piece.p]
        & Attacks.PawnAttacks[(int)Side.White, (int)BoardSquare.e4];
    and.Print();

    Console.WriteLine("=====================================");

    position = new Position(Constants.EmptyBoardFEN);
    position.PieceBitBoards[(int)Piece.n].SetBit(BoardSquare.c6);
    position.PieceBitBoards[(int)Piece.n].SetBit(BoardSquare.f6);

    position.PrintAttackedSquares(Side.Black);

    Console.WriteLine(Attacks.IsSquaredAttacked((int)BoardSquare.e4, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));

    Console.WriteLine("=====================================");

    position = new Position(Constants.EmptyBoardFEN);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.b7);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d7);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.f7);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.h7);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.b3);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d3);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.f3);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.h3);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d1);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.c4);
    position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.g4);

    position.PrintAttackedSquares(Side.Black);

    Console.WriteLine("=====================================");

    position = new Position(Constants.EmptyBoardFEN);
    position.PieceBitBoards[(int)Piece.K].SetBit(BoardSquare.e4);
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

    //position.OccupancyBitBoards[2] |= 0b11100111UL << 8 * 4;
    var moves = MoveGenerator.GenerateAllMoves(position);

    foreach (var move in moves)
    {
        Console.WriteLine(move);
    }

    position = new Position(KillerPosition);
    position.PieceBitBoards[0].Print();
    position.PieceBitBoards[6].Print();
    position.Print();
    moves = MoveGenerator.GenerateAllMoves(position);

    foreach (var move in moves)
    {
        Console.WriteLine(move);
    }
}

static void _23_Castling_Moves()
{
    var position = new Position("rn2k2r/pppppppp/8/8/8/8/PPPPPPPP/RN2K2R w KQkq - 0 1");
    position.Print();

    var moves = MoveGenerator.GenerateCastlingMovesForReference(position, Utils.PieceOffset(position.Side)).ToList();

    foreach (var move in moves)
    {
        Console.WriteLine(move);
    }
}

static void _26_Piece_Moves()
{
    var position = new Position(TrickyPosition);
    position.Print();

    var moves = MoveGenerator.GenerateKnightMoves(position).ToList();
    moves.ForEach(m => Console.WriteLine(m));

    moves = MoveGenerator.GenerateAllMoves(position).ToList();
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
    const BoardSquare targetSquare = BoardSquare.h1;

    // Encode move
#pragma warning disable S2437 // Silly bit operations should not be performed - ¬¬
    move = (move | (int)targetSquare) << 6;
#pragma warning restore S2437 // Silly bit operations should not be performed
    Console.WriteLine($"{Convert.ToString(move, 2)}");

    // Decode move
    int square = (move & 0xFC0) >> 6;
    Console.WriteLine(Constants.Coordinates[square]);
}

static void _29_Move_List()
{
    var position = new Position(TrickyPosition);
    var moves = MoveGenerator.GenerateAllMoves(position);
    moves.PrintMoveList();

    position = new Position(TrickyPositionReversed);
    moves = MoveGenerator.GenerateAllMoves(position);
    moves.PrintMoveList();

    position = new Position(KillerPosition);
    position.Print();
    moves = MoveGenerator.GenerateAllMoves(position);
    moves.PrintMoveList();
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
    //GeneralMoveTest(game);
    //CastlingRightsTest(game);
    //CastlingRightsTest(reversedGame);

    MoveGenerator.GenerateAllMoves(gameWithPromotion.CurrentPosition).PrintMoveList();

    GeneralMoveTest(gameWithPromotion);

    static void GeneralMoveTest(Game game)
    {
        foreach (var move in MoveGenerator.GenerateAllMoves(game.CurrentPosition))
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
        foreach (var move in MoveGenerator.GenerateAllMoves(game.CurrentPosition))
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
        var nodes = Perft.Results(pos, depth);
        sw.Stop();

        Console.WriteLine($"Depth {depth}\tNodes: {nodes}\tTime: {sw.ElapsedMilliseconds}ms");
    }
}

static void _43_Perft()
{
    Perft.Divide(new Position(Constants.InitialPositionFEN), 5);
    Perft.Divide(new Position(TrickyPosition), 5);
}

static void _44_ParseUCI()
{
    var position = new Position("r1b1k2r/pPppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q2/P1PB1PpP/R3KB1R w KQkq - 0 1");
    position.Print();
}

static void _49_Rudimetary_Evaluation()
{
    //var position = new Position(Constants.InitialPositionFEN);

    //foreach (var move in MoveGenerator.GenerateAllMoves(position))
    //{
    //    var newBlackPosition = new Position(position, move);
    //    if (newBlackPosition.IsValid())
    //    {
    //        var newWhitePosition = new Position(newBlackPosition, newBlackPosition.AllPossibleMoves()[0]);
    //        if (newBlackPosition.IsValid())
    //        {
    //            Console.WriteLine($"{move} | {newWhitePosition.EvaluateMaterial()} | {newWhitePosition.EvaluateMaterialAndPosition_MiniMax()}");
    //        }
    //    }
    //}
}

static void _50_MiniMax_AlphaBeta()
{
    //const string fen = "7k/6b1/7p/2p1pPpP/2P3P1/5P2/7N/7K w - - 0 1";

    //{
    //    var game = new Game(fen);
    //    var (evaluation, moveList) = SearchAlgorithms.MiniMax(game.CurrentPosition, Configuration.EngineSettings.Depth);
    //    Console.WriteLine($"Evaluation: {evaluation}");

    //    var bestMove = moveList!.Moves.Last();
    //    Console.WriteLine($"Best move: {bestMove}");
    //}

    //Console.WriteLine("=====================================================================================");

    //*
    // * 8   . . . . . . . k
    // * 7   . . . . . . b .
    // * 6   . . . . . . . p
    // * 5   . . p . p P p P
    // * 4   . . P . . . P .
    // * 3   . . . . . P . .
    // * 2   . . . . . . . N
    // * 1   . . . . . . . K
    // *     a b c d e f g h
    // *
    // *          o
    // *            \ f5f6
    // *             * -----------
    // *     e5e4  /   \  h8h7     \  g7f8
    // *          /     \           \
    // *         o       o ----      o
    // *    f6g7 |  f6f7 | \   \
    // *         |       |  \   \
    // *         *       *   *   *
    // *       +415    +415  ??  ??
    // *
    // *  o -> White
    // *  * -> Black
    // *
    // *       1/     f5f6
    // *       2/     e5e4
    // *       3/     f6g7 + resto de hermanos
    // *       2.a/   e5e4 -> +415
    // *       4/     h8h7
    // *       5/     f6f7 -> +415
    // *       4.a8   h8h7 >= +415 -> Las blancas nunca van a hacer algo peor que eso
    // */
    //{
    //    var game = new Game(fen);
    //    var (evaluation, moveList) = SearchAlgorithms.MiniMax_AlphaBeta(game.CurrentPosition, Configuration.EngineSettings.Depth);
    //    Console.WriteLine($"Evaluation: {evaluation}");

    //    var bestMove = moveList!.Moves.Last();
    //    Console.WriteLine($"Best move: {bestMove}");
    //}
}

static void _52_Quiescence_Search()
{
    //const string fen = "8/1p4pp/kPp5/p1P3PP/KpP5/1Pp2P2/2P5/8 w - - 0 1";

    //var game = new Game(fen);

    //game.CurrentPosition.Print();

    //var (evaluation, moveList) = SearchAlgorithms.MiniMax_AlphaBeta_Quiescence(game.CurrentPosition, Configuration.EngineSettings.Depth - 1);
    //Console.WriteLine($"Evaluation: {evaluation}");

    //var bestMove = moveList!.Moves.Last();
    //Console.WriteLine($"Best move: {bestMove}");
}

static void _53_MVVLVA()
{
    for (int attacker = (int)Piece.P; attacker <= (int)Piece.K; ++attacker)
    {
        for (int victim = (int)Piece.p; victim <= (int)Piece.k; ++victim)
        {
            var score = EvaluationConstants.MostValueableVictimLeastValuableAttacker[attacker, victim];
            Console.WriteLine($"Score {(Piece)attacker}x{(Piece)victim}: {score}");
        }
        Console.WriteLine();
    }

    for (int attacker = (int)Piece.p; attacker <= (int)Piece.k; ++attacker)
    {
        for (int victim = (int)Piece.P; victim <= (int)Piece.K; ++victim)
        {
            var score = EvaluationConstants.MostValueableVictimLeastValuableAttacker[attacker, victim];
            Console.WriteLine($"Score {(Piece)attacker}x{(Piece)victim}: {score}");
        }
        Console.WriteLine();
    }
}

static void _54_ScoreMove()
{
    var position = new Position(KillerPosition);
    position.Print();

    foreach (var move in position.AllCapturesMoves())
    {
        Console.WriteLine($"{move} {move.Score(position)}");
    }

    position = new Position(TrickyPosition);
    position.Print();

    foreach (var move in position.AllCapturesMoves())
    {
        Console.WriteLine($"{move} {move.Score(position)}");
    }
}

static void ZobristTable()
{
    var pos = new Position(KillerPosition);
    var zobristTable = InitializeZobristTable();
    var hash = CalculatePositionHash(zobristTable, pos);
    var updatedHash = UpdatePositionHash(zobristTable, hash, pos.AllPossibleMoves().First());

    Console.WriteLine(updatedHash);
}

static long[,] InitializeZobristTable()
{
    var randomInstance = new Random(int.MaxValue);
    var zobristTable = new long[64, 12];

    for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
    {
        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            zobristTable[squareIndex, pieceIndex] = randomInstance.NextInt64();
        }
    }

    return zobristTable;
}

static long CalculatePositionHash(long[,] zobristTable, Position position)
{
    long positionHash = 0;

    for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
    {
        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            if (position.PieceBitBoards[pieceIndex].GetBit(squareIndex))
            {
                positionHash ^= zobristTable[squareIndex, pieceIndex];
            }
        }
    }

    return positionHash;
}

static long UpdatePositionHash(long[,] zobristTable, long positionHash, Move move)
{
    var sourcePiece = move.Piece();
    var piece = move.PromotedPiece();
    if (piece == default)
    {
        piece = sourcePiece;
    }

    var sourceSquare = move.SourceSquare();
    var targetSquare = move.TargetSquare();

    return positionHash
        ^ zobristTable[sourcePiece, sourceSquare]
        ^ zobristTable[piece, targetSquare]
        ^ zobristTable[piece, targetSquare];
}

static void PV_Table()
{
    // Debugging, Configuration.EngineSettings.MaxDepth = 32

    /*
     * Watch optimized
     *
     *  $"{pvTable[0].ToString()} {pvTable[1].ToString()}" + $" {pvTable[2].ToString()} {pvTable[3].ToString()}" + $"{pvTable[4].ToString()} {pvTable[5].ToString()}" + $" {pvTable[6].ToString()} {pvTable[7].ToString()}"
     *  $"           {pvTable[32].ToString()} {pvTable[33].ToString()}" + $" {pvTable[34].ToString()} {pvTable[35].ToString()}" + $"{pvTable[36].ToString()} {pvTable[37].ToString()}" + $" {pvTable[38].ToString()}"
     *  $"                      {pvTable[63].ToString()} {pvTable[64].ToString()}" + $" {pvTable[65].ToString()} {pvTable[66].ToString()}" + $"{pvTable[67].ToString()} {pvTable[68].ToString()}"
     *  $"                                 {pvTable[93].ToString()} {pvTable[94].ToString()}" + $" {pvTable[95].ToString()} {pvTable[96].ToString()}" + $"{pvTable[97].ToString()}"
     *  $"                                            {pvTable[122].ToString()} {pvTable[123].ToString()}" + $" {pvTable[124].ToString()} {pvTable[125].ToString()}"
     *  $"                                                       {pvTable[150].ToString()} {pvTable[151].ToString()}" + $" {pvTable[152].ToString()}"
     *
     *  $"{pvTable[0]} {pvTable[1]}" + $" {pvTable[2]} {pvTable[3]}" + $"{pvTable[4]} {pvTable[5]}" + $" {pvTable[6]} {pvTable[7]}" + Environment.NewLine +
     *  $"           {pvTable[32]} {pvTable[33]}" + $" {pvTable[34]} {pvTable[35]}" + $"{pvTable[36]} {pvTable[37]}" + $" {pvTable[38]}" + Environment.NewLine +
     *  $"                      {pvTable[63]} {pvTable[64]}" + $" {pvTable[65]} {pvTable[66]}" + $"{pvTable[67]} {pvTable[68]}" + Environment.NewLine +
     *  $"                                 {pvTable[93]} {pvTable[94]}" + $" {pvTable[95]} {pvTable[96]}" + $"{pvTable[97]}" + Environment.NewLine +
     *  $"                                            {pvTable[122]} {pvTable[123]}" + $" {pvTable[124]} {pvTable[125]}" + Environment.NewLine +
     *  $"                                                       {pvTable[150]} {pvTable[151]}" + $" {pvTable[152]}" + Environment.NewLine
     *
     *
     *  Console optimized
            Console.WriteLine(
$"{pvTable[0],-6} {pvTable[1], -6}" + $" {pvTable[2], -6} {pvTable[3], -6}" + $" {pvTable[4], -6} {pvTable[5], -6}" + $" {pvTable[6], -6} {pvTable[7], -6}" + Environment.NewLine +
$"       {pvTable[32], -6} {pvTable[33], -6}" + $" {pvTable[34], -6} {pvTable[35], -6}" + $" {pvTable[36], -6} {pvTable[37], -6}" + $" {pvTable[38], -6}" + Environment.NewLine +
$"              {pvTable[63], -6} {pvTable[64], -6}" + $" {pvTable[65], -6} {pvTable[66], -6}" + $" {pvTable[67], -6} {pvTable[68], -6}" + Environment.NewLine +
$"                     {pvTable[93], -6} {pvTable[94], -6}" + $" {pvTable[95], -6} {pvTable[96], -6}" + $" {pvTable[97], -6}" + Environment.NewLine +
$"                            {pvTable[122], -6} {pvTable[123], -6}" + $" {pvTable[124], -6} {pvTable[125], -6}" + Environment.NewLine +
$"                                   {pvTable[150], -6} {pvTable[151], -6}" + $" {pvTable[152], -6}" + Environment.NewLine);
     *
     */
}

static void CountBits()
{
    var Data = new[] {
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    };

    foreach (var position in Data)
    {
        var oldC = Check_LS1B_And_Reset(position);
        var newC = BitOperations_PopCount(position);

        Console.WriteLine($"{oldC} | {newC}");
    }
    Console.WriteLine("=========================================================================");

    static int Check_LS1B_And_Reset(Position position)
    {
        int counter = 0;

        foreach (var bitboard in position.PieceBitBoards)
        {
            counter += CountBits_Naive(bitboard);
        }

        foreach (var bitboard in position.OccupancyBitBoards)
        {
            counter += CountBits_Naive(bitboard);
        }

        return counter;
    }

    static int BitOperations_PopCount(Position position)
    {
        int counter = 0;

        foreach (var bitboard in position.PieceBitBoards)
        {
            counter += CountBits_PopCount(bitboard);
        }

        foreach (var bitboard in position.OccupancyBitBoards)
        {
            counter += CountBits_PopCount(bitboard);
        }

        return counter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int CountBits_Naive(ulong bitboard)
    {
        int counter = 0;

        // Consecutively reset LSB
        while (bitboard != default)
        {
            ++counter;
            bitboard = ResetLS1B(bitboard);
        }

        return counter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int CountBits_PopCount(ulong bitboard) => BitOperations.PopCount(bitboard);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong ResetLS1B(ulong bitboard)
    {
        return bitboard & (bitboard - 1);
    }
}

static void GetLS1BIndex()
{
    var Data = new[] {
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    };

    foreach (var position in Data)
    {
        var oldC = Original_GetLS1BIndex(position);
        var newC = BitOperations_GetLS1BIndex(position);

        Console.WriteLine($"{oldC} | {newC}");
    }
    Console.WriteLine("=========================================================================");

    static int Original_GetLS1BIndex(Position position)
    {
        int counter = 0;

        foreach (var bitboard in position.PieceBitBoards)
        {
            counter += Original_GetLS1BIndex_Impl(bitboard);
        }

        foreach (var bitboard in position.OccupancyBitBoards)
        {
            counter += Original_GetLS1BIndex_Impl(bitboard);
        }

        return counter;
    }

    static int BitOperations_GetLS1BIndex(Position position)
    {
        int counter = 0;

        foreach (var bitboard in position.PieceBitBoards)
        {
            counter += BitOperations_GetLS1BIndex_Impl(bitboard);
        }

        foreach (var bitboard in position.OccupancyBitBoards)
        {
            counter += BitOperations_GetLS1BIndex_Impl(bitboard);
        }

        return counter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int Original_GetLS1BIndex_Impl(ulong bitboard)
    {
        if (bitboard == default)
        {
            return (int)BoardSquare.noSquare;
        }

        return CountBits(bitboard ^ (bitboard - 1)) - 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int BitOperations_GetLS1BIndex_Impl(ulong bitboard)
    {
        if (bitboard == default)
        {
            return (int)BoardSquare.noSquare;
        }

        return BitOperations.TrailingZeroCount(bitboard);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int CountBits(ulong bitboard)
    {
        return BitOperations.PopCount(bitboard);
    }
}

static void FileAndRankMasks()
{
    const int square = (int)BoardSquare.e4;

    Masks.RankMasks[square].Print();
    Masks.FileMasks[square].Print();
    Masks.IsolatedPawnMasks[square].Print();
    Masks.WhitePassedPawnMasks[square].Print();
    Masks.BlackPassedPawnMasks[square].Print();
}

static void EnhancedPawnEvaluation()
{
    var position = new Position("4k3/ppp5/8/8/8/P7/PP6/4K3 w - - 0 1");
    var eval = position.StaticEvaluation(new(), new());
    position.Print();
    Console.WriteLine(eval);

    position = new Position("4k3/pp4pp/p7/8/8/P7/PPP3P1/4K3 w - - 0 1");
    position.Print();
    eval = position.StaticEvaluation(new(), new());
    Console.WriteLine(eval);
    
    position = new Position("4k3/pp3pp1/p7/8/8/P7/PPP4P/4K3 w - - 0 1");
    position.Print();
    eval = position.StaticEvaluation(new(), new());
    Console.WriteLine(eval);

    position = new Position("4k3/pp3pp1/p7/8/8/P7/PP3P1P/4K3 w - - 0 1");
    position.Print();
    eval = position.StaticEvaluation(new(), new());
    Console.WriteLine(eval);

    position = new Position("4k3/pp2pp2/p7/8/8/P7/PP3P1P/4K3 w - - 0 1");
    position.Print();
    eval = position.StaticEvaluation(new(), new());
    Console.WriteLine(eval);

    position = new Position("4k3/pp2pp2/p7/8/7P/P7/PP3P2/4K3 w - - 0 1");
    position.Print();
    eval = position.StaticEvaluation(new(), new());
    Console.WriteLine(eval);

    position = new Position("4k3/pp2pp1P/p7/8/8/P7/PP3P2/4K3 w - - 0 1");
    position.Print();
    eval = position.StaticEvaluation(new(), new());
    Console.WriteLine(eval);
}
