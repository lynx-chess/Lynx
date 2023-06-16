using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.Model;

public class PositionMoveConstructorTest
{
    #region Captures

    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . . . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . b . . . .
    /// 4   . . . . N . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . . .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   -- | --
    /// </summary>
    [Test]
    public void Capture_Black()
    {
        // Arrange
        var position = new Position("4k3/8/8/3b4/4N3/8/8/4K3 b - - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.b].GetBit(BoardSquare.d5));
        Assert.True(position.PieceBitBoards[(int)Piece.N].GetBit(BoardSquare.e4));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.e4));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.d5));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e4));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d5));

        var moves = MoveGenerator.GenerateAllMoves(in position);

        var captureMove = moves.Single(m => m.IsCapture());

        // Act
        var newPosition = new Position(in position, captureMove);

        // Assert
        Assert.False(newPosition.PieceBitBoards[(int)Piece.b].GetBit(BoardSquare.d5));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.N].GetBit(BoardSquare.e4));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.b].GetBit(BoardSquare.e4));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.e4));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.d5));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.e4));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d5));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e4));
    }

    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . . . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . B . . . .
    /// 4   . . . . n . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . . .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   -- | --
    /// </summary>
    [Test]
    public void Capture_White()
    {
        // Arrange
        var position = new Position("4k3/8/8/3B4/4n3/8/8/4K3 w - - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.B].GetBit(BoardSquare.d5));
        Assert.True(position.PieceBitBoards[(int)Piece.n].GetBit(BoardSquare.e4));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.e4));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.d5));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e4));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d5));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var captureMove = moves.Single(m => m.IsCapture());

        // Act
        var newPosition = new Position(in position, captureMove);

        // Assert
        Assert.False(newPosition.PieceBitBoards[(int)Piece.B].GetBit(BoardSquare.d5));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.n].GetBit(BoardSquare.e4));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.B].GetBit(BoardSquare.e4));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.e4));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.d5));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.e4));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d5));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e4));
    }

    #endregion

    #region Promotions

    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . P . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . . .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   -- | --
    /// </summary>
    [Test]
    public void Promotion_White()
    {
        // Arrange
        var position = new Position("4k3/1P6/8/8/8/8/8/4K3 w - - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b7));
        Assert.False(position.PieceBitBoards[(int)Piece.N].GetBit(BoardSquare.b8));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b7));
        Assert.False(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b7));
        Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var promotionMove = moves.Single(m => m.PromotedPiece() == (int)Piece.N);

        // Act
        var newPosition = new Position(in position, promotionMove);

        // Assert
        Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b7));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.N].GetBit(BoardSquare.b8));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b7));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b8));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b7));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8));
    }

    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . . . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . . .
    /// 1   . p . . K . . .
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   -- | --
    /// </summary>
    [Test]
    public void Promotion_Black()
    {
        // Arrange
        var position = new Position("4k3/8/8/8/8/8/1p6/4K3 b - - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.b2));
        Assert.False(position.PieceBitBoards[(int)Piece.n].GetBit(BoardSquare.b1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.b2));
        Assert.False(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.b1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b2));
        Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var promotionMove = moves.Single(m => m.PromotedPiece() == (int)Piece.n);

        // Act
        var newPosition = new Position(in position, promotionMove);

        // Assert
        Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.b2));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.n].GetBit(BoardSquare.b1));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.b2));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.b1));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b2));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1));
    }

    /// <summary>
    /// 8   r k . . . . . .
    /// 7   . P . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . . .
    /// 1   . . . . K . . .
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   -- | --
    /// </summary>
    [Test]
    public void PromotionWithCapture_White()
    {
        // Arrange
        var position = new Position("rk6/1P6/8/8/8/8/8/4K3 w - - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b7));
        Assert.False(position.PieceBitBoards[(int)Piece.N].GetBit(BoardSquare.a8));
        Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.a8));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b7));
        Assert.False(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.a8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.a8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b7));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a8));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var promotionMove = moves.Single(m => m.PromotedPiece() == (int)Piece.N);

        // Act
        var newPosition = new Position(in position, promotionMove);

        // Assert
        Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b7));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.N].GetBit(BoardSquare.a8));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.a8));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b7));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.a8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.a8));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b7));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a8));
    }

    /// <summary>
    /// 8   . . . . k . . .
    /// 7   . . . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . p . . . . . .
    /// 1   R K . . . . . .
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   -- | --
    /// </summary>
    [Test]
    public void PromotionWithCapture_Black()
    {
        // Arrange
        var position = new Position("4k3/8/8/8/8/8/1p6/RK6 b - - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.b2));
        Assert.False(position.PieceBitBoards[(int)Piece.n].GetBit(BoardSquare.a1));
        Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.a1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.b2));
        Assert.False(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.a1));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.a1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b2));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a1));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var promotionMove = moves.Single(m => m.PromotedPiece() == (int)Piece.n);

        // Act
        var newPosition = new Position(in position, promotionMove);

        // Assert
        Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.b2));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.n].GetBit(BoardSquare.a1));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.a1));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.b2));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.a1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.a1));

        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b2));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a1));
    }

    #endregion

    #region DoublePawnPush

    [Test]
    public void DoublePawnPush_White()
    {
        // Arrange
        var position = new Position("4k3/8/8/8/2p5/8/1P6/4K3 w - - 0 1");

        Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b2));
        Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c4));
        Assert.False(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b4));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b2));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.c4));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b2));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c4));
        Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b4));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var enPassant = moves.Single(m => m.IsDoublePawnPush());

        // Act
        var newPosition = new Position(in position, enPassant);

        // Assert
        Assert.AreEqual(BoardSquare.b3, newPosition.EnPassant);

        Assert.True(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b4));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c4));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b2));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b3));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b4));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b2));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.b3));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b4));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c4));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b2));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b3));
    }

    [Test]
    public void DoublePawnPush_Black()
    {
        // Arrange
        var position = new Position("4k3/2p5/8/1P6/8/8/8/4K3 b - - 0 1");

        Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c7));
        Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b5));
        Assert.False(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c5));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.c7));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b5));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c7));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b5));
        Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c5));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var enPassant = moves.Single(m => m.IsDoublePawnPush());

        // Act
        var newPosition = new Position(in position, enPassant);

        // Assert
        Assert.AreEqual(BoardSquare.c6, newPosition.EnPassant);

        Assert.True(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c5));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b5));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c7));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c6));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.c5));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.c2));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b6));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b5));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c5));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c7));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c6));
    }

    #endregion

    #region EnPassant

    [Test]
    public void EnPassant_White()
    {
        // Arrange
        var position = new Position("4k3/8/8/1Pp5/8/8/8/4K3 w - c6 0 1");

        Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b5));
        Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c5));
        Assert.False(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c6));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b5));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.c5));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b5));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c5));
        Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c6));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var enPassant = moves.Single(m => m.IsEnPassant());

        // Act
        var newPosition = new Position(in position, enPassant);

        // Assert
        Assert.True(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.c6));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c5));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b5));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.c6));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b5));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.c5));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c6));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c5));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b5));
    }

    [Test]
    public void EnPassant_Black()
    {
        // Arrange
        var position = new Position("4k3/8/8/8/1Pp5/8/8/4K3 b - b3 0 1");

        Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b4));
        Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c4));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b4));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.c4));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b4));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c4));
        Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b3));

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var enPassant = moves.Single(m => m.IsEnPassant());

        // Act
        var newPosition = new Position(in position, enPassant);

        // Assert
        Assert.True(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.b3));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c4));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b4));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.b3));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.b4));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.c4));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b3));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c4));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b4));
    }

    [TestCase(Constants.KillerTestPositionFEN)]
    [TestCase(Constants.TrickyTestPositionReversedFEN)]
    [TestCase(Constants.TrickyTestPositionFEN)]
    public void DoublePawnPushEnablesEnPassant(string fen)
    {
        var position = new Position(fen);

        foreach (var move in MoveGenerator.GenerateAllMoves(in position).Where(m => m.IsDoublePawnPush()))
        {
            var newPosition = new Position(in position, move);
            Assert.AreNotEqual(position.EnPassant, newPosition.EnPassant);
        }
    }

    [TestCase("r3kb2/1b4PP/p2ppppp/NPp1qn1r/2B2N2/1PPPPPP1/1p4B1/R2QK2R w KQq c6 0 1")]
    [TestCase("r3kb2/1b4PP/p2ppppp/N1p1qn1r/2B2N2/1PPPPPP1/1p4B1/R2QK2R w KQq c6 0 1")]
    [TestCase("r3kb2/1b6/p2pppp1/N1p1qn1r/2B2NPp/1PPPPP2/1p3B2/R2QK2R b KQq g3 0 1")]
    [TestCase("r3kb2/1b6/p2pppp1/N1p1qn1r/2B2NP1/1PPPPP2/1p3B2/R2QK2R b KQq g3 0 1")]
    [TestCase(Constants.KillerTestPositionFEN)]
    [TestCase("4k3/8/8/1Pp5/8/8/8/4K3 w - c6 0 1")]
    public void AnyMoveDisablesEnPassant(string fen)
    {
        var position = new Position(fen);

        Assert.AreNotEqual(BoardSquare.noSquare, position.EnPassant);

        foreach (var move in MoveGenerator.GenerateAllMoves(in position).Where(m => !m.IsDoublePawnPush()))
        {
            var newPosition = new Position(in position, move);
            Assert.AreEqual(BoardSquare.noSquare, newPosition.EnPassant);
        }
    }

    #endregion

    #region Castling

    [Test]
    public void CastlingRemovesCastlingRights_Short_White()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.K].GetBit(BoardSquare.e1));
        Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.a1));
        Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.h1));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.e1));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.a1));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.h1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.h1));
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var shortCastling = moves.Single(m => m.IsShortCastle());

        // Act
        var newPosition = new Position(in position, shortCastling);

        // Assert - position and occupancy after castling
        Assert.True(newPosition.PieceBitBoards[(int)Piece.K].GetBit(Constants.WhiteShortCastleKingSquare));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.R].GetBit(Constants.WhiteShortCastleRookSquare));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.a1));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.K].GetBit(BoardSquare.e1));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.h1));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(Constants.WhiteShortCastleKingSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(Constants.WhiteShortCastleRookSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.a1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.e1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.h1));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.WhiteShortCastleKingSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.WhiteShortCastleRookSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.h1));

        // Assert - Castling rights
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void CastlingRemovesCastlingRights_Long_White()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.K].GetBit(BoardSquare.e1));
        Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.a1));
        Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.h1));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.e1));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.a1));
        Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.h1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a1));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.h1));
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var shortCastling = moves.Single(m => m.IsLongCastle());

        // Act
        var newPosition = new Position(in position, shortCastling);

        // Assert - position and occupancy after castling
        Assert.True(newPosition.PieceBitBoards[(int)Piece.K].GetBit(Constants.WhiteLongCastleKingSquare));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.R].GetBit(Constants.WhiteLongCastleRookSquare));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.h1));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.K].GetBit(BoardSquare.e1));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquare.a1));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(Constants.WhiteLongCastleKingSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(Constants.WhiteLongCastleRookSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.h1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.e1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquare.a1));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.WhiteLongCastleKingSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.WhiteLongCastleRookSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.h1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e1));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a1));

        // Assert - Castling rights
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void CastlingRemovesCastlingRights_Short_Black()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.k].GetBit(BoardSquare.e8));
        Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.a8));
        Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.h8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.e8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.a8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.h8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.h8));
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var shortCastling = moves.Single(m => m.IsShortCastle());

        // Act
        var newPosition = new Position(in position, shortCastling);

        // Assert - position and occupancy after castling
        Assert.True(newPosition.PieceBitBoards[(int)Piece.k].GetBit(Constants.BlackShortCastleKingSquare));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.r].GetBit(Constants.BlackShortCastleRookSquare));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.a8));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.k].GetBit(BoardSquare.e8));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.h8));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(Constants.BlackShortCastleKingSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(Constants.BlackShortCastleRookSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.a8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.e8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.h8));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.BlackShortCastleKingSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.BlackShortCastleRookSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.h8));

        // Assert - Castling rights
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
    }

    [Test]
    public void CastlingRemovesCastlingRights_Long_Black()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
        Assert.True(position.PieceBitBoards[(int)Piece.k].GetBit(BoardSquare.e8));
        Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.a8));
        Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.h8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.e8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.a8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.h8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a8));
        Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.h8));
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var shortCastling = moves.Single(m => m.IsLongCastle());

        // Act
        var newPosition = new Position(in position, shortCastling);

        // Assert - position and occupancy after castling
        Assert.True(newPosition.PieceBitBoards[(int)Piece.k].GetBit(Constants.BlackLongCastleKingSquare));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.r].GetBit(Constants.BlackLongCastleRookSquare));
        Assert.True(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.h8));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.k].GetBit(BoardSquare.e8));
        Assert.False(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquare.a8));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(Constants.BlackLongCastleKingSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(Constants.BlackLongCastleRookSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.h8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.e8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquare.a8));

        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.BlackLongCastleKingSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.BlackLongCastleRookSquare));
        Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.h8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.e8));
        Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.a8));

        // Assert - Castling rights
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
    }

    [Test]
    public void MovingKingRemovesCastlingRights_White()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var quietKingMove = moves.First(m => !m.IsCastle() && m.Piece() == (int)Piece.K + Utils.PieceOffset(position.Side));

        // Act
        var newPosition = new Position(in position, quietKingMove);

        // Assert - Castling rights
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void MovingKingRemovesCastlingRights_Black()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var quietKingMove = moves.First(m => !m.IsCastle() && m.Piece() == (int)Piece.K + Utils.PieceOffset(position.Side));

        // Act
        var newPosition = new Position(in position, quietKingMove);

        // Assert - Castling rights
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
    }

    [Test]
    public void MovingRookRemovesCastlingRights_Queenside_White()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookMove = moves.First(m =>
            m.Piece() == (int)Piece.R + Utils.PieceOffset(position.Side)
            && !m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.a8 + (7 * 8 * (int)position.Side));

        // Act
        var newPosition = new Position(in position, rookMove);

        // Assert - Castling rights
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void MovingRookRemovesCastlingRights_Kingside_White()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookMove = moves.First(m =>
            m.Piece() == (int)Piece.R + Utils.PieceOffset(position.Side)
            && !m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.h8 + (7 * 8 * (int)position.Side));

        // Act
        var newPosition = new Position(in position, rookMove);

        // Assert - Castling rights
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void MovingRookRemovesCastlingRights_Queenside_Black()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookMove = moves.First(m =>
            m.Piece() == (int)Piece.R + Utils.PieceOffset(position.Side)
            && !m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.a8 + (7 * 8 * (int)position.Side));

        // Act
        var newPosition = new Position(in position, rookMove);

        // Assert - Castling rights
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void MovingRookRemovesCastlingRights_Kingside_Black()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookMove = moves.First(m =>
            m.Piece() == (int)Piece.R + Utils.PieceOffset(position.Side)
            && !m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.h8 + (7 * 8 * (int)position.Side));

        // Act
        var newPosition = new Position(in position, rookMove);

        // Assert - Castling rights
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void CapturingRookRemovesCastlingRights_QueenSide_BlackRook_Queenside()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/3B4/8/8/8/R3K2R w KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookCapture = moves.First(m =>
            m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
            && m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.d5);

        Assert.AreEqual((int)BoardSquare.a8, rookCapture.TargetSquare());

        // Act
        var newPosition = new Position(in position, rookCapture);

        // Assert - Castling rights
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void CapturingRookRemovesCastlingRights_QueenSide_BlackRook_Kingside()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/4B3/8/8/8/R3K2R w KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookCapture = moves.First(m =>
            m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
            && m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.e5);

        Assert.AreEqual((int)BoardSquare.h8, rookCapture.TargetSquare());

        // Act
        var newPosition = new Position(in position, rookCapture);

        // Assert - Castling rights
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void CapturingRookRemovesCastlingRights_QueenSide_WhiteRook_Queenside()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/4b3/8/8/8/R3K2R b KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookCapture = moves.First(m =>
            m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
            && m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.e5);

        Assert.AreEqual((int)BoardSquare.a1, rookCapture.TargetSquare());

        // Act
        var newPosition = new Position(in position, rookCapture);

        // Assert - Castling rights
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    [Test]
    public void CapturingRookRemovesCastlingRights_QueenSide_WhiteRook_Kingside()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/3b4/8/8/8/R3K2R b KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookCapture = moves.First(m =>
            m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
            && m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.d5);

        Assert.AreEqual((int)BoardSquare.h1, rookCapture.TargetSquare());

        // Act
        var newPosition = new Position(in position, rookCapture);

        // Assert - Castling rights
        Assert.AreEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   . r . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . B . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . . .
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void CapturingAnotherRookShouldntRemoveCastlingRights_BlackRook()
    {
        // Arrange
        var position = new Position("r3k2r/1r6/8/3B4/8/8/8/R3K2R w KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookCapture = moves.First(m =>
            m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
            && m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.d5);

        // Act
        var newPosition = new Position(in position, rookCapture);

        // Assert - Castling rights
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   . . . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . b . . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . R .
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void CapturingAnotherRookShouldntRemoveCastlingRights_WhiteRook()
    {
        // Arrange
        var position = new Position("r3k2r/8/8/3b4/8/8/6R1/R3K2R b KQkq - 0 1");
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), position.Castle & (int)CastlingRights.BQ);

        var moves = MoveGenerator.GenerateAllMoves(in position);
        var rookCapture = moves.First(m =>
            m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
            && m.IsCapture()
            && m.SourceSquare() == (int)BoardSquare.d5);

        // Act
        var newPosition = new Position(in position, rookCapture);

        // Assert - Castling rights
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.WQ);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BK);
        Assert.AreNotEqual(default(int), newPosition.Castle & (int)CastlingRights.BQ);
    }

    #endregion
}
