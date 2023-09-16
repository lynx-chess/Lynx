using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx.Model;

public class Position
{
    //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public long UniqueIdentifier { get; private set; }

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards { get; }

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards { get; }

    public Side Side { get; private set; }

    public BoardSquare EnPassant { get; private set; }

    /// <summary>
    /// See <see cref="<CastlingRights"/>
    /// </summary>
    public byte Castle { get; private set; }

    public Position(string fen) : this(FENParser.ParseFEN(fen))
    {
    }

    public Position((bool Success, BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, byte Castle, BoardSquare EnPassant,
        int HalfMoveClock, int FullMoveCounter) parsedFEN)
    {
        PieceBitBoards = parsedFEN.PieceBitBoards;
        OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
        Side = parsedFEN.Side;
        Castle = parsedFEN.Castle;
        EnPassant = parsedFEN.EnPassant;

        UniqueIdentifier = ZobristTable.PositionHash(this);

        // TODO: half move and full move counters
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

    /// <summary>
    /// Slower than make-unmake move method
    /// </summary>
    /// <param name="position"></param>
    /// <param name="move"></param>
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
            Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameState MakeMove(Move move)
    {
        sbyte capturedPiece = -1;
        byte castleCopy = Castle;
        BoardSquare enpassantCopy = EnPassant;
        long uniqueIdentifierCopy = UniqueIdentifier;

        var oldSide = (int)Side;
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

        PieceBitBoards[piece].PopBit(sourceSquare);
        OccupancyBitBoards[oldSide].PopBit(sourceSquare);

        PieceBitBoards[newPiece].SetBit(targetSquare);
        OccupancyBitBoards[oldSide].SetBit(targetSquare);

        UniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ ZobristTable.PieceHash(sourceSquare, piece)
            ^ ZobristTable.PieceHash(targetSquare, newPiece)
            ^ ZobristTable.EnPassantHash((int)EnPassant)            // We clear the existing enpassant square, if any
            ^ ZobristTable.CastleHash(Castle);                      // We clear the existing castle rights

        EnPassant = BoardSquare.noSquare;
        if (move.IsCapture())
        {
            var oppositePawnIndex = (int)Piece.p - offset;

            if (move.IsEnPassant())
            {
                var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                capturedPiece = (sbyte)oppositePawnIndex;
            }
            else
            {
                var limit = (int)Piece.K + oppositePawnIndex;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        PieceBitBoards[pieceIndex].PopBit(targetSquare);
                        UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                        capturedPiece = (sbyte)pieceIndex;
                        break;
                    }
                }

                OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
            }
        }
        else if (move.IsDoublePawnPush())
        {
            var pawnPush = +8 - (oldSide * 16);
            var enPassantSquare = sourceSquare + pawnPush;
            Utils.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

            EnPassant = (BoardSquare)enPassantSquare;
            UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);
        }
        else if (move.IsShortCastle())
        {
            var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
            var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
            var rookIndex = (int)Piece.R + offset;

            PieceBitBoards[rookIndex].PopBit(rookSourceSquare);
            OccupancyBitBoards[oldSide].PopBit(rookSourceSquare);

            PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
            OccupancyBitBoards[oldSide].SetBit(rookTargetSquare);

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
            OccupancyBitBoards[oldSide].PopBit(rookSourceSquare);

            PieceBitBoards[rookIndex].SetBit(rookTargetSquare);
            OccupancyBitBoards[oldSide].SetBit(rookTargetSquare);

            UniqueIdentifier ^=
                ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[2] = OccupancyBitBoards[1] | OccupancyBitBoards[0];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

        return new GameState(capturedPiece, castleCopy, enpassantCopy, uniqueIdentifierCopy);
        //var clone = new Position(this);
        //clone.UnmakeMove(move, gameState);
        //if (uniqueIdentifierCopy != clone.UniqueIdentifier)
        //{
        //    throw new($"{FEN()}: {uniqueIdentifierCopy} expected, got {clone.UniqueIdentifier} got after Make/Unmake move {move.ToEPDString()}");
        //}
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnmakeMove(Move move, GameState gameState)
    {
        var oppositeSide = (int)Side;
        var side = Utils.OppositeSide(oppositeSide);
        Side = (Side)side;
        var offset = Utils.PieceOffset(side);

        int sourceSquare = move.SourceSquare();
        int targetSquare = move.TargetSquare();
        int piece = move.Piece();
        int promotedPiece = move.PromotedPiece();

        var newPiece = piece;
        if (promotedPiece != default)
        {
            newPiece = promotedPiece;
        }

        PieceBitBoards[newPiece].PopBit(targetSquare);
        OccupancyBitBoards[side].PopBit(targetSquare);

        PieceBitBoards[piece].SetBit(sourceSquare);
        OccupancyBitBoards[side].SetBit(sourceSquare);

        if (move.IsCapture())
        {
            var oppositePawnIndex = (int)Piece.p - offset;

            if (move.IsEnPassant())
            {
                var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                Utils.Assert(OccupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                    $"Expected empty {capturedPawnSquare}");

                PieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                OccupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);
            }
            else
            {
                PieceBitBoards[gameState.CapturedPiece].SetBit(targetSquare);
                OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
            }
        }
        else if (move.IsShortCastle())
        {
            var rookSourceSquare = Utils.ShortCastleRookSourceSquare(side);
            var rookTargetSquare = Utils.ShortCastleRookTargetSquare(side);
            var rookIndex = (int)Piece.R + offset;

            PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
            OccupancyBitBoards[side].SetBit(rookSourceSquare);

            PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
            OccupancyBitBoards[side].PopBit(rookTargetSquare);
        }
        else if (move.IsLongCastle())
        {
            var rookSourceSquare = Utils.LongCastleRookSourceSquare(side);
            var rookTargetSquare = Utils.LongCastleRookTargetSquare(side);
            var rookIndex = (int)Piece.R + offset;

            PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
            OccupancyBitBoards[side].SetBit(rookSourceSquare);

            PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
            OccupancyBitBoards[side].PopBit(rookTargetSquare);
        }

        OccupancyBitBoards[2] = OccupancyBitBoards[1] | OccupancyBitBoards[0];

        // Updating saved values
        Castle = gameState.Castle;
        EnPassant = gameState.EnPassant;
        UniqueIdentifier = gameState.ZobristKey;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameState MakeNullMove()
    {
        Side = (Side)Utils.OppositeSide(Side);
        var oldEnPassant = EnPassant;
        var oldUniqueIdentifier = UniqueIdentifier;
        EnPassant = BoardSquare.noSquare;

        UniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ ZobristTable.EnPassantHash((int)oldEnPassant);

        return new GameState(-1, byte.MaxValue, oldEnPassant, oldUniqueIdentifier);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnMakeNullMove(GameState gameState)
    {
        Side = (Side)Utils.OppositeSide(Side);
        EnPassant = gameState.EnPassant;

        UniqueIdentifier = gameState.ZobristKey;
    }

    /// <summary>
    /// False if any of the kings has been captured, or if the opponent king is in check.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsValid()
    {
        var offset = Utils.PieceOffset(Side);

        var kingBitBoard = PieceBitBoards[(int)Piece.K + offset];
        var kingSquare = kingBitBoard == default ? -1 : kingBitBoard.GetLS1BIndex();

        var oppositeKingBitBoard = PieceBitBoards[(int)Piece.k - offset];
        var oppositeKingSquare = oppositeKingBitBoard == default ? -1 : oppositeKingBitBoard.GetLS1BIndex();

        return kingSquare >= 0 && oppositeKingSquare >= 0
            && !Attacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
    }

    /// <summary>
    /// Lightweight version of <see cref="IsValid"/>
    /// False if the opponent king is in check.
    /// This method is meant to be invoked only after a pseudolegal <see cref="Position(Position, Move)"/> or <see cref="MakeMove(int)"/>.
    /// i.e. it doesn't ensure that both kings are on the board
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool WasProduceByAValidMove()
    {
        var oppositeKingSquare = PieceBitBoards[(int)Piece.k - Utils.PieceOffset(Side)].GetLS1BIndex();

        return !Attacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInCheck()
    {
        var kingSquare = PieceBitBoards[(int)Piece.K + Utils.PieceOffset(Side)].GetLS1BIndex();
        var oppositeSide = (Side)Utils.OppositeSide(Side);

        return Attacks.IsSquareInCheck(kingSquare, oppositeSide, PieceBitBoards, OccupancyBitBoards);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string FEN(int halfMovesWithoutCaptureOrPawnMove = 0, int fullMoveClock = 1)
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

        sb.Append(' ').Append(halfMovesWithoutCaptureOrPawnMove).Append(' ').Append(fullMoveClock);

        return sb.ToString();
    }

    public int CountPieces() => PieceBitBoards.Sum(b => b.CountBits());

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="Side"/>.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int StaticEvaluation(int movesWithoutCaptureOrPawnMove, CancellationToken cancellationToken = default)
    {
        var result = OnlineTablebaseProber.EvaluationSearch(this, movesWithoutCaptureOrPawnMove, cancellationToken);
        Debug.Assert(result < EvaluationConstants.CheckMateBaseEvaluation, $"position {FEN()} returned tb eval out of bounds: {result}");
        Debug.Assert(result > -EvaluationConstants.CheckMateBaseEvaluation, $"position {FEN()} returned tb eval out of bounds: {result}");

        if (result != OnlineTablebaseProber.NoResult)
        {
            return result;
        }

        var pieceCount = new int[PieceBitBoards.Length];

        int middleGameScore = 0;
        int endGameScore = 0;
        int gamePhase = 0;
        int eval = 0;

        for (int pieceIndex = (int)Piece.P; pieceIndex < (int)Piece.K; ++pieceIndex)
        {
            // Bitboard copy that we 'empty'
            var bitboard = PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                var pieceSquareIndex = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                middleGameScore += EvaluationConstants.MiddleGameTable[pieceIndex, pieceSquareIndex];
                endGameScore += EvaluationConstants.EndGameTable[pieceIndex, pieceSquareIndex];
                gamePhase += EvaluationConstants.GamePhaseByPiece[pieceIndex];

                ++pieceCount[pieceIndex];

                eval += AdditionalPieceEvaluation(pieceSquareIndex, pieceIndex);
            }
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex < (int)Piece.k; ++pieceIndex)
        {
            // Bitboard copy that we 'empty'
            var bitboard = PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                var pieceSquareIndex = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                middleGameScore += EvaluationConstants.MiddleGameTable[pieceIndex, pieceSquareIndex];
                endGameScore += EvaluationConstants.EndGameTable[pieceIndex, pieceSquareIndex];
                gamePhase += EvaluationConstants.GamePhaseByPiece[pieceIndex];

                ++pieceCount[pieceIndex];

                eval -= AdditionalPieceEvaluation(pieceSquareIndex, pieceIndex);
            }
        }

        var whiteKing = PieceBitBoards[(int)Piece.K].GetLS1BIndex();
        middleGameScore += EvaluationConstants.MiddleGameTable[(int)Piece.K, whiteKing];
        endGameScore += EvaluationConstants.EndGameTable[(int)Piece.K, whiteKing];
        eval += KingAdditionalEvaluation(whiteKing, Side.White, pieceCount);

        var blackKing = PieceBitBoards[(int)Piece.k].GetLS1BIndex();
        middleGameScore += EvaluationConstants.MiddleGameTable[(int)Piece.k, blackKing];
        endGameScore += EvaluationConstants.EndGameTable[(int)Piece.k, blackKing];
        eval -= KingAdditionalEvaluation(blackKing, Side.Black, pieceCount);

        // Check if drawn position due to lack of material
        if (endGameScore >= 0)
        {
            bool whiteCannotWin = pieceCount[(int)Piece.P] == 0 && pieceCount[(int)Piece.Q] == 0 && pieceCount[(int)Piece.R] == 0
                && (pieceCount[(int)Piece.B] + pieceCount[(int)Piece.N] == 1                // B or N
                    || (pieceCount[(int)Piece.B] == 0 && pieceCount[(int)Piece.N] == 2));   // N+N

            if (whiteCannotWin)
            {
                return 0;
            }
        }
        else
        {
            bool blackCannotWin = pieceCount[(int)Piece.p] == 0 && pieceCount[(int)Piece.q] == 0 && pieceCount[(int)Piece.r] == 0
                && (pieceCount[(int)Piece.b] + pieceCount[(int)Piece.n] == 1                // B or N
                    || (pieceCount[(int)Piece.b] == 0 && pieceCount[(int)Piece.n] == 2));   // N+N

            if (blackCannotWin)
            {
                return 0;
            }
        }

        const int maxPhase = 24;

        if (gamePhase > maxPhase)    // Early promotions
        {
            gamePhase = maxPhase;
        }

        int endGamePhase = maxPhase - gamePhase;
        //_logger.Trace("Phase: {0}/24", gamePhase);

        eval += ((middleGameScore * gamePhase) + (endGameScore * endGamePhase)) / 24;

        if (pieceCount[(int)Piece.B] >= 2)
        {
            eval += Configuration.EngineSettings.BishopPairMaxBonus * endGamePhase / 24;
        }
        if (pieceCount[(int)Piece.b] >= 2)
        {
            eval -= Configuration.EngineSettings.BishopPairMaxBonus * endGamePhase / 24;
        }

        return Side == Side.White
            ? eval
            : -eval;
    }

    /// <summary>
    /// Assuming a current position has no legal moves (<see cref="AllPossibleMoves"/> doesn't produce any <see cref="IsValid"/> position),
    /// this method determines if a position is a result of either a loss by Checkmate or a draw by stalemate.
    /// NegaMax style
    /// </summary>
    /// <param name="ply">Modulates the output, favouring positions with lower ply (i.e. Checkmate in less moves)</param>
    /// <param name="isInCheck"></param>
    /// <returns>At least <see cref="CheckMateEvaluation"/> if Position.Side lost (more extreme values when <paramref name="ply"/> increases)
    /// or 0 if Position.Side was stalemated</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int EvaluateFinalPosition(int ply, bool isInCheck)
    {
        if (isInCheck)
        {
            // Checkmate evaluation, but not as bad/shallow as it looks like since we're already searching at a certain depth
            return -EvaluationConstants.CheckMateBaseEvaluation + (EvaluationConstants.CheckmateDepthFactor * ply);
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Doesn't include <see cref="Piece.K"/> and <see cref="Piece.k"/> evaluation since this method is called
    /// on the fly while going through the existing pieces and <see cref="KingAdditionalEvaluation(int, Side, int[])"/>
    /// requires oppposite piece count, which in case of white king it's not available when this method is invoked
    /// </summary>
    /// <param name="pieceSquareIndex"></param>
    /// <param name="pieceIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int AdditionalPieceEvaluation(int pieceSquareIndex, int pieceIndex)
    {
        return pieceIndex switch
        {
            (int)Piece.P or (int)Piece.p => PawnAdditionalEvaluation(pieceSquareIndex, pieceIndex),
            (int)Piece.R or (int)Piece.r => RookAdditonalEvaluation(pieceSquareIndex, pieceIndex),
            (int)Piece.B or (int)Piece.b => BishopAdditionalEvaluation(pieceSquareIndex),
            (int)Piece.Q or (int)Piece.q => QueenAdditionalEvaluation(pieceSquareIndex),
            _ => 0
        };
    }

    /// <summary>
    /// Doubled pawns penalty, isolated pawns penalty, passed pawns bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="pieceIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PawnAdditionalEvaluation(int squareIndex, int pieceIndex)
    {
        var bonus = 0;

        var doublePawnsCount = (PieceBitBoards[pieceIndex] & Masks.FileMasks[squareIndex]).CountBits();
        if (doublePawnsCount > 1)
        {
            bonus -= doublePawnsCount * Configuration.EngineSettings.DoubledPawnPenalty;
        }

        if ((PieceBitBoards[pieceIndex] & Masks.IsolatedPawnMasks[squareIndex]) == default) // isIsolatedPawn
        {
            bonus -= Configuration.EngineSettings.IsolatedPawnPenalty;
        }

        if ((PieceBitBoards[(int)Piece.p - pieceIndex] & Masks.PassedPawns[pieceIndex][squareIndex]) == default)    // isPassedPawn
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

    /// <summary>
    /// Open and semiopen file bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="pieceIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int RookAdditonalEvaluation(int squareIndex, int pieceIndex)
    {
        const int pawnToRookOffset = (int)Piece.R - (int)Piece.P;

        if (((PieceBitBoards[(int)Piece.P] | PieceBitBoards[(int)Piece.p]) & Masks.FileMasks[squareIndex]) == default)  // isOpenFile
        {
            return Configuration.EngineSettings.OpenFileRookBonus;
        }

        if ((PieceBitBoards[pieceIndex - pawnToRookOffset] & Masks.FileMasks[squareIndex]) == default)  // isSemiOpenFile
        {
            return Configuration.EngineSettings.SemiOpenFileRookBonus;
        }

        return 0;
    }

    /// <summary>
    /// Mobility bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BishopAdditionalEvaluation(int squareIndex)
    {
        return Configuration.EngineSettings.BishopMobilityBonus
            * Attacks.BishopAttacks(squareIndex, OccupancyBitBoards[(int)Side.Both]).CountBits();
    }

    /// <summary>
    /// Mobility bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int QueenAdditionalEvaluation(int squareIndex)
    {
        return Configuration.EngineSettings.QueenMobilityBonus
            * Attacks.QueenAttacks(squareIndex, OccupancyBitBoards[(int)Side.Both]).CountBits();
    }

    /// <summary>
    /// Open and semiopenfile penalties, shield bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="kingSide"></param>
    /// <param name="pieceCount"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int KingAdditionalEvaluation(int squareIndex, Side kingSide, int[] pieceCount)
    {
        var bonus = 0;
        var kingSideOffset = Utils.PieceOffset(kingSide);

        if (pieceCount[(int)Piece.r - kingSideOffset] + pieceCount[(int)Piece.q - kingSideOffset] != default) // areThereOppositeSideRooksOrQueens
        {
            if (((PieceBitBoards[(int)Piece.P] | PieceBitBoards[(int)Piece.p]) & Masks.FileMasks[squareIndex]) == default)  // isOpenFile
            {
                bonus -= Configuration.EngineSettings.OpenFileKingPenalty;
            }
            else if ((PieceBitBoards[(int)Piece.P + kingSideOffset] & Masks.FileMasks[squareIndex]) == default) // isSemiOpenFile
            {
                bonus -= Configuration.EngineSettings.SemiOpenFileKingPenalty;
            }
        }

        return bonus + Configuration.EngineSettings.KingShieldBonus *
            (Attacks.KingAttacks[squareIndex] & OccupancyBitBoards[(int)kingSide]).CountBits();
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
        Console.WriteLine($"    FEN:\t{FEN()}");
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
