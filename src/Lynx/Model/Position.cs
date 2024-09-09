using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

using static Lynx.EvaluationConstants;
using static Lynx.EvaluationParams;
using static Lynx.EvaluationPSQTs;

namespace Lynx.Model;

public class Position
{
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

    public BitBoard Queens => PieceBitBoards[(int)Piece.Q] | PieceBitBoards[(int)Piece.q];
    public BitBoard Rooks => PieceBitBoards[(int)Piece.R] | PieceBitBoards[(int)Piece.r];
    public BitBoard Bishops => PieceBitBoards[(int)Piece.B] | PieceBitBoards[(int)Piece.b];
    public BitBoard Knights => PieceBitBoards[(int)Piece.N] | PieceBitBoards[(int)Piece.n];
    public BitBoard Kings => PieceBitBoards[(int)Piece.K] | PieceBitBoards[(int)Piece.k];

    /// <summary>
    /// Beware, half move counter isn't take into account
    /// Use alternative constructor instead and set it externally if relevant
    /// </summary>
    /// <param name="fen"></param>
    public Position(string fen) : this(FENParser.ParseFEN(fen))
    {
    }

    public Position((BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, byte Castle, BoardSquare EnPassant,
        int _/*, int FullMoveCounter*/) parsedFEN)
    {
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

    #region Move making

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameState MakeMove(Move move)
    {
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

        switch (move.SpecialMoveFlag())
        {
            case SpecialMoveType.None:
                {
                    if (move.IsCapture())
                    {
                        var capturedSquare = targetSquare;
                        var capturedPiece = move.CapturedPiece();

                        PieceBitBoards[capturedPiece].PopBit(capturedSquare);
                        OccupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                        UniqueIdentifier ^= ZobristTable.PieceHash(capturedSquare, capturedPiece);
                    }

                    break;
                }
            case SpecialMoveType.DoublePawnPush:
                {
                    var pawnPush = +8 - (oldSide * 16);
                    var enPassantSquare = sourceSquare + pawnPush;
                    Utils.Assert(Constants.EnPassantCaptureSquares.Length > enPassantSquare && Constants.EnPassantCaptureSquares[enPassantSquare] != 0, $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

                    EnPassant = (BoardSquare)enPassantSquare;
                    UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                    break;
                }
            case SpecialMoveType.ShortCastle:
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

                    break;
                }
            case SpecialMoveType.LongCastle:
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

                    break;
                }
            case SpecialMoveType.EnPassant:
                {
                    var oppositePawnIndex = (int)Piece.p - offset;

                    var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    var capturedPiece = oppositePawnIndex;
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");

                    PieceBitBoards[capturedPiece].PopBit(capturedSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedSquare, capturedPiece);

                    break;
                }
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[2] = OccupancyBitBoards[1] | OccupancyBitBoards[0];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

        return new GameState(uniqueIdentifierCopy, enpassantCopy, castleCopy);
        //var clone = new Position(this);
        //clone.UnmakeMove(move, gameState);
        //if (uniqueIdentifierCopy != clone.UniqueIdentifier)
        //{
        //    throw new($"{FEN()}: {uniqueIdentifierCopy} expected, got {clone.UniqueIdentifier} got after Make/Unmake move {move.ToEPDString()}");
        //}
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameState MakeMoveCalculatingCapturedPiece(ref Move move)
    {
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

        switch (move.SpecialMoveFlag())
        {
            case SpecialMoveType.DoublePawnPush:
                {
                    var pawnPush = +8 - (oldSide * 16);
                    var enPassantSquare = sourceSquare + pawnPush;
                    Utils.Assert(Constants.EnPassantCaptureSquares.Length > enPassantSquare && Constants.EnPassantCaptureSquares[enPassantSquare] != 0, $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

                    EnPassant = (BoardSquare)enPassantSquare;
                    UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                    break;
                }
            case SpecialMoveType.ShortCastle:
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

                    break;
                }
            case SpecialMoveType.LongCastle:
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

                    break;
                }

            case SpecialMoveType.EnPassant:
                {
                    var oppositePawnIndex = (int)Piece.p - offset;

                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Utils.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                    UniqueIdentifier ^= ZobristTable.PieceHash(capturedPawnSquare, oppositePawnIndex);
                    move = MoveExtensions.EncodeCapturedPiece(move, oppositePawnIndex);

                    break;
                }
            default:
                {
                    if (move.IsCapture())
                    {
                        var oppositePawnIndex = (int)Piece.p - offset;

                        var limit = (int)Piece.K + oppositePawnIndex;
                        for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                        {
                            if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                            {
                                PieceBitBoards[pieceIndex].PopBit(targetSquare);
                                UniqueIdentifier ^= ZobristTable.PieceHash(targetSquare, pieceIndex);
                                move = MoveExtensions.EncodeCapturedPiece(move, pieceIndex);
                                break;
                            }
                        }

                        OccupancyBitBoards[oppositeSide].PopBit(targetSquare);
                    }

                    break;
                }
        }

        Side = (Side)oppositeSide;
        OccupancyBitBoards[2] = OccupancyBitBoards[1] | OccupancyBitBoards[0];

        // Updating castling rights
        Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        UniqueIdentifier ^= ZobristTable.CastleHash(Castle);

        return new GameState(uniqueIdentifierCopy, enpassantCopy, castleCopy);
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

        switch (move.SpecialMoveFlag())
        {
            case SpecialMoveType.None:
                {
                    if (move.IsCapture())
                    {
                        PieceBitBoards[move.CapturedPiece()].SetBit(targetSquare);
                        OccupancyBitBoards[oppositeSide].SetBit(targetSquare);
                    }

                    break;
                }
            case SpecialMoveType.ShortCastle:
                {
                    var rookSourceSquare = Utils.ShortCastleRookSourceSquare(side);
                    var rookTargetSquare = Utils.ShortCastleRookTargetSquare(side);
                    var rookIndex = (int)Piece.R + offset;

                    PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                    OccupancyBitBoards[side].SetBit(rookSourceSquare);

                    PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                    OccupancyBitBoards[side].PopBit(rookTargetSquare);

                    break;
                }
            case SpecialMoveType.LongCastle:
                {
                    var rookSourceSquare = Utils.LongCastleRookSourceSquare(side);
                    var rookTargetSquare = Utils.LongCastleRookTargetSquare(side);
                    var rookIndex = (int)Piece.R + offset;

                    PieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                    OccupancyBitBoards[side].SetBit(rookSourceSquare);

                    PieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                    OccupancyBitBoards[side].PopBit(rookTargetSquare);

                    break;
                }
            case SpecialMoveType.EnPassant:
                {
                    Debug.Assert(move.IsEnPassant());

                    var oppositePawnIndex = (int)Piece.p - offset;
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];

                    Utils.Assert(OccupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                        $"Expected empty {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);

                    break;
                }
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

        return new GameState(oldUniqueIdentifier, oldEnPassant, byte.MaxValue);
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
            && !IsSquareAttacked(oppositeKingSquare, Side);
    }

    /// <summary>
    /// Lightweight version of <see cref="IsValid"/>
    /// False if the opponent king is in check.
    /// This method is meant to be invoked only after a pseudolegal <see cref="MakeMove(int)"/>.
    /// i.e. it doesn't ensure that both kings are on the board
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool WasProduceByAValidMove()
    {
        var oppositeKingSquare = PieceBitBoards[(int)Piece.k - Utils.PieceOffset(Side)].GetLS1BIndex();

        return !IsSquareAttacked(oppositeKingSquare, Side);
    }

    #endregion

    #region Evaluation

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="Side"/>.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Score, int Phase) StaticEvaluation(int movesWithoutCaptureOrPawnMove = 0)
    {
        //var result = OnlineTablebaseProber.EvaluationSearch(this, movesWithoutCaptureOrPawnMove, cancellationToken);
        //Debug.Assert(result < CheckMateBaseEvaluation, $"position {FEN()} returned tb eval out of bounds: {result}");
        //Debug.Assert(result > -CheckMateBaseEvaluation, $"position {FEN()} returned tb eval out of bounds: {result}");

        //if (result != OnlineTablebaseProber.NoResult)
        //{
        //    return result;
        //}

        int packedScore = 0;
        int gamePhase = 0;

        BitBoard whitePawnAttacks = PieceBitBoards[(int)Piece.P].ShiftUpRight() | PieceBitBoards[(int)Piece.P].ShiftUpLeft();
        BitBoard blackPawnAttacks = PieceBitBoards[(int)Piece.p].ShiftDownRight() | PieceBitBoards[(int)Piece.p].ShiftDownLeft();

        var whiteKing = PieceBitBoards[(int)Piece.K].GetLS1BIndex();
        var blackKing = PieceBitBoards[(int)Piece.k].GetLS1BIndex();

        var whiteBucket = PSQTBucketLayout[whiteKing];
        var blackBucket = PSQTBucketLayout[blackKing ^ 56];

        for (int pieceIndex = (int)Piece.P; pieceIndex < (int)Piece.K; ++pieceIndex)
        {
            // Bitboard copy that we 'empty'
            var bitboard = PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                var pieceSquareIndex = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                packedScore += PSQT(0, whiteBucket, pieceIndex, pieceSquareIndex)
                             + PSQT(1, blackBucket, pieceIndex, pieceSquareIndex);

                gamePhase += GamePhaseByPiece[pieceIndex];

                packedScore += AdditionalPieceEvaluation(whiteBucket, pieceSquareIndex, pieceIndex, (int)Side.White, whiteKing, blackKing, blackPawnAttacks);
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

                packedScore += PSQT(0, blackBucket, pieceIndex, pieceSquareIndex)
                             + PSQT(1, whiteBucket, pieceIndex, pieceSquareIndex);

                gamePhase += GamePhaseByPiece[pieceIndex];

                packedScore -= AdditionalPieceEvaluation(blackBucket, pieceSquareIndex, pieceIndex, (int)Side.Black, blackKing, whiteKing, whitePawnAttacks);
            }
        }

        // Bishop pair bonus
        if (PieceBitBoards[(int)Piece.B].CountBits() >= 2)
        {
            packedScore += BishopPairBonus;
        }

        if (PieceBitBoards[(int)Piece.b].CountBits() >= 2)
        {
            packedScore -= BishopPairBonus;
        }

        // Pieces protected by pawns bonus
        packedScore += PieceProtectedByPawnBonus
            * ((whitePawnAttacks & OccupancyBitBoards[(int)Side.White] /*& (~PieceBitBoards[(int)Piece.P])*/).CountBits()
                - (blackPawnAttacks & OccupancyBitBoards[(int)Side.Black] /*& (~PieceBitBoards[(int)Piece.p])*/).CountBits());

        packedScore += PieceAttackedByPawnPenalty
            * ((blackPawnAttacks & OccupancyBitBoards[(int)Side.White]).CountBits()
                - (whitePawnAttacks & OccupancyBitBoards[(int)Side.Black]).CountBits());

        packedScore += PSQT(0, whiteBucket, (int)Piece.K, whiteKing)
            + PSQT(0, blackBucket, (int)Piece.k, blackKing)
            + PSQT(1, blackBucket, (int)Piece.K, whiteKing)
            + PSQT(1, whiteBucket, (int)Piece.k, blackKing)
            + KingAdditionalEvaluation(whiteKing, (int)Side.White, blackPawnAttacks)
            - KingAdditionalEvaluation(blackKing, (int)Side.Black, whitePawnAttacks);

        const int maxPhase = 24;

        if (gamePhase > maxPhase)    // Early promotions
        {
            gamePhase = maxPhase;
        }

        int totalPawnsCount = PieceBitBoards[(int)Piece.P].CountBits() + PieceBitBoards[(int)Piece.p].CountBits();

        // Pawnless endgames with few pieces
        if (gamePhase <= 3 && totalPawnsCount == 0)
        {
            switch (gamePhase)
            {
                //case 5:
                //    {
                //        // RB vs R, RN vs R - scale it down due to the chances of it being a draw
                //        if (pieceCount[(int)Piece.R] == 1 && pieceCount[(int)Piece.r] == 1)
                //        {
                //            packedScore >>= 1; // /2
                //        }

                //        break;
                //    }
                case 3:
                    {
                        var winningSideOffset = Utils.PieceOffset(packedScore >= 0);

                        if (PieceBitBoards[(int)Piece.N + winningSideOffset].CountBits() == 2)      // NN vs N, NN vs B
                        {
                            return (0, gamePhase);
                        }

                        // Without rooks, only BB vs N is a win and BN vs N can have some chances
                        // Not taking that into account here though, we would need this to rule them out: `pieceCount[(int)Piece.b - winningSideOffset] == 1 || pieceCount[(int)Piece.B + winningSideOffset] <= 1`
                        //if (pieceCount[(int)Piece.R + winningSideOffset] == 0)  // BN vs B, NN vs B, BB vs B, BN vs N, NN vs N
                        //{
                        //    packedScore >>= 1; // /2
                        //}

                        break;
                    }
                case 2:
                    {
                        var whiteKnightsCount = PieceBitBoards[(int)Piece.N].CountBits();

                        if (whiteKnightsCount + PieceBitBoards[(int)Piece.n].CountBits() == 2            // NN vs -, N vs N
                                || whiteKnightsCount + PieceBitBoards[(int)Piece.B].CountBits() == 1)    // B vs N, B vs B
                        {
                            return (0, gamePhase);
                        }

                        break;
                    }
                case 1:
                case 0:
                    {
                        return (0, gamePhase);
                    }
            }
        }

        int endGamePhase = maxPhase - gamePhase;

        var middleGameScore = Utils.UnpackMG(packedScore);
        var endGameScore = Utils.UnpackEG(packedScore);
        var eval = ((middleGameScore * gamePhase) + (endGameScore * endGamePhase)) / maxPhase;

        // Endgame scaling with pawn count, formula yoinked from Sirius
        eval = (int)(eval * ((80 + (totalPawnsCount * 7)) / 128.0));

        eval = ScaleEvalWith50MovesDrawDistance(eval, movesWithoutCaptureOrPawnMove);

        eval = Math.Clamp(eval, MinEval, MaxEval);

        var sideEval = Side == Side.White
            ? eval
            : -eval;

        return (sideEval, gamePhase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int TaperedEvaluation(TaperedEvaluationTerm taperedEvaluationTerm, int phase)
    {
        return ((Utils.UnpackMG(taperedEvaluationTerm) * phase) + (Utils.UnpackEG(taperedEvaluationTerm) * (24 - phase))) / 24;
    }

    /// <summary>
    /// Assuming a current position has no legal moves (<see cref="AllPossibleMoves"/> doesn't produce any <see cref="IsValid"/> position),
    /// this method determines if a position is a result of either a loss by checkmate or a draw by stalemate.
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
            return -CheckMateBaseEvaluation + (CheckmateDepthFactor * ply);
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Doesn't include <see cref="Piece.K"/> and <see cref="Piece.k"/> evaluation
    /// </summary>
    /// <param name="pieceSquareIndex"></param>
    /// <param name="pieceIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int AdditionalPieceEvaluation(int bucket, int pieceSquareIndex, int pieceIndex, int pieceSide, int sameSideKingSquare, int oppositeSideKingSquare, BitBoard enemyPawnAttacks)
    {
        return pieceIndex switch
        {
            (int)Piece.P or (int)Piece.p => PawnAdditionalEvaluation(bucket, pieceSquareIndex, pieceIndex, sameSideKingSquare, oppositeSideKingSquare),
            (int)Piece.R or (int)Piece.r => RookAdditionalEvaluation(pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks),
            (int)Piece.B or (int)Piece.b => BishopAdditionalEvaluation(pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks),
            (int)Piece.N or (int)Piece.n => KnightAdditionalEvaluation(pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks),
            (int)Piece.Q or (int)Piece.q => QueenAdditionalEvaluation(pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks),
            _ => 0
        };
    }

    /// <summary>
    /// Doubled pawns penalty, isolated pawns penalty, passed pawns bonus
    /// </summary>
    /// <param name = "squareIndex" ></ param >
    /// < param name="pieceIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PawnAdditionalEvaluation(int bucket, int squareIndex, int pieceIndex, int sameSideKingSquare, int oppositeSideKingSquare)
    {
        int packedBonus = 0;

        if ((PieceBitBoards[pieceIndex] & Masks.IsolatedPawnMasks[squareIndex]) == default) // isIsolatedPawn
        {
            packedBonus += IsolatedPawnPenalty;
        }

        if ((PieceBitBoards[(int)Piece.p - pieceIndex] & Masks.PassedPawns[pieceIndex][squareIndex]) == default)    // isPassedPawn
        {
            var rank = Constants.Rank[squareIndex];
            if (pieceIndex == (int)Piece.p)
            {
                rank = 7 - rank;
            }

            var friendlyKingDistance = Constants.ChebyshevDistance[squareIndex][sameSideKingSquare];

            var enemyKingDistance = Constants.ChebyshevDistance[squareIndex][oppositeSideKingSquare];

            packedBonus += PassedPawnBonus[bucket][rank]
                + FriendlyKingDistanceToPassedPawnBonus[friendlyKingDistance]
                + EnemyKingDistanceToPassedPawnPenalty[enemyKingDistance];
        }

        return packedBonus;
    }

    /// <summary>
    /// Open and semiopen file bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="pieceIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int RookAdditionalEvaluation(int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks)
    {
        var attacksCount =
            (Attacks.RookAttacks(squareIndex, OccupancyBitBoards[(int)Side.Both])
                & (~(OccupancyBitBoards[pieceSide] | enemyPawnAttacks)))
            .CountBits();

        var packedBonus = RookMobilityBonus[attacksCount];

        const int pawnToRookOffset = (int)Piece.R - (int)Piece.P;

        if (((PieceBitBoards[(int)Piece.P] | PieceBitBoards[(int)Piece.p]) & Masks.FileMasks[squareIndex]) == default)  // isOpenFile
        {
            packedBonus += OpenFileRookBonus;
        }
        else if ((PieceBitBoards[pieceIndex - pawnToRookOffset] & Masks.FileMasks[squareIndex]) == default)  // isSemiOpenFile
        {
            packedBonus += SemiOpenFileRookBonus;
        }

        return packedBonus;
    }

    /// <summary>
    /// Mobility and bishop pair bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="pieceIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int KnightAdditionalEvaluation(int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks)
    {
        var attacksCount =
            (Attacks.KnightAttacks[squareIndex]
                & (~(OccupancyBitBoards[pieceSide] | enemyPawnAttacks)))
            .CountBits();

        return KnightMobilityBonus[attacksCount];
    }

    /// <summary>
    /// Mobility and bishop pair bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="pieceIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BishopAdditionalEvaluation(int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks)
    {
        var attacksCount =
            (Attacks.BishopAttacks(squareIndex, OccupancyBitBoards[(int)Side.Both])
                & (~(OccupancyBitBoards[pieceSide] | enemyPawnAttacks)))
            .CountBits();

        return BishopMobilityBonus[attacksCount];
    }

    /// <summary>
    /// Mobility bonus
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int QueenAdditionalEvaluation(int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks)
    {
        var attacksCount =
            (Attacks.QueenAttacks(squareIndex, OccupancyBitBoards[(int)Side.Both])
                & (~(OccupancyBitBoards[pieceSide] | enemyPawnAttacks)))
            .CountBits();

        return QueenMobilityBonus[attacksCount];
    }

    /// <summary>
    /// Open and semiopenfile penalties, shield bonus, virtual mobility bonus/penalty
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="pieceSide"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int KingAdditionalEvaluation(int squareIndex, int pieceSide, BitBoard enemyPawnAttacks)
    {
        var attacksCount =
            (Attacks.QueenAttacks(squareIndex, OccupancyBitBoards[(int)Side.Both])
            & ~(OccupancyBitBoards[pieceSide] | enemyPawnAttacks)).CountBits();
        int packedBonus = VirtualKingMobilityBonus[attacksCount];

        var kingSideOffset = Utils.PieceOffset(pieceSide);

        if (PieceBitBoards[(int)Piece.r - kingSideOffset] + PieceBitBoards[(int)Piece.q - kingSideOffset] != 0) // areThereOppositeSideRooksOrQueens
        {
            if (((PieceBitBoards[(int)Piece.P] | PieceBitBoards[(int)Piece.p]) & Masks.FileMasks[squareIndex]) == 0)  // isOpenFile
            {
                packedBonus += OpenFileKingPenalty;
            }
            else if ((PieceBitBoards[(int)Piece.P + kingSideOffset] & Masks.FileMasks[squareIndex]) == 0) // isSemiOpenFile
            {
                packedBonus += SemiOpenFileKingPenalty;
            }
        }

        var ownPiecesAroundCount = (Attacks.KingAttacks[squareIndex] & PieceBitBoards[(int)Piece.P + kingSideOffset]).CountBits();

        return packedBonus + (ownPiecesAroundCount * KingShieldBonus);
    }

    /// <summary>
    /// Scales <paramref name="eval"/> with <paramref name="movesWithoutCaptureOrPawnMove"/>, so that
    /// an eval with 100 halfmove counter is half of the value of one with 0 halfmove counter
    /// </summary>
    /// <param name="eval"></param>
    /// <param name="movesWithoutCaptureOrPawnMove"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ScaleEvalWith50MovesDrawDistance(int eval, int movesWithoutCaptureOrPawnMove) =>
        eval * (200 - movesWithoutCaptureOrPawnMove) / 200;

    #endregion

    #region Attacks

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AllAttackersTo(int square, BitBoard occupancy)
    {
        Debug.Assert(square != (int)BoardSquare.noSquare);

        var queens = Queens;
        var rooks = queens | Rooks;
        var bishops = queens | Bishops;

        return (rooks & Attacks.RookAttacks(square, occupancy))
            | (bishops & Attacks.BishopAttacks(square, occupancy))
            | (PieceBitBoards[(int)Piece.p] & Attacks.PawnAttacks[(int)Side.White][square])
            | (PieceBitBoards[(int)Piece.P] & Attacks.PawnAttacks[(int)Side.Black][square])
            | (Knights & Attacks.KnightAttacks[square])
            | (Kings & Attacks.KingAttacks[square]);
    }

    /// <summary>
    /// Overload that has rooks and bishops precalculated for the position
    /// </summary>
    /// <param name="square"></param>
    /// <param name="occupancy"></param>
    /// <param name="rooks">Includes Queen bitboard</param>
    /// <param name="bishops">Includes Queen bitboard</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AllAttackersTo(int square, BitBoard occupancy, BitBoard rooks, BitBoard bishops)
    {
        Debug.Assert(square != (int)BoardSquare.noSquare);

        return (rooks & Attacks.RookAttacks(square, occupancy))
            | (bishops & Attacks.BishopAttacks(square, occupancy))
            | (PieceBitBoards[(int)Piece.p] & Attacks.PawnAttacks[(int)Side.White][square])
            | (PieceBitBoards[(int)Piece.P] & Attacks.PawnAttacks[(int)Side.Black][square])
            | (Knights & Attacks.KnightAttacks[square])
            | (Kings & Attacks.KingAttacks[square]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttackedBySide(int squaredIndex, Side sideToMove) => IsSquareAttacked(squaredIndex, sideToMove);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttacked(int squareIndex, Side sideToMove)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);
        var bothSidesOccupancy = OccupancyBitBoards[(int)Side.Both];

        // I tried to order them from most to least likely - not tested
        return
            IsSquareAttackedByPawns(squareIndex, sideToMoveInt, offset)
            || IsSquareAttackedByKing(squareIndex, offset)
            || IsSquareAttackedByKnights(squareIndex, offset)
            || IsSquareAttackedByBishops(squareIndex, offset, bothSidesOccupancy, out var bishopAttacks)
            || IsSquareAttackedByRooks(squareIndex, offset, bothSidesOccupancy, out var rookAttacks)
            || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInCheck()
    {
        var oppositeSideInt = Utils.OppositeSide(Side);
        var oppositeSideOffset = Utils.PieceOffset(oppositeSideInt);

        var kingSquare = PieceBitBoards[(int)Piece.k - oppositeSideOffset].GetLS1BIndex();

        var bothSidesOccupancy = OccupancyBitBoards[(int)Side.Both];

        // I tried to order them from most to least likely - not tested
        return
            IsSquareAttackedByRooks(kingSquare, oppositeSideOffset, bothSidesOccupancy, out var rookAttacks)
            || IsSquareAttackedByBishops(kingSquare, oppositeSideOffset, bothSidesOccupancy, out var bishopAttacks)
            || IsSquareAttackedByQueens(oppositeSideOffset, bishopAttacks, rookAttacks)
            || IsSquareAttackedByKnights(kingSquare, oppositeSideOffset)
            || IsSquareAttackedByPawns(kingSquare, oppositeSideInt, oppositeSideOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByPawns(int squareIndex, int sideToMove, int offset)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & PieceBitBoards[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByKnights(int squareIndex, int offset)
    {
        return (Attacks.KnightAttacks[squareIndex] & PieceBitBoards[(int)Piece.N + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByKing(int squareIndex, int offset)
    {
        return (Attacks.KingAttacks[squareIndex] & PieceBitBoards[(int)Piece.K + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard bothSidesOccupancy, out BitBoard bishopAttacks)
    {
        bishopAttacks = Attacks.BishopAttacks(squareIndex, bothSidesOccupancy);
        return (bishopAttacks & PieceBitBoards[(int)Piece.B + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard bothSidesOccupancy, out BitBoard rookAttacks)
    {
        rookAttacks = Attacks.RookAttacks(squareIndex, bothSidesOccupancy);
        return (rookAttacks & PieceBitBoards[(int)Piece.R + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks)
    {
        var queenAttacks = Attacks.QueenAttacks(rookAttacks, bishopAttacks);
        return (queenAttacks & PieceBitBoards[(int)Piece.Q + offset]) != default;
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CountPieces() => PieceBitBoards.Sum(b => b.CountBits());

    /// <summary>
    /// Based on Stormphrax
    /// </summary>
    /// <param name="square"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int PieceAt(int square)
    {
        var bit = BitBoardExtensions.SquareBit(square);

        Side color;

        if ((OccupancyBitBoards[(int)Side.Black] & bit) != default)
        {
            color = Side.Black;
        }
        else if ((OccupancyBitBoards[(int)Side.White] & bit) != default)
        {
            color = Side.White;
        }
        else
        {
            return (int)Piece.None;
        }

        var offset = Utils.PieceOffset(color);

        for (int pieceIndex = offset; pieceIndex < 6 + offset; ++pieceIndex)
        {
            if (!(PieceBitBoards[pieceIndex] & bit).Empty())
            {
                return pieceIndex;
            }
        }

        Debug.Fail($"Bit set in {Side} occupancy bitboard, but not piece found");

        return (int)Piece.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string FEN(int halfMovesWithoutCaptureOrPawnMove = 0, int fullMoveClock = 1)
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

                var pieceRepresentation = IsSquareAttacked(squareIndex, sideToMove)
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
