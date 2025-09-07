using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.Model;
public class MoveToEPDStringTest
{
    [TestCase("d5", (int)BoardSquare.d4, (int)BoardSquare.d5, (int)Piece.P, default, 0)]
    [TestCase("d4", (int)BoardSquare.d2, (int)BoardSquare.d4, (int)Piece.P, 0, 0, 1)]
    [TestCase("d8=Q", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.Q)]
    [TestCase("d8=N", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.N)]
    [TestCase("d8=B", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.B)]
    [TestCase("d8=R", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.R)]
    [TestCase("dxd8=R", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.R, 1)]
    [TestCase("dxe5", (int)BoardSquare.d4, (int)BoardSquare.e5, (int)Piece.P, default, 1)]
    [TestCase("Bxe5", (int)BoardSquare.d4, (int)BoardSquare.e5, (int)Piece.B, default, 1)]
    [TestCase("Nxe5", (int)BoardSquare.d3, (int)BoardSquare.e5, (int)Piece.N, default, 1)]
    [TestCase("dxe7", (int)BoardSquare.d6, (int)BoardSquare.e7, (int)Piece.P, default, 1, default, 1)]
    public void ToEPDString(string expectedString, int sourceSquare, int targetSquare, int piece,
        int promotedPiece = default,
        int isCapture = default, int isDoublePawnPush = default, int isEnPassant = default,
        int isShortCastle = default, int isLongCastle = default)
    {
        int move;

        if (isShortCastle != default)
        {
            move = MoveExtensions.EncodeShortCastle(sourceSquare, targetSquare, piece);
        }
        else if (isLongCastle != default)
        {
            move = MoveExtensions.EncodeLongCastle(sourceSquare, targetSquare, piece);
        }
        else
        {
            if (isEnPassant != default)
            {
                move = MoveExtensions.EncodeEnPassant(sourceSquare, targetSquare, piece);
            }
            else if (isDoublePawnPush != default)
            {
                move = MoveExtensions.EncodeDoublePawnPush(sourceSquare, targetSquare, piece);
            }
            else if (promotedPiece != default)
            {
                if (isCapture != default)
                {
                    move = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece, capturedPiece: 1);
                }
                else
                {
                    move = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece);
                }
            }
            else if (isCapture != default)
            {
                move = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece: 1);
            }
            else
            {
                move = MoveExtensions.Encode(sourceSquare, targetSquare, piece);
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        Assert.AreEqual(expectedString, move.ToEPDString());
#pragma warning restore CS0618 // Type or member is obsolete
    }

    // Ambiguous move
    [TestCase("3K4/8/8/3k4/8/8/2Q1Q3/R6R w - - 0 1", Piece.R, BoardSquare.d1, "Rad1", "Rhd1")]
    [TestCase("3k4/2R1R1N1/3K4/8/3N4/8/8/8 w - - 0 1", Piece.N, BoardSquare.e6, "Nde6", "Nge6")]
    [TestCase("3k4/2R1R1N1/3K4/6N1/8/8/8/8 w - - 0 1", Piece.N, BoardSquare.e6, "N5e6", "N7e6")]
    [TestCase("3k4/2R1R1N1/4q3/6N1/8/4K3/8/8 w - - 0 1", Piece.N, BoardSquare.e6, "N5xe6", "N7xe6")]
    [TestCase("6QQ/7Q/8/8/8/8/8/3k1K2 w - - 0 1", Piece.Q, BoardSquare.g7, "Q7g7", "Qgg7", "Qh8g7")]
    // Not ambiguous moves because of pinned pieces
    [TestCase("3k4/2R1R1N1/8/2b5/3N4/4K3/8/8 w - - 0 1", Piece.N, BoardSquare.e6, "Ne6")]
    [TestCase("3k4/2R1R1N1/8/3K2Nr/8/8/8/8 w - - 1 1", Piece.N, BoardSquare.e6, "Ne6")]
    [TestCase("3k4/2R1R1N1/7b/6N1/8/8/3K4/8 w - - 1 1", Piece.N, BoardSquare.e6, "Ne6")]
    [TestCase("8/4K3/4QQ2/5Qb1/8/8/8/3k4 w - - 0 1", Piece.Q, BoardSquare.e5, "Qee5", "Qfe5")]
    [TestCase("8/5K2/4QQ2/3b1Q2/8/8/8/3k4 w - - 0 1", Piece.Q, BoardSquare.e5, "Q5e5", "Q6e5")]
    // Pawn captures, where file is included instead of piece
    [TestCase("3rr1k1/5ppp/p1p5/1P6/8/Q2B4/PK6/3b4 b - - 0 33", Piece.p, BoardSquare.b5, "axb5", "cxb5")]
    // En-passant
    [TestCase("rnbqkbnr/1pp2p2/p3p2p/2PpP1p1/3P4/8/PP3PPP/RNBQKBNR w KQkq d6 0 6", Piece.P, BoardSquare.d6, "cxd6", "exd6")]
    // Promotion
    [TestCase("3k4/P7/8/8/8/8/p7/3K4 b - - 0 1", Piece.p, BoardSquare.a1, "a1=B", "a1=N", "a1=Q", "a1=R")]
    [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1", Piece.K, BoardSquare.g1, "O-O")]
    [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1", Piece.K, BoardSquare.c1, "O-O-O")]
    [TestCase("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1", Piece.k, BoardSquare.g8, "O-O")]
    [TestCase("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1", Piece.k, BoardSquare.c8, "O-O-O")]
    public void ToStrictEPDString(string fen, Piece piece, BoardSquare targetSquare, string m0, string? m1 = default, string? m2 = default, string? m3 = default)
    {
        var position = new Position(fen);

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);
        evaluationContext.EnsureThreatsAreCalculated(position);

        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, ref evaluationContext, moves).ToArray();

        var ambiguousMoves = pseudoLegalMoves
            .Where(m => m.Piece() == (int)piece && m.TargetSquare() == (int)targetSquare)
            .Where(m =>
            {
                var gameState = position.MakeMove(m);
                var isLegal = position.WasProduceByAValidMove();
                position.UnmakeMove(m, gameState);

                return isLegal;
            })
            .Select(m => m.ToEPDString(position))
            .Order()
            .ToList();

        Assert.AreEqual(m0, ambiguousMoves[0]);
        if (m1 == default)
        {
            Assert.AreEqual(1, ambiguousMoves.Count);
        }
        else
        {
            Assert.AreEqual(m1, ambiguousMoves[1]);

            if (m2 == default)
            {
                Assert.AreEqual(2, ambiguousMoves.Count);
            }
            else
            {
                Assert.AreEqual(m2, ambiguousMoves[2]);

                if (m3 == default)
                {
                    Assert.AreEqual(3, ambiguousMoves.Count);
                }
                else
                {
                    Assert.AreEqual(4, ambiguousMoves.Count);
                    Assert.AreEqual(m3, ambiguousMoves[3]);
                }
            }
        }
    }
}
