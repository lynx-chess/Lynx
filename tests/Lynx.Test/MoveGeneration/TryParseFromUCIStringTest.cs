using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class TryParseFromUCIStringTest
{
    /// <summary>
    /// 8   r . b . k . . r
    /// 7   p P p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . .
    /// 2   P . P B . P p P
    /// 1   R . . . K B . R
    ///     a b c d e f g h
    /// </summary>
    [TestCase("b7b8q", BoardSquare.b7, BoardSquare.b8, Piece.Q)]
    [TestCase("b7b8r", BoardSquare.b7, BoardSquare.b8, Piece.R)]
    [TestCase("b7b8n", BoardSquare.b7, BoardSquare.b8, Piece.N)]
    [TestCase("b7b8b", BoardSquare.b7, BoardSquare.b8, Piece.B)]
    [TestCase("b7a8q", BoardSquare.b7, BoardSquare.a8, Piece.Q)]
    [TestCase("b7a8r", BoardSquare.b7, BoardSquare.a8, Piece.R)]
    [TestCase("b7a8n", BoardSquare.b7, BoardSquare.a8, Piece.N)]
    [TestCase("b7a8b", BoardSquare.b7, BoardSquare.a8, Piece.B)]
    [TestCase("b7c8q", BoardSquare.b7, BoardSquare.c8, Piece.Q)]
    [TestCase("b7c8r", BoardSquare.b7, BoardSquare.c8, Piece.R)]
    [TestCase("b7c8n", BoardSquare.b7, BoardSquare.c8, Piece.N)]
    [TestCase("b7c8b", BoardSquare.b7, BoardSquare.c8, Piece.B)]
    [TestCase("e1d1", BoardSquare.e1, BoardSquare.d1, default(Piece))]
    [TestCase("e1c1", BoardSquare.e1, (BoardSquare)Constants.WhiteKingQueensideCastlingSquare, default(Piece))]
    public void ParseFromUCIString_White(string UCIString, BoardSquare sourceSquare, BoardSquare targetSquare, Piece promotedPiece)
    {
        // Arrange
        const string fen = "r1b1k2r/pPppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q2/P1PB1PpP/R3KB1R w KQkq - 0 1";
        var position = new Position(fen);
        var moves = MoveGenerator.GenerateAllMoves(position);

        // Act
        Assert.True(MoveExtensions.TryParseFromUCIString(position, UCIString, moves, out var move));

        // Assert
        Assert.AreEqual((int)sourceSquare, move!.Value.SourceSquare());
        Assert.AreEqual((int)targetSquare, move!.Value.TargetSquare());
        Assert.AreEqual((int)promotedPiece, move!.Value.PromotedPiece());
    }

    /// <summary>
    /// 8   r . b . k . . r
    /// 7   p P p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . .
    /// 2   P . P B . P p P
    /// 1   R . . . K B . R
    ///     a b c d e f g h
    /// </summary>
    [TestCase("g2g1q", BoardSquare.g2, BoardSquare.g1, Piece.q)]
    [TestCase("g2g1r", BoardSquare.g2, BoardSquare.g1, Piece.r)]
    [TestCase("g2g1n", BoardSquare.g2, BoardSquare.g1, Piece.n)]
    [TestCase("g2g1b", BoardSquare.g2, BoardSquare.g1, Piece.b)]
    [TestCase("g2h1q", BoardSquare.g2, BoardSquare.h1, Piece.q)]
    [TestCase("g2h1r", BoardSquare.g2, BoardSquare.h1, Piece.r)]
    [TestCase("g2h1n", BoardSquare.g2, BoardSquare.h1, Piece.n)]
    [TestCase("g2h1b", BoardSquare.g2, BoardSquare.h1, Piece.b)]
    [TestCase("g2f1q", BoardSquare.g2, BoardSquare.f1, Piece.q)]
    [TestCase("g2f1r", BoardSquare.g2, BoardSquare.f1, Piece.r)]
    [TestCase("g2f1n", BoardSquare.g2, BoardSquare.f1, Piece.n)]
    [TestCase("g2f1b", BoardSquare.g2, BoardSquare.f1, Piece.b)]
    [TestCase("e8f8", BoardSquare.e8, BoardSquare.f8, default(Piece))]
    [TestCase("e8g8", BoardSquare.e8, (BoardSquare)Constants.BlackKingKingsideCastlingSquare, default(Piece))]
    public void ParseFromUCIString_Black(string UCIString, BoardSquare sourceSquare, BoardSquare targetSquare, Piece promotedPiece)
    {
        // Arrange
        const string fen = "r1b1k2r/pPppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q2/P1PB1PpP/R3KB1R b KQkq - 0 1";
        var position = new Position(fen);
        var moves = MoveGenerator.GenerateAllMoves(position);

        // Act
        Assert.True(MoveExtensions.TryParseFromUCIString(position, UCIString, moves, out var move));

        // Assert
        Assert.AreEqual((int)sourceSquare, move!.Value.SourceSquare());
        Assert.AreEqual((int)targetSquare, move!.Value.TargetSquare());
        Assert.AreEqual((int)promotedPiece, move!.Value.PromotedPiece());
    }

    [TestCase("e2e5")]
    [TestCase("e7e8q")]
    public void ParseFromUCIString_Error(string UCIString)
    {
        // Arrange
        const string fen = Constants.InitialPositionFEN;
        var position = new Position(fen);
        var moves = MoveGenerator.GenerateAllMoves(position);

        // Act & Assert
        Assert.False(MoveExtensions.TryParseFromUCIString(position, UCIString, moves, out var result));
        Assert.Null(result);
    }
}
