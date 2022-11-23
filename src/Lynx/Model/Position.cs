using NLog;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx.Model;

public sealed class Position
{
    internal const int DepthFactor = 1_000_000;

    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private string? _fen;

    public string FEN
    {
        get => _fen ??= CalculateFEN();
        init => _fen = value;
    }

    public long UniqueIdentifier { get; }

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards { get; }

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards { get; }

    public Side Side { get; }

    public BoardSquare EnPassant { get; }

    public int Castle { get; }

    public Position(string fen)
    {
        _fen = null;    // Otherwise halfmove and fullmove numbers may interfere whenever FEN is being used as key of a dictionary
        var parsedFEN = FENParser.ParseFEN(fen);

        if (!parsedFEN.Success)
        {
            _logger.Error($"Error parsing FEN {fen}");
        }

        PieceBitBoards = parsedFEN.PieceBitBoards;
        OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
        Side = parsedFEN.Side;
        Castle = parsedFEN.Castle;
        EnPassant = parsedFEN.EnPassant;

        UniqueIdentifier = ZobristTable.PositionHash(this);
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    /// <param name="position"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Position(Position position)
    {
        UniqueIdentifier = position.UniqueIdentifier;
        PieceBitBoards = new BitBoard[12];
        Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

        OccupancyBitBoards = new BitBoard[3];
        Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

        Side = position.Side;
        Castle = position.Castle;
        EnPassant = position.EnPassant;
    }

    /// <summary>
    /// Null moves constructor
    /// </summary>
    /// <param name="position"></param>
    /// <param name="nullMove"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable RCS1163, IDE0060 // Unused parameter.
    public Position(Position position, bool nullMove)
    {
        UniqueIdentifier = position.UniqueIdentifier;
        PieceBitBoards = new BitBoard[12];
        Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

        OccupancyBitBoards = new BitBoard[3];
        Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

        Side = (Side)Utils.OppositeSide(position.Side);
        Castle = position.Castle;
        EnPassant = BoardSquare.noSquare;

        UniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ ZobristTable.EnPassantHash((int)position.EnPassant);
    }
#pragma warning restore RCS1163 // Unused parameter.

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Position(Position position, Move move) : this(position)
    {
        var oldSide = Side;
        var offset = Utils.PieceOffset(oldSide);
        var oppositeSide = Utils.OppositeSide(oldSide);

        int sourceSquare = move.SourceSquare();
        int targetSquare = move.TargetSquare();
        int piece = move.Piece();
        int promotedPiece = move.PromotedPiece();

        var newPiece = piece;
        if (promotedPiece != default)
        {
            newPiece = promotedPiece;
        }

        EnPassant = BoardSquare.noSquare;

        PieceBitBoards[piece].PopBit(sourceSquare);
        OccupancyBitBoards[(int)Side].PopBit(sourceSquare);

        PieceBitBoards[newPiece].SetBit(targetSquare);
        OccupancyBitBoards[(int)Side].SetBit(targetSquare);

        UniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ ZobristTable.PieceHash(sourceSquare, piece)
            ^ ZobristTable.PieceHash(targetSquare, newPiece)
            ^ ZobristTable.EnPassantHash((int)position.EnPassant)
            ^ ZobristTable.CastleHash(position.Castle);

        if (move.IsCapture())
        {
            var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
            var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

            if (move.IsEnPassant())
            {
                var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
            }
            else
            {
                var limit = (int)Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
                        UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                        break;
                    }
                }

                OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
            }
        }
        else if (move.IsDoublePawnPush())
        {
            var pawnPush = +8 - ((int)oldSide * 16);
            var enPassantSquare = sourceSquare + pawnPush;
            Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

            EnPassant = (BoardSquare)enPassantSquare;
            UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
        }
        else if (move.IsShortCastle())
        {
            var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
            var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
            var rookIndex = (int)Piece.R + offset;

            PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
            OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

            PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
            OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

            UniqueIdentifier ^=
                ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
        }
        else if (move.IsLongCastle())
        {
            var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
            var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
            var rookIndex = (int)Piece.R + offset;

            PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
            OccupancyBitBoards[(int)Side].PopBit(rookSourceSquare);

            PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
            OccupancyBitBoards[(int)Side].SetBit(rookTargetSquare);

            UniqueIdentifier ^=
                ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[(int)Side.Both] = OccupancyBitBoards[(int)Side.White] | OccupancyBitBoards[(int)Side.Black];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        UniqueIdentifier ^= ZobristTable.CastleHash(Castle);
    }

    /// <summary>
    /// False if any of the kings has been captured, or if the opponent king is in check.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsValid()
    {
        var kingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset(Side)];
        var kingSquare = kingBitBoard == default ? -1 : kingBitBoard.GetLS1BIndex();

        var oppositeKingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset((Side)Utils.OppositeSide(Side))];
        var oppositeKingSquare = oppositeKingBitBoard == default ? -1 : oppositeKingBitBoard.GetLS1BIndex();

        return kingSquare >= 0 && oppositeKingSquare >= 0
            && !Attacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
    }

    /// <summary>
    /// Lightweight version of <see cref="IsValid"/>
    /// False if the opponent king is in check.
    /// This method is meant to be invoked only after <see cref="Position(Position, Move)"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool WasProduceByAValidMove()
    {
        var oppositeKingBitBoard = PieceBitBoards[(int)Piece.K + Utils.PieceOffset((Side)Utils.OppositeSide(Side))];
        var oppositeKingSquare = oppositeKingBitBoard == default ? -1 : oppositeKingBitBoard.GetLS1BIndex();

        return oppositeKingSquare >= 0 && !Attacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInCheck()
    {
        var kingSquare = PieceBitBoards[(int)Piece.K + Utils.PieceOffset(Side)].GetLS1BIndex();
        var oppositeSide = (Side)Utils.OppositeSide(Side);

        return Attacks.IsSquareInCheck(kingSquare, oppositeSide, PieceBitBoards, OccupancyBitBoards);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal string CalculateFEN()
    {
        var sb = new StringBuilder(100);

        var squaresPerFile = 0;

        int squaresWithoutPiece = 0;
        int lengthBeforeSlash = sb.Length;
        for (int square = 0; square < 64; ++square)
        {
            int foundPiece = -1;
            for (var pieceBoardIndex = 0; pieceBoardIndex < 12; ++pieceBoardIndex)
            {
                if (PieceBitBoards[pieceBoardIndex].GetBit(square))
                {
                    foundPiece = pieceBoardIndex;
                    break;
                }
            }

            if (foundPiece != -1)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                sb.Append(Constants.AsciiPieces[foundPiece]);
            }
            else
            {
                ++squaresWithoutPiece;
            }

            squaresPerFile = (squaresPerFile + 1) % 8;
            if (squaresPerFile == 0)
            {
                if (squaresWithoutPiece != 0)
                {
                    sb.Append(squaresWithoutPiece);
                    squaresWithoutPiece = 0;
                }

                if (square != 63)
                {
                    if (sb.Length == lengthBeforeSlash)
                    {
                        sb.Append('8');
                    }
                    sb.Append('/');
                    lengthBeforeSlash = sb.Length;
                    squaresWithoutPiece = 0;
                }
            }
        }

        sb.Append(' ');
        sb.Append(Side == Side.White ? 'w' : 'b');

        sb.Append(' ');
        var length = sb.Length;

        if ((Castle & (int)CastlingRights.WK) != default)
        {
            sb.Append('K');
        }
        if ((Castle & (int)CastlingRights.WQ) != default)
        {
            sb.Append('Q');
        }
        if ((Castle & (int)CastlingRights.BK) != default)
        {
            sb.Append('k');
        }
        if ((Castle & (int)CastlingRights.BQ) != default)
        {
            sb.Append('q');
        }

        if (sb.Length == length)
        {
            sb.Append('-');
        }

        sb.Append(' ');

        sb.Append(EnPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)EnPassant]);

        sb.Append(" 0 1");

        return sb.ToString();
    }

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="Side"/>.
    /// </summary>
    /// <param name="positionHistory"></param>
    /// <param name="movesWithoutCaptureOrPawnMove"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int StaticEvaluation(Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove)
    {
        var eval = 0;

        if (positionHistory.Values.Any(val => val >= 3) || movesWithoutCaptureOrPawnMove >= 100)
        {
            return eval;
        }

        int whiteMaterialEval = 0, blackMaterialEval = 0;
        var pieceCount = new int[PieceBitBoards.Length];

        bool IsEndgameForWhite() => pieceCount[(int)Piece.q] == 0;
        bool IsEndgameForBlack() => pieceCount[(int)Piece.Q] == 0;

        for (int pieceIndex = 0; pieceIndex < Constants.SideLimit - 1; ++pieceIndex)
        {
            // Bitboard copy that we 'empty'
            var bitboard = PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                var pieceSquareIndex = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                ++pieceCount[pieceIndex];

                // Material evaluation
                whiteMaterialEval += EvaluationConstants.MaterialScore[pieceIndex];

                // Positional evaluation
                eval += EvaluationConstants.PositionalScore[pieceIndex][pieceSquareIndex];

                eval += CustomPieceEvaluation(pieceSquareIndex, pieceIndex);
            }
        }

        for (int pieceIndex = Constants.SideLimit; pieceIndex < PieceBitBoards.Length - 1; ++pieceIndex)
        {
            // Bitboard copy that we 'empty'
            var bitboard = PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                var pieceSquareIndex = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                ++pieceCount[pieceIndex];

                // Material evaluation
                blackMaterialEval += EvaluationConstants.MaterialScore[pieceIndex];

                // Positional evaluation
                eval += EvaluationConstants.PositionalScore[pieceIndex][pieceSquareIndex];

                eval -= CustomPieceEvaluation(pieceSquareIndex, pieceIndex);
            }
        }

        //++pieceCount[Constants.WhiteKingIndex];
        var whiteKing = PieceBitBoards[(int)Piece.K].GetLS1BIndex();
        eval += IsEndgameForWhite()
            ? EvaluationConstants.EndgamePositionalScore[(int)Piece.K][whiteKing]
            : EvaluationConstants.PositionalScore[(int)Piece.K][whiteKing];
        eval += KingEvaluation(whiteKing, Side.White, pieceCount);

        //++pieceCount[Constants.BlackKingIndex];
        var blackKing = PieceBitBoards[(int)Piece.k].GetLS1BIndex();
        eval += IsEndgameForBlack()
            ? EvaluationConstants.EndgamePositionalScore[(int)Piece.k][blackKing]
            : EvaluationConstants.PositionalScore[(int)Piece.k][blackKing];
        eval -= KingEvaluation(blackKing, Side.Black, pieceCount);

        eval += whiteMaterialEval + blackMaterialEval;

        // Check if drawn position due to lack of material
        if (eval >= 0)
        {
            bool whiteCannotWin = pieceCount[(int)Piece.P] == 0
                && (whiteMaterialEval <= EvaluationConstants.MaterialScore[(int)Piece.B]            // B or N
                    || whiteMaterialEval == 2 * EvaluationConstants.MaterialScore[(int)Piece.N]);   // N+N

            if (whiteCannotWin)
            {
                eval = 0;
            }
        }
        else
        {
            bool blackCannotWin = pieceCount[(int)Piece.p] == 0
                && (blackMaterialEval >= EvaluationConstants.MaterialScore[(int)Piece.b]            // b or n
                    || blackMaterialEval == 2 * EvaluationConstants.MaterialScore[(int)Piece.n]);   // n+n

            if (blackCannotWin)
            {
                eval = 0;
            }
        }

        return Side == Side.White
            ? eval
            : -eval;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CustomPieceEvaluation(int pieceSquareIndex, int pieceIndex)
    {
        return pieceIndex switch
        {
            (int)Piece.P or (int)Piece.p => PawnEvaluation(pieceSquareIndex, pieceIndex),
            (int)Piece.R or (int)Piece.r => RookEvaluation(pieceSquareIndex, pieceIndex),
            (int)Piece.B or (int)Piece.b => BishopEvaluation(pieceSquareIndex),
            _ => 0
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PawnEvaluation(int squareIndex, int pieceIndex)
    {
        var bonus = 0;

        var doublePawnsCount = (PieceBitBoards[pieceIndex] & Masks.FileMasks[squareIndex]).CountBits();
        if (doublePawnsCount > 1)
        {
            bonus -= doublePawnsCount * Configuration.EngineSettings.DoubledPawnPenalty;
        }

        bool IsIsolatedPawn() => (PieceBitBoards[pieceIndex] & Masks.IsolatedPawnMasks[squareIndex]) == default;
        if (IsIsolatedPawn())
        {
            bonus -= Configuration.EngineSettings.IsolatedPawnPenalty;
        }

        bool IsPassedPawn() => (PieceBitBoards[(int)Piece.p - pieceIndex] & Masks.PassedPawns[pieceIndex][squareIndex]) == default;
        if (IsPassedPawn())
        {
            var rank = Constants.Rank[squareIndex];
            if (pieceIndex == (int)Piece.p)
            {
                rank = 7 - rank;
            }
            bonus += Configuration.EngineSettings.PassedPawnBonus[rank];
        }

        return bonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int RookEvaluation(int squareIndex, int pieceIndex)
    {
        const int pawnToRookOffset = (int)Piece.R - (int)Piece.P;
        bool IsOpenFile() => ((PieceBitBoards[(int)Piece.P] | PieceBitBoards[(int)Piece.p]) & Masks.FileMasks[squareIndex]) == default;
        bool IsSemiOpenFile() => (PieceBitBoards[pieceIndex - pawnToRookOffset] & Masks.FileMasks[squareIndex]) == default;

        if (IsOpenFile())
        {
            return Configuration.EngineSettings.OpenFileRookBonus;
        }

        if (IsSemiOpenFile())
        {
            return Configuration.EngineSettings.SemiOpenFileRookBonus;
        }

        return 0;
    }

    int KingEvaluation(int squareIndex, Side pieceSide, int[] pieceCount)
    {
        var bonus = 0;
        var oppositeSide = Utils.OppositeSide(pieceSide);
        var opposieSideOffset = Utils.PieceOffset(oppositeSide);

        bool areThereOppositeSideRooksOrQueens() => pieceCount[(int)Piece.R + opposieSideOffset] + pieceCount[(int)Piece.Q + opposieSideOffset] != default;
        if (areThereOppositeSideRooksOrQueens())
        {
            bool isOpenFile() => ((PieceBitBoards[(int)Piece.P] | PieceBitBoards[(int)Piece.p]) & Masks.FileMasks[squareIndex]) == default;
            bool isSemiOpenFile() => (PieceBitBoards[(int)Piece.p - opposieSideOffset] & Masks.FileMasks[squareIndex]) == default;
            if (isOpenFile())
            {
                bonus -= Configuration.EngineSettings.OpenFileKingPenalty;
            }
            else if (isSemiOpenFile())
            {
                bonus -= Configuration.EngineSettings.SemiOpenFileKingPenalty;
            }
        }

        return bonus += (Attacks.KingAttacks[squareIndex] & OccupancyBitBoards[(int)pieceSide]).CountBits() * Configuration.EngineSettings.KingShieldBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BishopEvaluation(int squareIndex)
    {
        return Configuration.EngineSettings.BishopMobilityBonus
            * Attacks.BishopAttacks(squareIndex, OccupancyBitBoards[(int)Side.Both]).CountBits();
    }

    int KingEvaluation(int squareIndex, Side pieceSide, int[] pieceCount)
    {
        var bonus = 0;
        var oppositeSide = Utils.OppositeSide(pieceSide);
        var opposieSideOffset = Utils.PieceOffset(oppositeSide);

        bool areThereOppositeSideRooksOrQueens() => pieceCount[(int)Piece.R + opposieSideOffset] + pieceCount[(int)Piece.Q + opposieSideOffset] != default;
        if (areThereOppositeSideRooksOrQueens())
        {
            bool isOpenFile() => ((PieceBitBoards[(int)Piece.P] | PieceBitBoards[(int)Piece.p]) & Masks.FileMasks[squareIndex]) == default;
            bool isSemiOpenFile() => (PieceBitBoards[(int)Piece.p - opposieSideOffset] & Masks.FileMasks[squareIndex]) == default;
            if (isOpenFile())
            {
                bonus -= Configuration.EngineSettings.OpenFileKingPenalty;
            }
            else if (isSemiOpenFile())
            {
                bonus -= Configuration.EngineSettings.SemiOpenFileKingPenalty;
            }
        }

        return bonus += Configuration.EngineSettings.KingShieldBonus *
            (Attacks.KingAttacks[squareIndex] & OccupancyBitBoards[(int)pieceSide]).CountBits();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Move> AllPossibleMoves(Move[]? movePool = null) => MoveGenerator.GenerateAllMoves(this, movePool);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Move> AllCapturesMoves(Move[]? movePool = null) => MoveGenerator.GenerateAllMoves(this, movePool, capturesOnly: true);

    /// <summary>
    /// Assuming a current position has no legal moves (<see cref="AllPossibleMoves"/> doesn't produce any <see cref="IsValid"/> position),
    /// this method determines if a position is a result of either a loss by Checkmate or a draw by stalemate.
    /// NegaMax style
    /// </summary>
    /// <param name="depth">Modulates the output, favouring positions with lower depth left (i.e. Checkmate in less moves)</param>
    /// <param name="positionHistory"></param>
    /// <param name="movesWithoutCaptureOrPawnMove"></param>
    /// <returns>At least <see cref="CheckMateEvaluation"/> if Position.Side lost (more extreme values when <paramref name="depth"/> increases)
    /// or 0 if Position.Side was stalemated</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int EvaluateFinalPosition(int depth, bool isInCheck, Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove)
    {
        if (positionHistory.Values.Any(val => val >= 3) || movesWithoutCaptureOrPawnMove >= 100)
        {
            return 0;
        }

        if (isInCheck)
        {
            return -EvaluationConstants.CheckMateEvaluation + (DepthFactor * depth);
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Combines <see cref="PieceBitBoards"/>, <see cref="Side"/>, <see cref="Castle"/> and <see cref="EnPassant"/>
    /// into a human-friendly representation
    /// </summary>
    public void Print()
    {
        const string separator = "____________________________________________________";
        Console.WriteLine(separator + Environment.NewLine);

        for (var rank = 0; rank < 8; ++rank)
        {
            for (var file = 0; file < 8; ++file)
            {
                if (file == 0)
                {
                    Console.Write($"{8 - rank}  ");
                }

                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                var piece = -1;

                for (int bbIndex = 0; bbIndex < PieceBitBoards.Length; ++bbIndex)
                {
                    if (PieceBitBoards[bbIndex].GetBit(squareIndex))
                    {
                        piece = bbIndex;
                    }
                }

                var pieceRepresentation = piece == -1
                    ? '.'
                    : Constants.AsciiPieces[piece];

                Console.Write($" {pieceRepresentation}");
            }

            Console.WriteLine();
        }

        Console.Write("\n    a b c d e f g h\n");

#pragma warning disable RCS1214 // Unnecessary interpolated string.
        Console.WriteLine();
        Console.WriteLine($"    Side:\t{Side}");
        Console.WriteLine($"    Enpassant:\t{(EnPassant == BoardSquare.noSquare ? "no" : Constants.Coordinates[(int)EnPassant])}");
        Console.WriteLine($"    Castling:\t" +
            $"{((Castle & (int)CastlingRights.WK) != default ? 'K' : '-')}" +
            $"{((Castle & (int)CastlingRights.WQ) != default ? 'Q' : '-')} | " +
            $"{((Castle & (int)CastlingRights.BK) != default ? 'k' : '-')}" +
            $"{((Castle & (int)CastlingRights.BQ) != default ? 'q' : '-')}"
            );
        Console.WriteLine($"    FEN:\t{FEN}");
#pragma warning restore RCS1214 // Unnecessary interpolated string.

        Console.WriteLine(separator);
    }

    public void PrintAttackedSquares(Side sideToMove)
    {
        const string separator = "____________________________________________________";
        Console.WriteLine(separator);

        for (var rank = 0; rank < 8; ++rank)
        {
            for (var file = 0; file < 8; ++file)
            {
                if (file == 0)
                {
                    Console.Write($"{8 - rank}  ");
                }

                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                var pieceRepresentation = Attacks.IsSquaredAttacked(squareIndex, sideToMove, PieceBitBoards, OccupancyBitBoards)
                    ? '1'
                    : '.';

                Console.Write($" {pieceRepresentation}");
            }

            Console.WriteLine();
        }

        Console.Write("\n    a b c d e f g h\n");
        Console.WriteLine(separator);
    }
}
