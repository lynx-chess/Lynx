using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

using static Lynx.EvaluationConstants;
using static Lynx.EvaluationPSQTs;

namespace Lynx.Model;

public partial class Position : IDisposable
{
    private bool _disposedValue;

    private int _stackCounter;
    private readonly State[] _stateStack;

    private State _state;

    private ulong[] _pieceBitBoards;
    private ulong[] _occupancyBitBoards;
    private int[] _board;

#pragma warning disable S3887, CA1051
    private readonly byte[] _castlingRightsUpdateConstants;
    public readonly ulong[] KingsideCastlingFreeSquares;
    public readonly ulong[] KingsideCastlingNonAttackedSquares;
    public readonly ulong[] QueensideCastlingFreeSquares;
    public readonly ulong[] QueensideCastlingNonAttackedSquares;

#pragma warning disable IDE1006 // Naming Styles
    internal int WhiteShortCastle;
    internal int WhiteLongCastle;
    internal int BlackShortCastle;
    internal int BlackLongCastle;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore S3887, CA1051

#if DEBUG
    private readonly int[] _initialKingsideRookSquares;
    private readonly int[] _initialQueensideRookSquares;
    private readonly int[] _initialKingSquares;
#endif

    private Side _side;

#pragma warning disable RCS1085 // Use auto-implemented property

    public ulong UniqueIdentifier => _state.UniqueIdentifier;
    public ulong KingPawnUniqueIdentifier => _state.KingPawnUniqueIdentifier;
    public ulong[] NonPawnHash => _state.NonPawnHash;
    public ulong MinorHash => _state.MinorHash;
    public ulong MajorHash => _state.MajorHash;

    public BitBoard[] PieceBitBoards => _pieceBitBoards;
    public BitBoard[] OccupancyBitBoards => _occupancyBitBoards;
    public int[] Board => _board;
    public Side Side => _side;
    public BoardSquare EnPassant => _state.EnPassant;

    /// <summary>
    /// See <see cref="<CastlingRights"/>
    /// </summary>
    public byte Castle { get => _state.Castle; private set => _state.Castle = value; }

#pragma warning restore RCS1085 // Use auto-implemented property

    public BitBoard Queens
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitBoards[(int)Piece.Q] | _pieceBitBoards[(int)Piece.q];
    }

    public BitBoard Rooks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitBoards[(int)Piece.R] | _pieceBitBoards[(int)Piece.r];
    }

    public BitBoard Bishops
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitBoards[(int)Piece.B] | _pieceBitBoards[(int)Piece.b];
    }

    public BitBoard Knights
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitBoards[(int)Piece.N] | _pieceBitBoards[(int)Piece.n];
    }

    public BitBoard Kings
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitBoards[(int)Piece.K] | _pieceBitBoards[(int)Piece.k];
    }

    public int WhiteKingSquare => _pieceBitBoards[(int)Piece.K].GetLS1BIndex();
    public int BlackKingSquare => _pieceBitBoards[(int)Piece.k].GetLS1BIndex();

    public int InitialKingSquare(int side) =>
        side == (int)Side.White
            ? WhiteShortCastle.SourceSquare()
            : BlackShortCastle.SourceSquare();

    private Position()
        : this(Constants.MaxNumberMovesInAGame)
    {
    }

    private Position(int stateStackLength)
    {
        _pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
        _occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
        _board = ArrayPool<int>.Shared.Rent(64);
        _castlingRightsUpdateConstants = ArrayPool<byte>.Shared.Rent(64);

        KingsideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);
        KingsideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);
        QueensideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);
        QueensideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);

        _stateStack = new State[stateStackLength];
        for (int i = 0; i < _stateStack.Length; ++i)
        {
            _stateStack[i] = new();
        }

        _state = _stateStack[0];
        _stackCounter = 0;

#if DEBUG
        _initialKingSquares = ArrayPool<int>.Shared.Rent(2);
        _initialKingsideRookSquares = ArrayPool<int>.Shared.Rent(2);
        _initialQueensideRookSquares = ArrayPool<int>.Shared.Rent(2);
#endif
    }

    /// <summary>
    /// Beware, half move counter isn't take into account
    /// Use alternative constructor instead and set it externally if relevant
    /// </summary>
    public Position(string fen)
        : this()
    {
        PopulateFrom(FENParser.ParseFEN(fen));
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    public Position(Position position)
        : this()
    {
        ResetTo(position);
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    public Position(Position position, int stateStackLength)
        : this(stateStackLength)
    {
        ResetTo(position);
    }

    public void PopulateFrom(ParseFENResult parsedFEN)
    {
        _pieceBitBoards = parsedFEN.PieceBitBoards;
        _occupancyBitBoards = parsedFEN.OccupancyBitBoards;
        _board = parsedFEN.Board;

        _side = parsedFEN.Side;
        _state.Castle = parsedFEN.Castle;
        _state.EnPassant = parsedFEN.EnPassant;

#pragma warning disable S3366 // "this" should not be exposed from constructors
        _state.NonPawnHash[(int)Side.White] = ZobristTable.NonPawnSideHash(this, (int)Side.White);
        _state.NonPawnHash[(int)Side.Black] = ZobristTable.NonPawnSideHash(this, (int)Side.Black);

        _state.MinorHash = ZobristTable.MinorHash(this);
        _state.MajorHash = ZobristTable.MajorHash(this);
        _state.KingPawnUniqueIdentifier = ZobristTable.KingPawnHash(this);

        _state.UniqueIdentifier = ZobristTable.PositionHash(this, _state.KingPawnUniqueIdentifier, _state.NonPawnHash[(int)Side.White], _state.NonPawnHash[(int)Side.Black]);

        Debug.Assert(_state.UniqueIdentifier == ZobristTable.PositionHash(this));
#pragma warning restore S3366 // "this" should not be exposed from constructors

        _state.IsIncrementalEval = false;

        Array.Fill(_castlingRightsUpdateConstants, Constants.NoUpdateCastlingRight, 0, 64);

        var whiteKingSquare = WhiteKingSquare;
        var blackKingSquare = BlackKingSquare;

        _castlingRightsUpdateConstants[whiteKingSquare] = Constants.WhiteKingCastlingRight;
        _castlingRightsUpdateConstants[blackKingSquare] = Constants.BlackKingCastlingRight;

        var castlingData = parsedFEN.CastlingData;

        var whiteKingsideRook = castlingData.WhiteKingsideRook;
        if (whiteKingsideRook != CastlingData.DefaultValues)
        {
            _castlingRightsUpdateConstants[whiteKingsideRook] = Constants.WhiteKingSideRookCastlingRight;
        }

        var whiteQueensideRook = castlingData.WhiteQueensideRook;
        if (whiteQueensideRook != CastlingData.DefaultValues)
        {
            _castlingRightsUpdateConstants[whiteQueensideRook] = Constants.WhiteQueenSideRookCastlingRight;
        }

        var blackKingsideRook = castlingData.BlackKingsideRook;
        if (blackKingsideRook != CastlingData.DefaultValues)
        {
            _castlingRightsUpdateConstants[blackKingsideRook] = Constants.BlackKingSideRookCastlingRight;
        }

        var blackQueensideRook = castlingData.BlackQueensideRook;
        if (blackQueensideRook != CastlingData.DefaultValues)
        {
            _castlingRightsUpdateConstants[blackQueensideRook] = Constants.BlackQueenSideRookCastlingRight;
        }

        KingsideCastlingNonAttackedSquares[(int)Side.White] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingSquare, Constants.WhiteKingShortCastleSquare);
        KingsideCastlingNonAttackedSquares[(int)Side.Black] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingSquare, Constants.BlackKingShortCastleSquare);

        QueensideCastlingNonAttackedSquares[(int)Side.White] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingSquare, Constants.WhiteKingLongCastleSquare);
        QueensideCastlingNonAttackedSquares[(int)Side.Black] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingSquare, Constants.BlackKingLongCastleSquare);

        // This could be simplified/hardcoded for standard chess, see FreeAndNonAttackedSquares
        var whiteKingsideFreeMask = KingsideCastlingNonAttackedSquares[(int)Side.White]
            | BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingsideRook, Constants.WhiteRookShortCastleSquare);
        whiteKingsideFreeMask.PopBit(whiteKingSquare);
        whiteKingsideFreeMask.PopBit(whiteKingsideRook);

        var blackKingsideFreeMask = KingsideCastlingNonAttackedSquares[(int)Side.Black]
            | BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingsideRook, Constants.BlackRookShortCastleSquare);
        blackKingsideFreeMask.PopBit(blackKingSquare);
        blackKingsideFreeMask.PopBit(blackKingsideRook);

        KingsideCastlingFreeSquares[(int)Side.White] = whiteKingsideFreeMask;
        KingsideCastlingFreeSquares[(int)Side.Black] = blackKingsideFreeMask;

        // This could be simplified/hardcoded for standard chess, see FreeAndNonAttackedSquares
        var whiteQueensideFreeMask = QueensideCastlingNonAttackedSquares[(int)Side.White]
            | BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteQueensideRook, Constants.WhiteRookLongCastleSquare);
        whiteQueensideFreeMask.PopBit(whiteKingSquare);
        whiteQueensideFreeMask.PopBit(whiteQueensideRook);

        var blackQueensideFreeMask = QueensideCastlingNonAttackedSquares[(int)Side.Black]
            | BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackQueensideRook, Constants.BlackRookLongCastleSquare);
        blackQueensideFreeMask.PopBit(blackKingSquare);
        blackQueensideFreeMask.PopBit(blackQueensideRook);

        QueensideCastlingFreeSquares[(int)Side.White] = whiteQueensideFreeMask;
        QueensideCastlingFreeSquares[(int)Side.Black] = blackQueensideFreeMask;

        // Usual encoding for standard chess, King to target square
        if (!Configuration.EngineSettings.IsChess960)
        {
            WhiteShortCastle = MoveExtensions.EncodeShortCastle(whiteKingSquare, Constants.WhiteKingShortCastleSquare, (int)Piece.K);
            WhiteLongCastle = MoveExtensions.EncodeLongCastle(whiteKingSquare, Constants.WhiteKingLongCastleSquare, (int)Piece.K);

            BlackShortCastle = MoveExtensions.EncodeShortCastle(blackKingSquare, Constants.BlackKingShortCastleSquare, (int)Piece.k);
            BlackLongCastle = MoveExtensions.EncodeLongCastle(blackKingSquare, Constants.BlackKingLongCastleSquare, (int)Piece.k);
        }
        // KxR encoding for DFRC
        else
        {
            WhiteShortCastle = MoveExtensions.EncodeShortCastle(whiteKingSquare, whiteKingsideRook, (int)Piece.K);
            WhiteLongCastle = MoveExtensions.EncodeLongCastle(whiteKingSquare, whiteQueensideRook, (int)Piece.K);

            BlackShortCastle = MoveExtensions.EncodeShortCastle(blackKingSquare, blackKingsideRook, (int)Piece.k);
            BlackLongCastle = MoveExtensions.EncodeLongCastle(blackKingSquare, blackQueensideRook, (int)Piece.k);
        }

#if DEBUG
        _initialKingSquares[(int)Side.White] = whiteKingSquare;
        _initialKingSquares[(int)Side.Black] = blackKingSquare;

        _initialKingsideRookSquares[(int)Side.White] = whiteKingsideRook;
        _initialKingsideRookSquares[(int)Side.Black] = blackKingsideRook;

        _initialQueensideRookSquares[(int)Side.White] = whiteQueensideRook;
        _initialQueensideRookSquares[(int)Side.Black] = blackQueensideRook;
#endif

        Validate();
    }

    public void ResetTo(Position position)
    {
        Debug.Assert(position._state != null);

        // TODO see if we can avoid
        for (int i = 0; i < _stateStack.Length; ++i)
        {
            _stateStack[i] = new();
        }
        _stateStack[0].SetupFromPrevious(position._state);
        _state = _stateStack[0];
        _stackCounter = 0;

        Array.Copy(position._pieceBitBoards, _pieceBitBoards, 12);
        Array.Copy(position._occupancyBitBoards, _occupancyBitBoards, 3);
        Array.Copy(position._board, _board, 64);

        _side = position._side;

        Array.Copy(position._castlingRightsUpdateConstants, _castlingRightsUpdateConstants, 64);

        KingsideCastlingFreeSquares[(int)Side.White] = position.KingsideCastlingFreeSquares[(int)Side.White];
        KingsideCastlingFreeSquares[(int)Side.Black] = position.KingsideCastlingFreeSquares[(int)Side.Black];

        KingsideCastlingNonAttackedSquares[(int)Side.White] = position.KingsideCastlingNonAttackedSquares[(int)Side.White];
        KingsideCastlingNonAttackedSquares[(int)Side.Black] = position.KingsideCastlingNonAttackedSquares[(int)Side.Black];

        QueensideCastlingFreeSquares[(int)Side.White] = position.QueensideCastlingFreeSquares[(int)Side.White];
        QueensideCastlingFreeSquares[(int)Side.Black] = position.QueensideCastlingFreeSquares[(int)Side.Black];

        QueensideCastlingNonAttackedSquares[(int)Side.White] = position.QueensideCastlingNonAttackedSquares[(int)Side.White];
        QueensideCastlingNonAttackedSquares[(int)Side.Black] = position.QueensideCastlingNonAttackedSquares[(int)Side.Black];

        WhiteShortCastle = position.WhiteShortCastle;
        WhiteLongCastle = position.WhiteLongCastle;
        BlackShortCastle = position.BlackShortCastle;
        BlackLongCastle = position.BlackLongCastle;

#if DEBUG
        _initialKingSquares[(int)Side.White] = position._initialKingSquares[(int)Side.White];
        _initialKingSquares[(int)Side.Black] = position._initialKingSquares[(int)Side.Black];

        _initialKingsideRookSquares[(int)Side.White] = position._initialKingsideRookSquares[(int)Side.White];
        _initialKingsideRookSquares[(int)Side.Black] = position._initialKingsideRookSquares[(int)Side.Black];

        _initialQueensideRookSquares[(int)Side.White] = position._initialQueensideRookSquares[(int)Side.White];
        _initialQueensideRookSquares[(int)Side.Black] = position._initialQueensideRookSquares[(int)Side.Black];
#endif

        Validate();
    }

    #region Move making

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MakeMove(Move move)
    {
        Debug.Assert(ZobristTable.PositionHash(this) == _state.UniqueIdentifier);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.White) == _state.NonPawnHash[(int)Side.White]);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.Black) == _state.NonPawnHash[(int)Side.Black]);
        Debug.Assert(ZobristTable.MinorHash(this) == _state.MinorHash);
        Debug.Assert(ZobristTable.MajorHash(this) == _state.MajorHash);

        var oldState = _state;

        ++_stackCounter;
        _state = _stateStack[_stackCounter];
        _state.SetupFromPrevious(oldState);
        _state.EnPassant = BoardSquare.noSquare;

        var oldSide = (int)_side;
        var offset = Utils.PieceOffset(oldSide);
        var oppositeSide = Utils.OppositeSide(oldSide);

        int sourceSquare = move.SourceSquare();
        int targetSquare = move.TargetSquare();
        int piece = move.Piece();
        int promotedPiece = move.PromotedPiece();

        var newPiece = piece;
        int extraPhaseIfIncremental = 0;
        if (promotedPiece != default)
        {
            newPiece = promotedPiece;
            extraPhaseIfIncremental = GamePhaseByPiece[promotedPiece]; // - GamePhaseByPiece[piece];
        }

        _pieceBitBoards[piece].PopBit(sourceSquare);
        _occupancyBitBoards[oldSide].PopBit(sourceSquare);
        _board[sourceSquare] = (int)Piece.None;

        _pieceBitBoards[newPiece].SetBit(targetSquare);
        _occupancyBitBoards[oldSide].SetBit(targetSquare);
        _board[targetSquare] = newPiece;

        var sourcePieceHash = ZobristTable.PieceHash(sourceSquare, piece);
        var targetPieceHash = ZobristTable.PieceHash(targetSquare, newPiece);
        var fullPieceMovementHash = sourcePieceHash ^ targetPieceHash;

        _state.UniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ fullPieceMovementHash
            ^ ZobristTable.EnPassantHash((int)oldState.EnPassant)            // We clear the existing enpassant square, if any
            ^ ZobristTable.CastleHash(oldState.Castle);                      // We clear the existing castle rights

        if (piece == (int)Piece.P || piece == (int)Piece.p)
        {
            _state.KingPawnUniqueIdentifier ^= sourcePieceHash;       // We remove pawn from start square

            if (promotedPiece == default)
            {
                _state.KingPawnUniqueIdentifier ^= targetPieceHash;   // We add pawn again to end square
            }
            else
            {
                // In case of promotion, the promoted piece won't be a pawn or a king, so no need to update the KingPawn hash with it, just to remove the pawn (done right above)
                // We do need to update the NonPawn hash
                _state.NonPawnHash[oldSide] ^= targetPieceHash;       // We add piece piece to the end square

                if (Utils.IsMinorPiece(newPiece))
                {
                    _state.MinorHash ^= targetPieceHash;
                }
                else if (Utils.IsMajorPiece(newPiece))
                {
                    _state.MajorHash ^= targetPieceHash;
                }
            }
        }
        else
        {
            _state.NonPawnHash[oldSide] ^= fullPieceMovementHash;

            if (piece == (int)Piece.K || piece == (int)Piece.k)
            {
                // King (and castling) moves require calculating king buckets twice and recalculating all related parameters, so skipping incremental eval for those cases for now
                // No need to check for move.IsCastle(), see CastlingMovesAreKingMoves test
                _state.IsIncrementalEval = false;

                _state.KingPawnUniqueIdentifier ^= fullPieceMovementHash;
            }
            else if (Utils.IsMinorPiece(piece))
            {
                _state.MinorHash ^= fullPieceMovementHash;
            }
            else if (Utils.IsMajorPiece(piece))
            {
                _state.MajorHash ^= fullPieceMovementHash;
            }
        }

        // _incrementalEvalAccumulator updates
        if (_state.IsIncrementalEval)
        {
            var whiteKing = _pieceBitBoards[(int)Piece.K].GetLS1BIndex();
            var blackKing = _pieceBitBoards[(int)Piece.k].GetLS1BIndex();
            var whiteBucket = PSQTBucketLayout[whiteKing];
            var blackBucket = PSQTBucketLayout[blackKing ^ 56];

            int sameSideBucket = whiteBucket;
            int oppositeSideBucket = blackBucket;
            if (_side == Side.Black)
            {
                (sameSideBucket, oppositeSideBucket) = (oppositeSideBucket, sameSideBucket);
            }

            _state.IncrementalEvalAccumulator -= PSQT(sameSideBucket, oppositeSideBucket, piece, sourceSquare);
            _state.IncrementalEvalAccumulator += PSQT(sameSideBucket, oppositeSideBucket, newPiece, targetSquare);

            _state.IncrementalPhaseAccumulator += extraPhaseIfIncremental;

            // No need to check for castling if it's incremental eval
            switch (move.SpecialMoveFlag())
            {
                case SpecialMoveType.None:
                    {
                        var capturedPiece = move.CapturedPiece();
                        if (capturedPiece != (int)Piece.None)
                        {
                            var capturedSquare = targetSquare;

                            _pieceBitBoards[capturedPiece].PopBit(capturedSquare);
                            _occupancyBitBoards[oppositeSide].PopBit(capturedSquare);

                            var capturedPieceHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                            _state.UniqueIdentifier ^= capturedPieceHash;

                            // Kings can't be captured
                            if (capturedPiece == (int)Piece.P || capturedPiece == (int)Piece.p)
                            {
                                _state.KingPawnUniqueIdentifier ^= capturedPieceHash;
                            }
                            else
                            {
                                _state.NonPawnHash[oppositeSide] ^= capturedPieceHash;

                                if (Utils.IsMinorPiece(capturedPiece))
                                {
                                    _state.MinorHash ^= capturedPieceHash;
                                }
                                else if (Utils.IsMajorPiece(capturedPiece))
                                {
                                    _state.MajorHash ^= capturedPieceHash;
                                }
                            }

                            _state.IncrementalEvalAccumulator -= PSQT(oppositeSideBucket, sameSideBucket, capturedPiece, capturedSquare);

                            _state.IncrementalPhaseAccumulator -= GamePhaseByPiece[capturedPiece];
                        }

                        break;
                    }
                case SpecialMoveType.DoublePawnPush:
                    {
                        var pawnPush = +8 - (oldSide * 16);
                        var enPassantSquare = sourceSquare + pawnPush;
                        Utils.Assert(Constants.EnPassantCaptureSquares.Length > enPassantSquare && Constants.EnPassantCaptureSquares[enPassantSquare] != 0, $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

                        _state.EnPassant = (BoardSquare)enPassantSquare;
                        _state.UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                        break;
                    }
                case SpecialMoveType.EnPassant:
                    {
                        var oppositePawnIndex = (int)Piece.p - offset;

                        var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                        var capturedPiece = oppositePawnIndex;
                        Utils.Assert(_pieceBitBoards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");

                        _pieceBitBoards[capturedPiece].PopBit(capturedSquare);
                        _occupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                        _board[capturedSquare] = (int)Piece.None;

                        var capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                        _state.UniqueIdentifier ^= capturedPawnHash;
                        _state.KingPawnUniqueIdentifier ^= capturedPawnHash;

                        _state.IncrementalEvalAccumulator -= PSQT(oppositeSideBucket, sameSideBucket, capturedPiece, capturedSquare);

                        //_incrementalPhaseAccumulator -= GamePhaseByPiece[capturedPiece];
                        break;
                    }
            }
        }
        // No _incrementalEvalAccumulator updates
        else
        {
            switch (move.SpecialMoveFlag())
            {
                case SpecialMoveType.None:
                    {
                        var capturedPiece = move.CapturedPiece();

                        if (capturedPiece != (int)Piece.None)
                        {
                            var capturedSquare = targetSquare;

                            _pieceBitBoards[capturedPiece].PopBit(capturedSquare);
                            _occupancyBitBoards[oppositeSide].PopBit(capturedSquare);

                            ulong capturedPieceHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                            _state.UniqueIdentifier ^= capturedPieceHash;

                            // Kings can't be captured
                            if (capturedPiece == (int)Piece.P || capturedPiece == (int)Piece.p)
                            {
                                _state.KingPawnUniqueIdentifier ^= capturedPieceHash;
                            }
                            else
                            {
                                _state.NonPawnHash[oppositeSide] ^= capturedPieceHash;

                                if (Utils.IsMinorPiece(capturedPiece))
                                {
                                    _state.MinorHash ^= capturedPieceHash;
                                }
                                else if (Utils.IsMajorPiece(capturedPiece))
                                {
                                    _state.MajorHash ^= capturedPieceHash;
                                }
                            }
                        }

                        break;
                    }
                case SpecialMoveType.DoublePawnPush:
                    {
                        var pawnPush = +8 - (oldSide * 16);
                        var enPassantSquare = sourceSquare + pawnPush;
                        Utils.Assert(Constants.EnPassantCaptureSquares.Length > enPassantSquare && Constants.EnPassantCaptureSquares[enPassantSquare] != 0, $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

                        _state.EnPassant = (BoardSquare)enPassantSquare;
                        _state.UniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                        break;
                    }
                case SpecialMoveType.ShortCastle:
                    {
                        var rookSourceSquare = Configuration.EngineSettings.IsChess960
                            ? targetSquare
                            : Utils.ShortCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitBoards[rookIndex].PopBit(rookSourceSquare);

                        var kingTargetSquare = Utils.KingShortCastleSquare(oldSide);

                        if (Configuration.EngineSettings.IsChess960)
                        {
                            // In DFRC castling moves are encoded as KxR, so the target square in the move isn't really the king target square
                            // We need to revert the incorrect changes + apply the right ones
                            // This could be avoided by adding a branch above for all moves and set the right target square for DFRC
                            // But that hurts performance, see https://github.com/lynx-chess/Lynx/pull/2043
                            _pieceBitBoards[newPiece].PopBit(targetSquare);
                            _occupancyBitBoards[oldSide].PopBit(targetSquare);
                            _board[targetSquare] = (int)Piece.None;
                            var hashToRevert = ZobristTable.PieceHash(targetSquare, newPiece);

                            _pieceBitBoards[newPiece].SetBit(kingTargetSquare);
                            _occupancyBitBoards[oldSide].SetBit(kingTargetSquare);
                            _board[kingTargetSquare] = newPiece;
                            var hashToApply = ZobristTable.PieceHash(kingTargetSquare, newPiece);

                            var hashFix = hashToRevert ^ hashToApply;

                            _state.UniqueIdentifier ^= hashFix;
                            _state.NonPawnHash[oldSide] ^= hashFix;
                            _state.KingPawnUniqueIdentifier ^= hashFix;
                        }

                        // In DFRC the square where the rook was could be occupied by the king after castling
                        // This guard could maybe be removed if we ever move the Sets after the switch, same as we did in Unmake
                        if (rookSourceSquare != kingTargetSquare)
                        {
                            _occupancyBitBoards[oldSide].PopBit(rookSourceSquare);
                            _board[rookSourceSquare] = (int)Piece.None;
                        }

                        _pieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                        _occupancyBitBoards[oldSide].SetBit(rookTargetSquare);
                        _board[rookTargetSquare] = rookIndex;

                        var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        _state.UniqueIdentifier ^= hashChange;
                        _state.NonPawnHash[oldSide] ^= hashChange;
                        _state.MajorHash ^= hashChange;

                        break;
                    }
                case SpecialMoveType.LongCastle:
                    {
                        var rookSourceSquare = Configuration.EngineSettings.IsChess960
                            ? targetSquare
                            : Utils.LongCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitBoards[rookIndex].PopBit(rookSourceSquare);

                        var kingTargetSquare = Utils.KingLongCastleSquare(oldSide);

                        if (Configuration.EngineSettings.IsChess960)
                        {
                            // In DFRC castling moves are encoded as KxR, so the target square in the move isn't really the king target square
                            // We need to revert the incorrect changes + apply the right ones
                            // This could be avoided by adding a branch above for all moves and set the right target square for DFRC
                            // But that hurts performance, see https://github.com/lynx-chess/Lynx/pull/2043
                            _pieceBitBoards[newPiece].PopBit(targetSquare);
                            _occupancyBitBoards[oldSide].PopBit(targetSquare);
                            _board[targetSquare] = (int)Piece.None;
                            var hashToRevert = ZobristTable.PieceHash(targetSquare, newPiece);

                            _pieceBitBoards[newPiece].SetBit(kingTargetSquare);
                            _occupancyBitBoards[oldSide].SetBit(kingTargetSquare);
                            _board[kingTargetSquare] = newPiece;
                            var hashToApply = ZobristTable.PieceHash(kingTargetSquare, newPiece);

                            var hashFix = hashToRevert ^ hashToApply;

                            _state.UniqueIdentifier ^= hashFix;
                            _state.NonPawnHash[oldSide] ^= hashFix;
                            _state.KingPawnUniqueIdentifier ^= hashFix;
                        }

                        // In DFRC the square where the rook was could be occupied by the king after castling
                        // This guard could maybe be removed if we ever move the Sets after the switch, same as we did in Unmake
                        if (rookSourceSquare != kingTargetSquare)
                        {
                            _occupancyBitBoards[oldSide].PopBit(rookSourceSquare);
                            _board[rookSourceSquare] = (int)Piece.None;
                        }

                        _pieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                        _occupancyBitBoards[oldSide].SetBit(rookTargetSquare);
                        _board[rookTargetSquare] = rookIndex;

                        var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        _state.UniqueIdentifier ^= hashChange;
                        _state.NonPawnHash[oldSide] ^= hashChange;
                        _state.MajorHash ^= hashChange;

                        break;
                    }
                case SpecialMoveType.EnPassant:
                    {
                        var oppositePawnIndex = (int)Piece.p - offset;

                        var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                        var capturedPiece = oppositePawnIndex;
                        Utils.Assert(_pieceBitBoards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");

                        _pieceBitBoards[capturedPiece].PopBit(capturedSquare);
                        _occupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                        _board[capturedSquare] = (int)Piece.None;

                        ulong capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                        _state.UniqueIdentifier ^= capturedPawnHash;
                        _state.KingPawnUniqueIdentifier ^= capturedPawnHash;

                        break;
                    }
            }
        }

        _side = (Side)oppositeSide;
        _occupancyBitBoards[2] = _occupancyBitBoards[1] | _occupancyBitBoards[0];

        // Updating castling rights
        _state.Castle &= _castlingRightsUpdateConstants[sourceSquare];
        _state.Castle &= _castlingRightsUpdateConstants[targetSquare];

        _state.UniqueIdentifier ^= ZobristTable.CastleHash(_state.Castle);

        Debug.Assert(ZobristTable.PositionHash(this) == _state.UniqueIdentifier);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.White) == _state.NonPawnHash[(int)Side.White]);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.Black) == _state.NonPawnHash[(int)Side.Black]);
        Debug.Assert(ZobristTable.MinorHash(this) == _state.MinorHash);
        Debug.Assert(ZobristTable.MajorHash(this) == _state.MajorHash);
        Debug.Assert(Math.Min(MaxPhase, PhaseFromScratch()) == Phase());

        // KingPawn hash assert won't work due to PassedPawnBonusNoEnemiesAheadBonus
        //Debug.Assert(ZobristTable.PawnKingHash(this) != _kingPawnUniqueIdentifier && WasProduceByAValidMove());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnmakeMove(Move move)
    {
        var oppositeSide = (int)_side;
        var side = Utils.OppositeSide(oppositeSide);
        _side = (Side)side;
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

        _pieceBitBoards[newPiece].PopBit(targetSquare);
        _occupancyBitBoards[side].PopBit(targetSquare);
        _board[targetSquare] = (int)Piece.None;

        // We purposely delay the sets here until after the switch

        switch (move.SpecialMoveFlag())
        {
            case SpecialMoveType.None:
                {
                    var capturedPiece = move.CapturedPiece();

                    if (capturedPiece != (int)Piece.None)
                    {
                        _pieceBitBoards[capturedPiece].SetBit(targetSquare);
                        _occupancyBitBoards[oppositeSide].SetBit(targetSquare);
                        _board[targetSquare] = capturedPiece;
                    }

                    break;
                }
            case SpecialMoveType.ShortCastle:
                {
                    int rookSourceSquare;
                    if (Configuration.EngineSettings.IsChess960)
                    {
                        // In DFRC castling moves are encoded as KxR, so the target square in the move isn't really the king target square
                        // However, that target square can only be potentially occupied by the castling rook, so all the ops done over it
                        // have already been undone by the rook ops above, or don't matter (removing the king from the target square, where it isn't anyway)

                        // However, the kings needs to be removed from the real target square, providing that's not also its source square
                        // We do it before the rook adjustments, to avoid wrongly emptying rook squares
                        var kingTargetSquare = Utils.KingShortCastleSquare(side);
                        // Since we set the king squares after the switch, we don't need the guard here
                        // if (kingTargetSquare != sourceSquare)
                        //{
                        _pieceBitBoards[newPiece].PopBit(kingTargetSquare);
                        _occupancyBitBoards[side].PopBit(kingTargetSquare);
                        _board[kingTargetSquare] = (int)Piece.None;
                        //}

                        rookSourceSquare = targetSquare;
                    }
                    else
                    {
                        rookSourceSquare = Utils.ShortCastleRookSourceSquare(side);
                    }

                    var rookTargetSquare = Utils.ShortCastleRookTargetSquare(side);
                    var rookIndex = (int)Piece.R + offset;

                    // Popping before setting, because in DFRC they can be the same square
                    _pieceBitBoards[rookIndex].PopBit(rookTargetSquare);

                    // In DFRC the square where the rook ended could be occupied by the king before castling
                    // Since we set the king squares after the switch, we don't need the guard here
                    //if (rookTargetSquare != InitialKingSquares[side])
                    //{
                    _occupancyBitBoards[side].PopBit(rookTargetSquare);
                    _board[rookTargetSquare] = (int)Piece.None;
                    //}

                    _pieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                    _occupancyBitBoards[side].SetBit(rookSourceSquare);
                    _board[rookSourceSquare] = rookIndex;

                    break;
                }
            case SpecialMoveType.LongCastle:
                {
                    int rookSourceSquare;
                    if (Configuration.EngineSettings.IsChess960)
                    {
                        // In DFRC castling moves are encoded as KxR, so the target square in the move isn't really the king target square
                        // However, that target square can only be potentially occupied by the castling rook, so all the ops done over it
                        // have already been undone by the rook ops above, or don't matter (removing the king from the target square, where it isn't anyway)

                        // However, the kings needs to be removed from the real target square
                        // We do it before the rook adjustments, to avoid wrongly emptying rook squares
                        var kingTargetSquare = Utils.KingLongCastleSquare(side);
                        // Since we set the king squares after the switch, we don't need the guard here
                        //if (kingTargetSquare != sourceSquare)
                        //{

                        _pieceBitBoards[newPiece].PopBit(kingTargetSquare);
                        _occupancyBitBoards[side].PopBit(kingTargetSquare);
                        _board[kingTargetSquare] = (int)Piece.None;
                        //}

                        rookSourceSquare = targetSquare;
                    }
                    else
                    {
                        rookSourceSquare = Utils.LongCastleRookSourceSquare(side);
                    }

                    var rookTargetSquare = Utils.LongCastleRookTargetSquare(side);
                    var rookIndex = (int)Piece.R + offset;

                    // Popping before setting, because in DFRC they can be the same square
                    _pieceBitBoards[rookIndex].PopBit(rookTargetSquare);

                    // In DFRC the square where the rook ended could be occupied by the king before castling
                    // Since we set the king squares after the switch, we don't need the guard here
                    //if (rookTargetSquare != InitialKingSquares[side])
                    //{
                    _occupancyBitBoards[side].PopBit(rookTargetSquare);
                    _board[rookTargetSquare] = (int)Piece.None;
                    //}

                    _pieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                    _occupancyBitBoards[side].SetBit(rookSourceSquare);
                    _board[rookSourceSquare] = rookIndex;

                    break;
                }
            case SpecialMoveType.EnPassant:
                {
                    Debug.Assert(move.IsEnPassant());

                    var oppositePawnIndex = (int)Piece.p - offset;
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];

                    Utils.Assert(_occupancyBitBoards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                        $"Expected empty {capturedPawnSquare}");

                    _pieceBitBoards[oppositePawnIndex].SetBit(capturedPawnSquare);
                    _occupancyBitBoards[oppositeSide].SetBit(capturedPawnSquare);
                    _board[capturedPawnSquare] = oppositePawnIndex;

                    break;
                }
        }

        _pieceBitBoards[piece].SetBit(sourceSquare);
        _occupancyBitBoards[side].SetBit(sourceSquare);
        _board[sourceSquare] = piece;

        _occupancyBitBoards[2] = _occupancyBitBoards[1] | _occupancyBitBoards[0];

        --_stackCounter;
        _state = _stateStack[_stackCounter];

        Validate();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullMoveGameState MakeNullMove()
    {
        var gameState = new NullMoveGameState(this);

        _state.UniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ ZobristTable.EnPassantHash((int)_state.EnPassant);

        _side = (Side)Utils.OppositeSide((int)_side);
        _state.EnPassant = BoardSquare.noSquare;

        Validate();

        return gameState;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnMakeNullMove(NullMoveGameState gameState)
    {
        _side = (Side)Utils.OppositeSide((int)_side);
        _state.EnPassant = gameState.EnPassant;
        _state.UniqueIdentifier = gameState.ZobristKey;

        Validate();
    }

    /// <summary>
    /// False if any of the kings has been captured, or if the opponent king is in check.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsValid()
    {
        var offset = Utils.PieceOffset((int)_side);

        var kingBitBoard = _pieceBitBoards[(int)Piece.K + offset];
        var kingSquare = kingBitBoard == default ? -1 : kingBitBoard.GetLS1BIndex();

        var oppositeKingBitBoard = _pieceBitBoards[(int)Piece.k - offset];
        var oppositeKingSquare = oppositeKingBitBoard == default ? -1 : oppositeKingBitBoard.GetLS1BIndex();

        return kingSquare >= 0 && oppositeKingSquare >= 0
            && !IsSquareAttacked(oppositeKingSquare, _side);
    }

    /// <summary>
    /// Lightweight version of <see cref="IsValid"/>
    /// False if the opponent king is in check.
    /// This method is meant to be invoked only after a pseudolegal <see cref="MakeMove(int)"/>.
    /// i.e. it doesn't ensure that both kings are on the board
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool WasProduceByAValidMove()
    {
        Debug.Assert(_pieceBitBoards[(int)Piece.k - Utils.PieceOffset((int)_side)].CountBits() == 1);

        var oppositeKingSquare = _pieceBitBoards[(int)Piece.k - Utils.PieceOffset((int)_side)].GetLS1BIndex();

#if DEBUG
        var isValid = !IsSquareAttacked(oppositeKingSquare, _side);

        if (isValid)
        {
            Validate();
        }

        return isValid;
#else
        return !IsSquareAttacked(oppositeKingSquare, _side);
#endif
    }

    #endregion

    #region Attacks

    /// <summary>
    /// Overload that has rooks and bishops precalculated for the position
    /// </summary>
    /// <param name="rooks">Includes Queen bitboard</param>
    /// <param name="bishops">Includes Queen bitboard</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AllAttackersTo(int square, BitBoard occupancy, BitBoard rooks, BitBoard bishops)
    {
        Debug.Assert(square != (int)BoardSquare.noSquare);

        return (rooks & Attacks.RookAttacks(square, occupancy))
            | (bishops & Attacks.BishopAttacks(square, occupancy))
            | (_pieceBitBoards[(int)Piece.p] & Attacks.PawnAttacks[(int)Side.White][square])
            | (_pieceBitBoards[(int)Piece.P] & Attacks.PawnAttacks[(int)Side.Black][square])
            | (Knights & Attacks.KnightAttacks[square])
            | (Kings & Attacks.KingAttacks[square]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AllAttackersTo(int square)
    {
        Debug.Assert(square != (int)BoardSquare.noSquare);

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var queens = Queens;
        var rooks = queens | Rooks;
        var bishops = queens | Bishops;

        return (rooks & Attacks.RookAttacks(square, occupancy))
            | (bishops & Attacks.BishopAttacks(square, occupancy))
            | (_pieceBitBoards[(int)Piece.p] & Attacks.PawnAttacks[(int)Side.White][square])
            | (_pieceBitBoards[(int)Piece.P] & Attacks.PawnAttacks[(int)Side.Black][square])
            | (Knights & Attacks.KnightAttacks[square])
            | (Kings & Attacks.KingAttacks[square]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AllSideAttackersTo(int square, int side)
    {
        Debug.Assert(square != (int)BoardSquare.noSquare);
        Debug.Assert(side != (int)Side.Both);

        var offset = Utils.PieceOffset(side);

        var occupancy = _occupancyBitBoards[(int)Side.Both];

        var queens = _pieceBitBoards[(int)Piece.q - offset];
        var rooks = queens | _pieceBitBoards[(int)Piece.r - offset];
        var bishops = queens | _pieceBitBoards[(int)Piece.b - offset];

        return (rooks & Attacks.RookAttacks(square, occupancy))
            | (bishops & Attacks.BishopAttacks(square, occupancy))
            | (_pieceBitBoards[(int)Piece.p - offset] & Attacks.PawnAttacks[side][square])
            | (_pieceBitBoards[(int)Piece.n - offset] & Attacks.KnightAttacks[square]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttacked(int squareIndex, Side sideToMove)
    {
        Debug.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);
        var bothSidesOccupancy = _occupancyBitBoards[(int)Side.Both];

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
    public bool AreSquaresAttacked(ulong squaresBitboard, Side attackingSide, ref EvaluationContext evaluationContext)
    {
        var attacks = evaluationContext.AttacksBySide[(int)attackingSide];

        if (attacks != 0)
        {
            return (attacks & squaresBitboard) != 0;
        }

        // Fallback: no threats
        while (squaresBitboard != 0)
        {
            squaresBitboard = squaresBitboard.WithoutLS1B(out var square);

            if (IsSquareAttacked(square, attackingSide))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInCheck()
    {
        var oppositeSideInt = Utils.OppositeSide((int)_side);
        var oppositeSideOffset = Utils.PieceOffset(oppositeSideInt);

        var kingSquare = _pieceBitBoards[(int)Piece.k - oppositeSideOffset].GetLS1BIndex();

        var bothSidesOccupancy = _occupancyBitBoards[(int)Side.Both];

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

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & _pieceBitBoards[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByKnights(int squareIndex, int offset)
    {
        return (Attacks.KnightAttacks[squareIndex] & _pieceBitBoards[(int)Piece.N + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByKing(int squareIndex, int offset)
    {
        return (Attacks.KingAttacks[squareIndex] & _pieceBitBoards[(int)Piece.K + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard bothSidesOccupancy, out BitBoard bishopAttacks)
    {
        bishopAttacks = Attacks.BishopAttacks(squareIndex, bothSidesOccupancy);
        return (bishopAttacks & _pieceBitBoards[(int)Piece.B + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard bothSidesOccupancy, out BitBoard rookAttacks)
    {
        rookAttacks = Attacks.RookAttacks(squareIndex, bothSidesOccupancy);
        return (rookAttacks & _pieceBitBoards[(int)Piece.R + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks)
    {
        var queenAttacks = Attacks.QueenAttacks(rookAttacks, bishopAttacks);
        return (queenAttacks & _pieceBitBoards[(int)Piece.Q + offset]) != default;
    }

    #endregion

    public int CountPieces() => _pieceBitBoards.Sum(b => b.CountBits());

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
                if (_pieceBitBoards[pieceBoardIndex].GetBit(square))
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
        sb.Append(_side == Side.White ? 'w' : 'b');

        sb.Append(' ');
        var lengthBeforeCastlingRights = sb.Length;

        if (!Configuration.EngineSettings.IsChess960)
        {
            if ((_state.Castle & (int)CastlingRights.WK) != default)
            {
                sb.Append('K');
            }
            if ((_state.Castle & (int)CastlingRights.WQ) != default)
            {
                sb.Append('Q');
            }
            if ((_state.Castle & (int)CastlingRights.BK) != default)
            {
                sb.Append('k');
            }
            if ((_state.Castle & (int)CastlingRights.BQ) != default)
            {
                sb.Append('q');
            }
        }
        else
        {
            // Shredder-FEN style (always showing columns), no support for X-FEN style yet (showing KQkq when not-ambiguous)
            if ((_state.Castle & (int)CastlingRights.WK) != default)
            {
                char file = (char)('A' + Constants.File[WhiteShortCastle.TargetSquare()]);
                sb.Append(file);
            }
            if ((_state.Castle & (int)CastlingRights.WQ) != default)
            {
                char file = (char)('A' + Constants.File[WhiteLongCastle.TargetSquare()]);
                sb.Append(file);
            }
            if ((_state.Castle & (int)CastlingRights.BK) != default)
            {
                char file = (char)('a' + Constants.File[BlackShortCastle.TargetSquare()]);
                sb.Append(file);
            }
            if ((_state.Castle & (int)CastlingRights.BQ) != default)
            {
                char file = (char)('a' + Constants.File[BlackLongCastle.TargetSquare()]);
                sb.Append(file);
            }
        }

        if (sb.Length == lengthBeforeCastlingRights)
        {
            sb.Append('-');
        }

        sb.Append(' ');

        sb.Append(_state.EnPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)_state.EnPassant]);

        sb.Append(' ').Append(halfMovesWithoutCaptureOrPawnMove).Append(' ').Append(fullMoveClock);

        return sb.ToString();
    }

#pragma warning disable S106, S2228 // Standard outputs should not be used directly to log anything

    /// <summary>
    /// Combines <see cref="_pieceBitBoards"/>, <see cref="_side"/>, <see cref="_state.Castle"/> and <see cref="_state.EnPassant"/>
    /// into a human-friendly representation
    /// </summary>
    public void Print(int halfMovesWithoutCaptureOrPawnMove = -1)
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

                for (int bbIndex = 0; bbIndex < _pieceBitBoards.Length; ++bbIndex)
                {
                    if (_pieceBitBoards[bbIndex].GetBit(squareIndex))
                    {
                        piece = bbIndex;
                        break;
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
        Console.WriteLine($"    Side:\t{_side}");
        Console.WriteLine($"    Enpassant:\t{(_state.EnPassant == BoardSquare.noSquare ? "no" : Constants.Coordinates[(int)_state.EnPassant])}");

        if (!Configuration.EngineSettings.IsChess960)
        {
            Console.WriteLine($"    Castling:\t" +
                $"{((_state.Castle & (int)CastlingRights.WK) != default ? 'K' : '-')}" +
                $"{((_state.Castle & (int)CastlingRights.WQ) != default ? 'Q' : '-')} | " +
                $"{((_state.Castle & (int)CastlingRights.BK) != default ? 'k' : '-')}" +
                $"{((_state.Castle & (int)CastlingRights.BQ) != default ? 'q' : '-')}");
        }
        else
        {
            char whiteKingSide = '-', whiteQueenside = '-', blackKingside = '-', blackQueenside = '-';

            if ((_state.Castle & (int)CastlingRights.WK) != default)
            {
                whiteKingSide = (char)('A' + Constants.File[WhiteShortCastle.TargetSquare()]);
            }
            if ((_state.Castle & (int)CastlingRights.WQ) != default)
            {
                whiteQueenside = (char)('A' + Constants.File[WhiteLongCastle.TargetSquare()]);
            }
            if ((_state.Castle & (int)CastlingRights.BK) != default)
            {
                blackKingside = (char)('a' + Constants.File[BlackShortCastle.TargetSquare()]);
            }
            if ((_state.Castle & (int)CastlingRights.BQ) != default)
            {
                blackQueenside = (char)('a' + Constants.File[BlackLongCastle.TargetSquare()]);
            }

            Console.WriteLine($"    Castling:\t" +
                whiteKingSide +
                $"{whiteQueenside} | " +
                blackKingside +
                $"{blackQueenside}");
        }

        if (halfMovesWithoutCaptureOrPawnMove != -1)
        {
            Console.WriteLine($"    Half-moves:\t{halfMovesWithoutCaptureOrPawnMove}");
        }

        Console.WriteLine($"    FEN:\t{FEN()}");
#pragma warning restore RCS1214 // Unnecessary interpolated string.

        Console.WriteLine(separator);
    }

    [Conditional("DEBUG")]
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

#pragma warning restore S106, S2228 // Standard outputs should not be used directly to log anything

    /// <summary>
    /// Inspired by rawr's validation method
    /// </summary>
    [Conditional("DEBUG")]
    public void Validate()
    {
        const string failureMessage = "Position validation failed";

        Debug.Assert(Side != Side.Both, failureMessage, "Side == Side.Both");

        var whitePawns = _pieceBitBoards[(int)Piece.P];
        var blackPawns = _pieceBitBoards[(int)Piece.p];
        var whiteKnights = _pieceBitBoards[(int)Piece.N];
        var whiteBishops = _pieceBitBoards[(int)Piece.B];
        var whiteRooks = _pieceBitBoards[(int)Piece.R];
        var whiteQueens = _pieceBitBoards[(int)Piece.Q];
        var whiteKings = _pieceBitBoards[(int)Piece.K];
        var blackKnights = _pieceBitBoards[(int)Piece.n];
        var blackBishops = _pieceBitBoards[(int)Piece.b];
        var blackRooks = _pieceBitBoards[(int)Piece.r];
        var blackQueens = _pieceBitBoards[(int)Piece.q];
        var blackKings = _pieceBitBoards[(int)Piece.k];

        // No pawns in 1 and 8 ranks
        Debug.Assert((whitePawns & Constants.PawnSquares) == whitePawns, failureMessage, "White pawn(s) un 1-8");
        Debug.Assert((blackPawns & Constants.PawnSquares) == blackPawns, failureMessage, "Black pawn(s) un 1-8");

        // No side occupancy overlap
        Debug.Assert((_occupancyBitBoards[(int)Side.White] & _occupancyBitBoards[(int)Side.Black]) == 0, failureMessage, "White and Black overlap");

        // Side.Both occupancy overlap
        Debug.Assert((_occupancyBitBoards[(int)Side.White] | _occupancyBitBoards[(int)Side.Black]) == _occupancyBitBoards[(int)Side.Both], failureMessage, "Occupancy not correct");

        // No piece overlap
        const string pieceOverlapMessage = "Piece overlap";
        // Pawns
        Debug.Assert((whitePawns & whiteKnights) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & whiteBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & whiteRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & whiteQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & whiteKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((whitePawns & blackPawns) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & blackKnights) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & blackBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whitePawns & blackKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((blackPawns & blackKnights) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackPawns & blackBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackPawns & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackPawns & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackPawns & blackKings) == 0, failureMessage, pieceOverlapMessage);

        // Knights
        Debug.Assert((whiteKnights & whiteBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKnights & whiteRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKnights & whiteQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKnights & whiteKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((whiteKnights & blackPawns) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKnights & blackKnights) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKnights & blackBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKnights & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKnights & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKnights & blackKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((blackKnights & blackBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackKnights & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackKnights & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackKnights & blackKings) == 0, failureMessage, pieceOverlapMessage);

        // Bishops
        Debug.Assert((whiteBishops & whiteRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteBishops & whiteQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteBishops & whiteKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((whiteBishops & blackPawns) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteBishops & blackKnights) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteBishops & blackBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteBishops & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteBishops & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteBishops & blackKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((blackBishops & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackBishops & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackBishops & blackKings) == 0, failureMessage, pieceOverlapMessage);

        // Rooks
        Debug.Assert((whiteRooks & whiteQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteRooks & whiteKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((whiteRooks & blackPawns) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteRooks & blackKnights) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteRooks & blackBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteRooks & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteRooks & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteRooks & blackKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((blackRooks & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((blackRooks & blackKings) == 0, failureMessage, pieceOverlapMessage);

        // Queens
        Debug.Assert((whiteQueens & whiteKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((whiteQueens & blackPawns) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteQueens & blackKnights) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteQueens & blackBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteQueens & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteQueens & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteQueens & blackKings) == 0, failureMessage, pieceOverlapMessage);

        Debug.Assert((blackQueens & blackKings) == 0, failureMessage, pieceOverlapMessage);

        // Kings
        Debug.Assert((whiteKings & blackPawns) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKings & blackKnights) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKings & blackBishops) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKings & blackRooks) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKings & blackQueens) == 0, failureMessage, pieceOverlapMessage);
        Debug.Assert((whiteKings & blackKings) == 0, failureMessage, pieceOverlapMessage);

        // 1 king per side
        Debug.Assert(whiteKings.CountBits() == 1, failureMessage, $"More than one white king, or none: {whiteKings}");
        Debug.Assert(blackKings.CountBits() == 1, failureMessage, $"More than one black king, or none: {blackKings}");

#if DEBUG
        if (_state.Castle != 0)
        {
            var whiteKingSourceSquare = _initialKingSquares[(int)Side.White];

            // Castling rights and king/rook positions
            if ((_state.Castle & (int)CastlingRights.WK) != 0)
            {
                Debug.Assert(whiteKings.GetBit(whiteKingSourceSquare), failureMessage, "No white king on e1 when short castling rights");

                Debug.Assert(_initialKingsideRookSquares[(int)Side.White] != CastlingData.DefaultValues, failureMessage, "White initial kingside rook not set");
                Debug.Assert(whiteRooks.GetBit(_initialKingsideRookSquares[(int)Side.White]), failureMessage, $"No white rook on {(BoardSquare)_initialKingsideRookSquares[(int)Side.White]} when short castling rights");
            }

            if ((_state.Castle & (int)CastlingRights.WQ) != 0)
            {
                Debug.Assert(whiteKings.GetBit(whiteKingSourceSquare), failureMessage, "No white king on e1 when long castling rights");

                Debug.Assert(whiteRooks.GetBit(_initialQueensideRookSquares[(int)Side.White]), failureMessage, $"No white rook on {(BoardSquare)_initialQueensideRookSquares[(int)Side.White]} when long castling rights");
                Debug.Assert(_initialQueensideRookSquares[(int)Side.White] != CastlingData.DefaultValues, failureMessage, "White initial queenside rook not set");
            }

            var blackKingSourceSquare = _initialKingSquares[(int)Side.Black];

            if ((_state.Castle & (int)CastlingRights.BK) != 0)
            {
                Debug.Assert(blackKings.GetBit(blackKingSourceSquare), failureMessage, "No black king on e8 when short castling rights");

                Debug.Assert(_initialKingsideRookSquares[(int)Side.Black] != CastlingData.DefaultValues, failureMessage, "Black initial kingside rook not set");
                Debug.Assert(blackRooks.GetBit(_initialKingsideRookSquares[(int)Side.Black]), failureMessage, $"No black rook on {(BoardSquare)_initialKingsideRookSquares[(int)Side.Black]} when short castling rights");
            }

            if ((_state.Castle & (int)CastlingRights.BQ) != 0)
            {
                Debug.Assert(blackKings.GetBit(blackKingSourceSquare), failureMessage, "No black king on e8 when long castling rights");

                Debug.Assert(blackRooks.GetBit(_initialQueensideRookSquares[(int)Side.Black]), failureMessage, $"No black rook on {(BoardSquare)_initialQueensideRookSquares[(int)Side.Black]} when long castling rights");
                Debug.Assert(_initialQueensideRookSquares[(int)Side.Black] != CastlingData.DefaultValues, failureMessage, "Black initial queenside rook not set");
            }
        }
#endif

        // En-passant and pawn to be captured position
        if (_state.EnPassant != BoardSquare.noSquare)
        {
            Debug.Assert(!_occupancyBitBoards[(int)Side.Both].GetBit((int)_state.EnPassant), failureMessage, $"Non-empty en passant square {_state.EnPassant}");

            var rank = Constants.Rank[(int)_state.EnPassant];
            Debug.Assert(rank == 2 || rank == 5, failureMessage, $"Wrong en-passant rank for {_state.EnPassant}");

            var pawnToCaptureSquare = Constants.EnPassantCaptureSquares[(int)_state.EnPassant];

            if (Side == Side.White)
            {
                Debug.Assert(blackPawns.GetBit(pawnToCaptureSquare), failureMessage, $"No black pawn on en-passant capture square for {_state.EnPassant}");
            }
            else
            {
                Debug.Assert(whitePawns.GetBit(pawnToCaptureSquare), failureMessage, $"No white pawn on en-passant capture square for {_state.EnPassant}");
            }
        }

        // Can't capture opponent's king
        Debug.Assert(!IsSquareAttacked(_pieceBitBoards[(int)Piece.k - Utils.PieceOffset((int)_side)].GetLS1BIndex(), Side), failureMessage, "Can't capture opponent's king");

        Debug.Assert(Math.Min(MaxPhase, PhaseFromScratch()) == Phase(), failureMessage, $"Wrong incremental phase: {Phase()} vs from scratch {Math.Min(MaxPhase, PhaseFromScratch())}");
    }

    [Conditional("DEBUG")]
    private void AssertAttackPopulation(ref EvaluationContext evaluationContext)
    {
        var attacks = evaluationContext.Attacks;

        Debug.Assert(_pieceBitBoards[(int)Piece.P] == 0 || attacks[(int)Piece.P] != 0);
        Debug.Assert(_pieceBitBoards[(int)Piece.N] == 0 || attacks[(int)Piece.N] != 0);
        Debug.Assert(_pieceBitBoards[(int)Piece.B] == 0 || attacks[(int)Piece.B] != 0);
        Debug.Assert(_pieceBitBoards[(int)Piece.R] == 0 || attacks[(int)Piece.R] != 0);
        Debug.Assert(_pieceBitBoards[(int)Piece.Q] == 0 || attacks[(int)Piece.Q] != 0);
        Debug.Assert(attacks[(int)Piece.K] != 0);
        Debug.Assert(evaluationContext.AttacksBySide[(int)Side.White] != 0);

        Debug.Assert(_pieceBitBoards[(int)Piece.p] == 0 || attacks[(int)Piece.p] != 0);
        Debug.Assert(_pieceBitBoards[(int)Piece.n] == 0 || attacks[(int)Piece.n] != 0);
        Debug.Assert(_pieceBitBoards[(int)Piece.b] == 0 || attacks[(int)Piece.b] != 0);
        Debug.Assert(_pieceBitBoards[(int)Piece.r] == 0 || attacks[(int)Piece.r] != 0);
        Debug.Assert(_pieceBitBoards[(int)Piece.q] == 0 || attacks[(int)Piece.q] != 0);
        Debug.Assert(attacks[(int)Piece.k] != 0);
        Debug.Assert(evaluationContext.AttacksBySide[(int)Side.Black] != 0);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ArrayPool<BitBoard>.Shared.Return(_pieceBitBoards);
                ArrayPool<BitBoard>.Shared.Return(_occupancyBitBoards);

                ArrayPool<ulong>.Shared.Return(KingsideCastlingFreeSquares);
                ArrayPool<ulong>.Shared.Return(QueensideCastlingFreeSquares);
                ArrayPool<ulong>.Shared.Return(KingsideCastlingNonAttackedSquares);
                ArrayPool<ulong>.Shared.Return(QueensideCastlingNonAttackedSquares);

                ArrayPool<byte>.Shared.Return(_castlingRightsUpdateConstants);

#if DEBUG
                ArrayPool<int>.Shared.Return(_initialKingSquares);
                ArrayPool<int>.Shared.Return(_initialKingsideRookSquares);
                ArrayPool<int>.Shared.Return(_initialQueensideRookSquares);
#endif

#pragma warning disable S3254 // Default parameter values should not be passed as arguments
                ArrayPool<int>.Shared.Return(_board);
#pragma warning restore S3254 // Default parameter values should not be passed as arguments
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
