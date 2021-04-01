using SharpFish;
using SharpFish.Model;
using System;

//_2_GettingStarted();
//_3_PawnAttacks();
//_4_KnightAttacks();
//_5_KingAttacks();
//_6_Bishop_Occupancy();
_7_Rook_Occupancy();

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
    var occupancy = AttacksGenerator.MaskBishopAttacks((int)BoardSquares.h8);
    occupancy.Print();

    AttacksGenerator.InitializeBishopOccupancy();
}

static void _7_Rook_Occupancy()
{
    var occupancy = AttacksGenerator.MaskRookAttacks((int)BoardSquares.e4);
    occupancy.Print();

    AttacksGenerator.InitializeRookOccupancy();
}