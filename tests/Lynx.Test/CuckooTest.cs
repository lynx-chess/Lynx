using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

public class CuckooTest
{
    [Test]
    public void TableContainsAllReversibleMoves()
    {
        var count = 0;

        for (int offset = 0; offset <= 6; offset += 6)
        {
            for (int pieceType = (int)Piece.N; pieceType <= (int)Piece.K; ++pieceType)
            {
                var piece = pieceType + offset;

                for (int from = 0; from < 64; ++from)
                {
                    for (int to = from + 1; to < 64; ++to)
                    {
                        var key = ZobristTable.PieceHash(from, piece) ^ ZobristTable.PieceHash(to, piece) ^ ZobristTable.SideHash();

                        if (CanMoveOnEmptyBoard(pieceType, from, to))
                        {
                            ++count;

                            Assert.True(Cuckoo.TryGetMove(key, out var foundFrom, out var foundTo), $"{(Piece)piece} {from}-{to} not found");
                            Assert.AreEqual(from, foundFrom, $"{(Piece)piece} {from}-{to}");
                            Assert.AreEqual(to, foundTo, $"{(Piece)piece} {from}-{to}");
                        }
                        else
                        {
                            Assert.False(Cuckoo.TryGetMove(key, out _, out _), $"{(Piece)piece} {from}-{to} shouldn't be found");
                        }
                    }
                }
            }
        }

        Assert.AreEqual(Cuckoo.ExpectedMoveCount, count);
        Assert.AreEqual(Cuckoo.ExpectedMoveCount, Cuckoo.Count);
    }

    [Test]
    public void TableDoesNotContainPawnMovesNorOtherKeys()
    {
        for (int pawn = (int)Piece.P; pawn <= (int)Piece.p; pawn += (int)Piece.p)
        {
            for (int from = 0; from < 64; ++from)
            {
                for (int to = from + 1; to < 64; ++to)
                {
                    var key = ZobristTable.PieceHash(from, pawn) ^ ZobristTable.PieceHash(to, pawn) ^ ZobristTable.SideHash();

                    Assert.False(Cuckoo.TryGetMove(key, out _, out _), $"{(Piece)pawn} {from}-{to} shouldn't be found");
                }
            }
        }

        // Empty slots have key 0
        Assert.False(Cuckoo.TryGetMove(0, out _, out _));

        var random = new LynxRandom(42);
        for (int i = 0; i < 10_000; ++i)
        {
            Assert.False(Cuckoo.TryGetMove(random.NextUInt64(), out _, out _));
        }
    }

    [Test]
    public void HasUpcomingRepetition_RookAndKingShuffle()
    {
        var game = new Game("7k/8/5KR1/8/8/8/5R2/8 w - - 0 1");

        Assert.False(game.HasUpcomingRepetition());

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R));
        Assert.False(game.HasUpcomingRepetition());

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k));
        Assert.False(game.HasUpcomingRepetition());

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R));
        Assert.True(game.HasUpcomingRepetition());      // Kh7-h8 repeats the initial position
        Assert.False(game.IsThreefoldRepetition(1));

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k));
        Assert.True(game.IsThreefoldRepetition(1));
        Assert.True(game.HasUpcomingRepetition());      // Rf2-e2 repeats the position after 1.Re2
    }

    [Test]
    public void HasUpcomingRepetition_BlockedPath()
    {
        // The queen can go from a1 to h1 once the black knight leaves d1, but the knight gets back to d1 and now the way back is blocked
        var game = new Game("6k1/8/8/8/8/6K1/8/Q2n4 b - - 0 1");

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.d1, (int)BoardSquare.b2, (int)Piece.n));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.a1, (int)BoardSquare.h1, (int)Piece.Q));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.b2, (int)BoardSquare.d1, (int)Piece.n));

        // White pieces are back in their original squares in the hash of the position 3 plies ago, but Qh1-a1 is blocked by Nd1
        Assert.False(game.HasUpcomingRepetition());
        Assert.False(game.IsThreefoldRepetition(1));
    }

    [Test]
    public void HasUpcomingRepetition_CaptureResetsTheWindow()
    {
        var game = new Game("7k/8/5KR1/8/8/8/4nR2/8 w - - 0 1");

        game.MakeMove(MoveExtensions.EncodeCapture((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R, (int)Piece.n));
        Assert.False(game.HasUpcomingRepetition());

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k));
        Assert.False(game.HasUpcomingRepetition());

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R));
        Assert.False(game.HasUpcomingRepetition());     // Kh7-h8 doesn't repeat a position: before the capture there was a knight on e2

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.k));

        // The position right after the capture is the oldest one that can be repeated
        Assert.True(game.HasUpcomingRepetition());      // Rf2-e2
    }

    [Test]
    public void HasUpcomingRepetition_PositionCommand()
    {
        var game = new Game(Constants.InitialPositionFEN);

        game.ParsePositionCommand("position startpos moves g1f3 g8f6 f3g1");
        Assert.True(game.HasUpcomingRepetition());      // Nf6-g8

        game.ParsePositionCommand("position startpos moves g1f3 g8f6 b1c3");
        Assert.False(game.HasUpcomingRepetition());
    }

    [Test]
    public void HasUpcomingRepetition_FENHalfMoveClockLargerThanHistory()
    {
        var game = new Game("8/8/8/8/8/8/R7/K6k w - - 90 80");

        Assert.False(game.HasUpcomingRepetition());
    }

    [Test]
    public void HasUpcomingRepetition_CastlingRightsLost()
    {
        // Moving the king loses castling rights: going back doesn't repeat the position
        var game = new Game("r3k3/8/8/8/8/8/8/R3K3 w Qq - 0 1");

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e1, (int)BoardSquare.e2, (int)Piece.K));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e8, (int)BoardSquare.e7, (int)Piece.k));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e1, (int)Piece.K));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.k));

        Assert.False(game.HasUpcomingRepetition());
        Assert.False(game.IsThreefoldRepetition(1));

        // But positions after the castling rights were lost can be repeated
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e1, (int)BoardSquare.e2, (int)Piece.K));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e8, (int)BoardSquare.e7, (int)Piece.k));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e1, (int)Piece.K));

        Assert.True(game.HasUpcomingRepetition());      // Ke7-e8
    }

    [Test]
    public void HasUpcomingRepetition_NullMoveLimitsTheWindow()
    {
        var game = new Game("7k/8/5KR1/8/8/8/5R2/8 w - - 0 1");
        var position = game.CurrentPosition;

        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.h8, (int)BoardSquare.h7, (int)Piece.k));
        game.MakeMove(MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R));
        Assert.True(game.HasUpcomingRepetition());

        var nullMoveGameState = position.MakeNullMove();
        var previousNullMovePointer = game.AddNullMoveToPositionHashHistory(position.UniqueIdentifier);

        Assert.False(game.HasUpcomingRepetition());

        game.RemoveNullMoveFromPositionHashHistory(previousNullMovePointer);
        position.UnMakeNullMove(nullMoveGameState);

        Assert.True(game.HasUpcomingRepetition());
    }

    /// <summary>
    /// Random walks (biased towards shuffling pieces back and forth, so that there are plenty of repetitions)
    /// where, at every position, <see cref="Game.HasUpcomingRepetition"/> is compared against the brute force approach
    /// (playing every legal move and checking <see cref="Game.IsThreefoldRepetition(int)"/> with ply > 0, the way the search does it)
    /// </summary>
    [TestCase("7k/8/5KR1/8/8/8/5R2/8 w - - 0 1")]
    [TestCase("8/5k2/8/8/3Q4/8/1B6/4K3 w - - 0 1")]
    [TestCase("4k3/8/2n5/8/8/5N2/8/4K3 w - - 0 1")]
    [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1")]
    [TestCase("r1bq1rk1/ppp2ppp/2n2n2/3pp3/1bPP4/2N1PN2/PP3PPP/R1BQKB1R w KQ - 0 1")]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    [TestCase("8/8/8/4k3/8/3K4/8/R3B3 b - - 40 70")]    // Half move clock larger than the history
    [TestCase("6k1/5ppp/8/8/8/8/5PPP/1Q4K1 w - - 0 1")]
    public void HasUpcomingRepetition_MatchesBruteForce(string fen)
    {
        var random = new Random(12345);

        var game = new Game(fen);
        var stats = new WalkStats();

        for (int i = 0; i < 300; ++i)
        {
            Walk(game, random, 80, [], stats);
        }

        TestContext.Out.WriteLine($"{stats.Checked} positions checked, {stats.WithUpcomingRepetition} with an upcoming repetition");

        Assert.Greater(stats.Checked, 1000);
        Assert.AreEqual(0, stats.Mismatches);
    }

    private sealed class WalkStats
    {
        public int Checked { get; set; }

        public int WithUpcomingRepetition { get; set; }

        public int Mismatches { get; set; }
    }

    private static void Walk(Game game, Random random, int remainingPlies, List<Move> path, WalkStats stats)
    {
        var position = game.CurrentPosition;
        var moves = ValidMovesAndRepetitions(game);

        // After 100 half moves the counter is no longer reset by captures or pawn moves
        if (game.HalfMovesWithoutCaptureOrPawnMove < 100)
        {
            var expected = moves.Exists(m => m.Repeats);
            var actual = game.HasUpcomingRepetition();

            ++stats.Checked;
            if (expected)
            {
                ++stats.WithUpcomingRepetition;
            }

            if (expected != actual)
            {
                ++stats.Mismatches;
            }

            Assert.AreEqual(
                expected,
                actual,
                $"{position.FEN(game.HalfMovesWithoutCaptureOrPawnMove)}, path: {string.Join(' ', path.Select(m => m == MoveExtensions.NullMove ? "null" : m.UCIString()))}");
        }

        if (remainingPlies == 0 || moves.Count == 0)
        {
            return;
        }

        // Null move, as NMP does it
        if (random.Next(12) == 0 && !position.IsInCheck())
        {
            var nullMoveGameState = position.MakeNullMove();
            var previousNullMovePointer = game.AddNullMoveToPositionHashHistory(position.UniqueIdentifier);
            path.Add(MoveExtensions.NullMove);

            Walk(game, random, remainingPlies - 1, path, stats);

            path.RemoveAt(path.Count - 1);
            game.RemoveNullMoveFromPositionHashHistory(previousNullMovePointer);
            position.UnMakeNullMove(nullMoveGameState);

            return;
        }

        var move = ChooseMove(moves, path, random);

        var gameState = position.MakeMove(move);
        var oldHalfMovesWithoutCaptureOrPawnMove = game.HalfMovesWithoutCaptureOrPawnMove;
        game.Update50movesRule(move);
        game.AddToPositionHashHistory(position.UniqueIdentifier);
        path.Add(move);

        Walk(game, random, remainingPlies - 1, path, stats);

        path.RemoveAt(path.Count - 1);
        game.RemoveFromPositionHashHistory();
        game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
        position.UnmakeMove(move, gameState);
    }

    private static Move ChooseMove(List<(Move Move, bool Repeats)> moves, List<Move> path, Random random)
    {
        // Mostly going back to where the piece was 2 plies ago, so that we keep repeating positions
        if (path.Count >= 2 && random.Next(4) != 0)
        {
            var twoPliesAgo = path[^2];

            if (twoPliesAgo != MoveExtensions.NullMove)
            {
                foreach (var (move, _) in moves)
                {
                    if (move.SourceSquare() == twoPliesAgo.TargetSquare()
                        && move.TargetSquare() == twoPliesAgo.SourceSquare()
                        && move.Piece() == twoPliesAgo.Piece())
                    {
                        return move;
                    }
                }
            }
        }

        // Otherwise, a random reversible move most of the times (triangulation, etc.)
        if (random.Next(5) != 0)
        {
            var reversibleMoves = moves
                .Where(m => m.Move.CapturedPiece() == (int)Piece.None && m.Move.Piece() != (int)Piece.P && m.Move.Piece() != (int)Piece.p)
                .ToList();

            if (reversibleMoves.Count > 0)
            {
                return reversibleMoves[random.Next(reversibleMoves.Count)].Move;
            }
        }

        return moves[random.Next(moves.Count)].Move;
    }

    /// <summary>
    /// All the legal moves, and whether they lead to a position that has already been seen
    /// (<see cref="Game.IsThreefoldRepetition(int)"/> with ply > 0, the same way NegaMax uses it)
    /// </summary>
    private static List<(Move Move, bool Repeats)> ValidMovesAndRepetitions(Game game)
    {
        var position = game.CurrentPosition;
        var movePool = new Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        var result = new List<(Move Move, bool Repeats)>();

        foreach (var move in MoveGenerator.GenerateAllMoves(position, movePool))
        {
            var gameState = position.MakeMove(move);
            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            var oldHalfMovesWithoutCaptureOrPawnMove = game.HalfMovesWithoutCaptureOrPawnMove;
            var canBeRepetition = game.Update50movesRule(move);
            game.AddToPositionHashHistory(position.UniqueIdentifier);

            var repeats = canBeRepetition && game.IsThreefoldRepetition(1);

            game.RemoveFromPositionHashHistory();
            game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
            position.UnmakeMove(move, gameState);

            result.Add((move, repeats));
        }

        return result;
    }

    private static bool CanMoveOnEmptyBoard(int pieceType, int from, int to)
    {
        var fileDifference = Math.Abs(Constants.File(to) - Constants.File(from));
        var rankDifference = Math.Abs(Constants.Rank(to) - Constants.Rank(from));

        var isStraight = fileDifference == 0 || rankDifference == 0;
        var isDiagonal = fileDifference == rankDifference;

        return pieceType switch
        {
            (int)Piece.N => (fileDifference == 1 && rankDifference == 2) || (fileDifference == 2 && rankDifference == 1),
            (int)Piece.B => isDiagonal,
            (int)Piece.R => isStraight,
            (int)Piece.Q => isDiagonal || isStraight,
            (int)Piece.K => Math.Max(fileDifference, rankDifference) == 1,
            _ => false
        };
    }
}
