using SharpFish;
using SharpFish.Model;
using System;

//_2_GettingStarted();
//_3_PawnAttacks();
//_4_KnightAttacks();
//_5_KingAttacks();
//_6_Bishop_Occupancy();
//_7_Rook_Occupancy();
//_8_Slider_Pieces_Attacks();
//_9_BitCount();
_10_SetOccupancy();

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