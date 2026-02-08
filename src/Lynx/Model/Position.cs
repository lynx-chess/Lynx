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

#pragma warning disable IDE1006 // Naming Styles
    internal int IncrementalEvalAccumulator;
    internal int IncrementalPhaseAccumulator;
    internal bool IsIncrementalEval;
#pragma warning restore IDE1006 // Naming Styles

    private ulong _uniqueIdentifier;
    private ulong _kingPawnUniqueIdentifier;
    private readonly ulong[] _nonPawnHash;
    private ulong _minorHash;
    private ulong _majorHash;

    private ulong[] _pieceBitboards;
    private ulong[] _occupancyBitboards;
    private int[] _board;

    private byte _castle;

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

    private BoardSquare _enPassant;
    private Side _side;

#pragma warning disable RCS1085 // Use auto-implemented property

    public ulong UniqueIdentifier => _uniqueIdentifier;
    public ulong KingPawnUniqueIdentifier => _kingPawnUniqueIdentifier;
    public ulong[] NonPawnHash => _nonPawnHash;
    public ulong MinorHash => _minorHash;
    public ulong MajorHash => _majorHash;

    public Bitboard[] PieceBitboards => _pieceBitboards;
    public Bitboard[] OccupancyBitboards => _occupancyBitboards;
    public int[] Board => _board;
    public Side Side => _side;
    public BoardSquare EnPassant => _enPassant;

    /// <summary>
    /// See <see cref="<CastlingRights"/>
    /// </summary>
    public byte Castle { get => _castle; }

#pragma warning restore RCS1085 // Use auto-implemented property

    public Bitboard Queens
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitboards[(int)Piece.Q] | _pieceBitboards[(int)Piece.q];
    }

    public Bitboard Rooks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitboards[(int)Piece.R] | _pieceBitboards[(int)Piece.r];
    }

    public Bitboard Bishops
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitboards[(int)Piece.B] | _pieceBitboards[(int)Piece.b];
    }

    public Bitboard Knights
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitboards[(int)Piece.N] | _pieceBitboards[(int)Piece.n];
    }

    public Bitboard Kings
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pieceBitboards[(int)Piece.K] | _pieceBitboards[(int)Piece.k];
    }

    public int WhiteKingSquare => _pieceBitboards[(int)Piece.K].GetLS1BIndex();
    public int BlackKingSquare => _pieceBitboards[(int)Piece.k].GetLS1BIndex();

    public int InitialKingSquare(int side) =>
        side == (int)Side.White
            ? WhiteShortCastle.SourceSquare()
            : BlackShortCastle.SourceSquare();

    private Position()
    {
        // Allocate all required backing arrays (independent copy, avoids sharing state)
        _pieceBitboards = ArrayPool<Bitboard>.Shared.Rent(12);
        _occupancyBitboards = ArrayPool<Bitboard>.Shared.Rent(3);
        _board = ArrayPool<int>.Shared.Rent(64);
        _nonPawnHash = ArrayPool<ulong>.Shared.Rent(2);
        _castlingRightsUpdateConstants = ArrayPool<byte>.Shared.Rent(64);

        // Always allocate length 2 so ResetTo can index safely regardless of castling rights
        KingsideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);
        KingsideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);
        QueensideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);
        QueensideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);

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

    public void PopulateFrom(ParseFENResult parsedFEN)
    {
        _pieceBitboards = parsedFEN.PieceBitboards;
        _occupancyBitboards = parsedFEN.OccupancyBitboards;
        _board = parsedFEN.Board;

        _side = parsedFEN.Side;
        _castle = parsedFEN.Castle;
        _enPassant = parsedFEN.EnPassant;

#pragma warning disable S3366 // "this" should not be exposed from constructors
        _nonPawnHash[(int)Side.White] = ZobristTable.NonPawnSideHash(this, (int)Side.White);
        _nonPawnHash[(int)Side.Black] = ZobristTable.NonPawnSideHash(this, (int)Side.Black);

        _minorHash = ZobristTable.MinorHash(this);
        _majorHash = ZobristTable.MajorHash(this);
        _kingPawnUniqueIdentifier = ZobristTable.KingPawnHash(this);

        _uniqueIdentifier = ZobristTable.PositionHash(this, _kingPawnUniqueIdentifier, _nonPawnHash[(int)Side.White], _nonPawnHash[(int)Side.Black]);

        Debug.Assert(_uniqueIdentifier == ZobristTable.PositionHash(this));
#pragma warning restore S3366 // "this" should not be exposed from constructors

        IsIncrementalEval = false;

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

        KingsideCastlingNonAttackedSquares[(int)Side.White] = BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingSquare, Constants.WhiteKingShortCastleSquare);
        KingsideCastlingNonAttackedSquares[(int)Side.Black] = BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingSquare, Constants.BlackKingShortCastleSquare);

        QueensideCastlingNonAttackedSquares[(int)Side.White] = BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingSquare, Constants.WhiteKingLongCastleSquare);
        QueensideCastlingNonAttackedSquares[(int)Side.Black] = BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingSquare, Constants.BlackKingLongCastleSquare);

        // This could be simplified/hardcoded for standard chess, see FreeAndNonAttackedSquares
        var whiteKingsideFreeMask = KingsideCastlingNonAttackedSquares[(int)Side.White]
            | BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingsideRook, Constants.WhiteRookShortCastleSquare);
        whiteKingsideFreeMask.PopBit(whiteKingSquare);
        whiteKingsideFreeMask.PopBit(whiteKingsideRook);

        var blackKingsideFreeMask = KingsideCastlingNonAttackedSquares[(int)Side.Black]
            | BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingsideRook, Constants.BlackRookShortCastleSquare);
        blackKingsideFreeMask.PopBit(blackKingSquare);
        blackKingsideFreeMask.PopBit(blackKingsideRook);

        KingsideCastlingFreeSquares[(int)Side.White] = whiteKingsideFreeMask;
        KingsideCastlingFreeSquares[(int)Side.Black] = blackKingsideFreeMask;

        // This could be simplified/hardcoded for standard chess, see FreeAndNonAttackedSquares
        var whiteQueensideFreeMask = QueensideCastlingNonAttackedSquares[(int)Side.White]
            | BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteQueensideRook, Constants.WhiteRookLongCastleSquare);
        whiteQueensideFreeMask.PopBit(whiteKingSquare);
        whiteQueensideFreeMask.PopBit(whiteQueensideRook);

        var blackQueensideFreeMask = QueensideCastlingNonAttackedSquares[(int)Side.Black]
            | BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackQueensideRook, Constants.BlackRookLongCastleSquare);
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
        _uniqueIdentifier = position._uniqueIdentifier;
        _kingPawnUniqueIdentifier = position._kingPawnUniqueIdentifier;
        _minorHash = position._minorHash;
        _majorHash = position._majorHash;

        _nonPawnHash[(int)Side.White] = position._nonPawnHash[(int)Side.White];
        _nonPawnHash[(int)Side.Black] = position._nonPawnHash[(int)Side.Black];

        Array.Copy(position._pieceBitboards, _pieceBitboards, 12);
        Array.Copy(position._occupancyBitboards, _occupancyBitboards, 3);
        Array.Copy(position._board, _board, 64);

        _side = position._side;
        _castle = position._castle;
        _enPassant = position._enPassant;

        IsIncrementalEval = position.IsIncrementalEval;
        IncrementalEvalAccumulator = position.IncrementalEvalAccumulator;
        IncrementalPhaseAccumulator = position.IncrementalPhaseAccumulator;

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
    public GameState MakeMove(Move move)
    {
        Debug.Assert(ZobristTable.PositionHash(this) == _uniqueIdentifier);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.White) == _nonPawnHash[(int)Side.White]);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.Black) == _nonPawnHash[(int)Side.Black]);
        Debug.Assert(ZobristTable.MinorHash(this) == _minorHash);
        Debug.Assert(ZobristTable.MajorHash(this) == _majorHash);

        var gameState = new GameState(this);

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

        _pieceBitboards[piece].PopBit(sourceSquare);
        _occupancyBitboards[oldSide].PopBit(sourceSquare);
        _board[sourceSquare] = (int)Piece.None;

        _pieceBitboards[newPiece].SetBit(targetSquare);
        _occupancyBitboards[oldSide].SetBit(targetSquare);
        _board[targetSquare] = newPiece;

        var sourcePieceHash = ZobristTable.PieceHash(sourceSquare, piece);
        var targetPieceHash = ZobristTable.PieceHash(targetSquare, newPiece);
        var fullPieceMovementHash = sourcePieceHash ^ targetPieceHash;

        _uniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ fullPieceMovementHash
            ^ ZobristTable.EnPassantHash((int)_enPassant)            // We clear the existing enpassant square, if any
            ^ ZobristTable.CastleHash(_castle);                      // We clear the existing castle rights

        if (piece == (int)Piece.P || piece == (int)Piece.p)
        {
            _kingPawnUniqueIdentifier ^= sourcePieceHash;       // We remove pawn from start square

            if (promotedPiece == default)
            {
                _kingPawnUniqueIdentifier ^= targetPieceHash;   // We add pawn again to end square
            }
            else
            {
                // In case of promotion, the promoted piece won't be a pawn or a king, so no need to update the KingPawn hash with it, just to remove the pawn (done right above)
                // We do need to update the NonPawn hash
                _nonPawnHash[oldSide] ^= targetPieceHash;       // We add piece piece to the end square

                if (Utils.IsMinorPiece(newPiece))
                {
                    _minorHash ^= targetPieceHash;
                }
                else if (Utils.IsMajorPiece(newPiece))
                {
                    _majorHash ^= targetPieceHash;
                }
            }
        }
        else
        {
            _nonPawnHash[oldSide] ^= fullPieceMovementHash;

            if (piece == (int)Piece.K || piece == (int)Piece.k)
            {
                // King (and castling) moves require calculating king buckets twice and recalculating all related parameters, so skipping incremental eval for those cases for now
                // No need to check for move.IsCastle(), see CastlingMovesAreKingMoves test
                IsIncrementalEval = false;

                _kingPawnUniqueIdentifier ^= fullPieceMovementHash;
            }
            else if (Utils.IsMinorPiece(piece))
            {
                _minorHash ^= fullPieceMovementHash;
            }
            else if (Utils.IsMajorPiece(piece))
            {
                _majorHash ^= fullPieceMovementHash;
            }
        }

        _enPassant = BoardSquare.noSquare;

        // _incrementalEvalAccumulator updates
        if (IsIncrementalEval)
        {
            var whiteKing = _pieceBitboards[(int)Piece.K].GetLS1BIndex();
            var blackKing = _pieceBitboards[(int)Piece.k].GetLS1BIndex();
            var whiteBucket = PSQTBucketLayout[whiteKing];
            var blackBucket = PSQTBucketLayout[blackKing ^ 56];

            int sameSideBucket = whiteBucket;
            int oppositeSideBucket = blackBucket;
            if (_side == Side.Black)
            {
                (sameSideBucket, oppositeSideBucket) = (oppositeSideBucket, sameSideBucket);
            }

            IncrementalEvalAccumulator -= PSQT(sameSideBucket, oppositeSideBucket, piece, sourceSquare);
            IncrementalEvalAccumulator += PSQT(sameSideBucket, oppositeSideBucket, newPiece, targetSquare);

            IncrementalPhaseAccumulator += extraPhaseIfIncremental;

            // No need to check for castling if it's incremental eval
            switch (move.SpecialMoveFlag())
            {
                case SpecialMoveType.None:
                    {
                        var capturedPiece = move.CapturedPiece();
                        if (capturedPiece != (int)Piece.None)
                        {
                            var capturedSquare = targetSquare;

                            _pieceBitboards[capturedPiece].PopBit(capturedSquare);
                            _occupancyBitboards[oppositeSide].PopBit(capturedSquare);

                            var capturedPieceHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                            _uniqueIdentifier ^= capturedPieceHash;

                            // Kings can't be captured
                            if (capturedPiece == (int)Piece.P || capturedPiece == (int)Piece.p)
                            {
                                _kingPawnUniqueIdentifier ^= capturedPieceHash;
                            }
                            else
                            {
                                _nonPawnHash[oppositeSide] ^= capturedPieceHash;

                                if (Utils.IsMinorPiece(capturedPiece))
                                {
                                    _minorHash ^= capturedPieceHash;
                                }
                                else if (Utils.IsMajorPiece(capturedPiece))
                                {
                                    _majorHash ^= capturedPieceHash;
                                }
                            }

                            IncrementalEvalAccumulator -= PSQT(oppositeSideBucket, sameSideBucket, capturedPiece, capturedSquare);

                            IncrementalPhaseAccumulator -= GamePhaseByPiece[capturedPiece];
                        }

                        break;
                    }
                case SpecialMoveType.DoublePawnPush:
                    {
                        var pawnPush = +8 - (oldSide * 16);
                        var enPassantSquare = sourceSquare + pawnPush;
                        Utils.Assert(Constants.EnPassantCaptureSquares.Length > enPassantSquare && Constants.EnPassantCaptureSquares[enPassantSquare] != 0, $"Unexpected en passant square : {(BoardSquare)enPassantSquare}");

                        _enPassant = (BoardSquare)enPassantSquare;
                        _uniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                        break;
                    }
                case SpecialMoveType.EnPassant:
                    {
                        var oppositePawnIndex = (int)Piece.p - offset;

                        var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                        var capturedPiece = oppositePawnIndex;
                        Utils.Assert(_pieceBitboards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");

                        _pieceBitboards[capturedPiece].PopBit(capturedSquare);
                        _occupancyBitboards[oppositeSide].PopBit(capturedSquare);
                        _board[capturedSquare] = (int)Piece.None;

                        var capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                        _uniqueIdentifier ^= capturedPawnHash;
                        _kingPawnUniqueIdentifier ^= capturedPawnHash;

                        IncrementalEvalAccumulator -= PSQT(oppositeSideBucket, sameSideBucket, capturedPiece, capturedSquare);

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

                            _pieceBitboards[capturedPiece].PopBit(capturedSquare);
                            _occupancyBitboards[oppositeSide].PopBit(capturedSquare);

                            ulong capturedPieceHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                            _uniqueIdentifier ^= capturedPieceHash;

                            // Kings can't be captured
                            if (capturedPiece == (int)Piece.P || capturedPiece == (int)Piece.p)
                            {
                                _kingPawnUniqueIdentifier ^= capturedPieceHash;
                            }
                            else
                            {
                                _nonPawnHash[oppositeSide] ^= capturedPieceHash;

                                if (Utils.IsMinorPiece(capturedPiece))
                                {
                                    _minorHash ^= capturedPieceHash;
                                }
                                else if (Utils.IsMajorPiece(capturedPiece))
                                {
                                    _majorHash ^= capturedPieceHash;
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

                        _enPassant = (BoardSquare)enPassantSquare;
                        _uniqueIdentifier ^= ZobristTable.EnPassantHash(enPassantSquare);

                        break;
                    }
                case SpecialMoveType.ShortCastle:
                    {
                        var rookSourceSquare = Configuration.EngineSettings.IsChess960
                            ? targetSquare
                            : Utils.ShortCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitboards[rookIndex].PopBit(rookSourceSquare);

                        var kingTargetSquare = Utils.KingShortCastleSquare(oldSide);

                        if (Configuration.EngineSettings.IsChess960)
                        {
                            // In DFRC castling moves are encoded as KxR, so the target square in the move isn't really the king target square
                            // We need to revert the incorrect changes + apply the right ones
                            // This could be avoided by adding a branch above for all moves and set the right target square for DFRC
                            // But that hurts performance, see https://github.com/lynx-chess/Lynx/pull/2043
                            _pieceBitboards[newPiece].PopBit(targetSquare);
                            _occupancyBitboards[oldSide].PopBit(targetSquare);
                            _board[targetSquare] = (int)Piece.None;
                            var hashToRevert = ZobristTable.PieceHash(targetSquare, newPiece);

                            _pieceBitboards[newPiece].SetBit(kingTargetSquare);
                            _occupancyBitboards[oldSide].SetBit(kingTargetSquare);
                            _board[kingTargetSquare] = newPiece;
                            var hashToApply = ZobristTable.PieceHash(kingTargetSquare, newPiece);

                            var hashFix = hashToRevert ^ hashToApply;

                            _uniqueIdentifier ^= hashFix;
                            _nonPawnHash[oldSide] ^= hashFix;
                            _kingPawnUniqueIdentifier ^= hashFix;
                        }

                        // In DFRC the square where the rook was could be occupied by the king after castling
                        // This guard could maybe be removed if we ever move the Sets after the switch, same as we did in Unmake
                        if (rookSourceSquare != kingTargetSquare)
                        {
                            _occupancyBitboards[oldSide].PopBit(rookSourceSquare);
                            _board[rookSourceSquare] = (int)Piece.None;
                        }

                        _pieceBitboards[rookIndex].SetBit(rookTargetSquare);
                        _occupancyBitboards[oldSide].SetBit(rookTargetSquare);
                        _board[rookTargetSquare] = rookIndex;

                        var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        _uniqueIdentifier ^= hashChange;
                        _nonPawnHash[oldSide] ^= hashChange;
                        _majorHash ^= hashChange;

                        break;
                    }
                case SpecialMoveType.LongCastle:
                    {
                        var rookSourceSquare = Configuration.EngineSettings.IsChess960
                            ? targetSquare
                            : Utils.LongCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitboards[rookIndex].PopBit(rookSourceSquare);

                        var kingTargetSquare = Utils.KingLongCastleSquare(oldSide);

                        if (Configuration.EngineSettings.IsChess960)
                        {
                            // In DFRC castling moves are encoded as KxR, so the target square in the move isn't really the king target square
                            // We need to revert the incorrect changes + apply the right ones
                            // This could be avoided by adding a branch above for all moves and set the right target square for DFRC
                            // But that hurts performance, see https://github.com/lynx-chess/Lynx/pull/2043
                            _pieceBitboards[newPiece].PopBit(targetSquare);
                            _occupancyBitboards[oldSide].PopBit(targetSquare);
                            _board[targetSquare] = (int)Piece.None;
                            var hashToRevert = ZobristTable.PieceHash(targetSquare, newPiece);

                            _pieceBitboards[newPiece].SetBit(kingTargetSquare);
                            _occupancyBitboards[oldSide].SetBit(kingTargetSquare);
                            _board[kingTargetSquare] = newPiece;
                            var hashToApply = ZobristTable.PieceHash(kingTargetSquare, newPiece);

                            var hashFix = hashToRevert ^ hashToApply;

                            _uniqueIdentifier ^= hashFix;
                            _nonPawnHash[oldSide] ^= hashFix;
                            _kingPawnUniqueIdentifier ^= hashFix;
                        }

                        // In DFRC the square where the rook was could be occupied by the king after castling
                        // This guard could maybe be removed if we ever move the Sets after the switch, same as we did in Unmake
                        if (rookSourceSquare != kingTargetSquare)
                        {
                            _occupancyBitboards[oldSide].PopBit(rookSourceSquare);
                            _board[rookSourceSquare] = (int)Piece.None;
                        }

                        _pieceBitboards[rookIndex].SetBit(rookTargetSquare);
                        _occupancyBitboards[oldSide].SetBit(rookTargetSquare);
                        _board[rookTargetSquare] = rookIndex;

                        var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        _uniqueIdentifier ^= hashChange;
                        _nonPawnHash[oldSide] ^= hashChange;
                        _majorHash ^= hashChange;

                        break;
                    }
                case SpecialMoveType.EnPassant:
                    {
                        var oppositePawnIndex = (int)Piece.p - offset;

                        var capturedSquare = Constants.EnPassantCaptureSquares[targetSquare];
                        var capturedPiece = oppositePawnIndex;
                        Utils.Assert(_pieceBitboards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");

                        _pieceBitboards[capturedPiece].PopBit(capturedSquare);
                        _occupancyBitboards[oppositeSide].PopBit(capturedSquare);
                        _board[capturedSquare] = (int)Piece.None;

                        ulong capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                        _uniqueIdentifier ^= capturedPawnHash;
                        _kingPawnUniqueIdentifier ^= capturedPawnHash;

                        break;
                    }
            }
        }

        _side = (Side)oppositeSide;
        _occupancyBitboards[2] = _occupancyBitboards[1] | _occupancyBitboards[0];

        // Updating castling rights
        _castle &= _castlingRightsUpdateConstants[sourceSquare];
        _castle &= _castlingRightsUpdateConstants[targetSquare];

        _uniqueIdentifier ^= ZobristTable.CastleHash(_castle);

        Debug.Assert(ZobristTable.PositionHash(this) == _uniqueIdentifier);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.White) == _nonPawnHash[(int)Side.White]);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.Black) == _nonPawnHash[(int)Side.Black]);
        Debug.Assert(ZobristTable.MinorHash(this) == _minorHash);
        Debug.Assert(ZobristTable.MajorHash(this) == _majorHash);
        Debug.Assert(Math.Min(MaxPhase, PhaseFromScratch()) == Phase());

        // KingPawn hash assert won't work due to PassedPawnBonusNoEnemiesAheadBonus
        //Debug.Assert(ZobristTable.PawnKingHash(this) != _kingPawnUniqueIdentifier && WasProduceByAValidMove());

        return gameState;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnmakeMove(Move move, GameState gameState)
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

        _pieceBitboards[newPiece].PopBit(targetSquare);
        _occupancyBitboards[side].PopBit(targetSquare);
        _board[targetSquare] = (int)Piece.None;

        // We purposely delay the sets here until after the switch

        switch (move.SpecialMoveFlag())
        {
            case SpecialMoveType.None:
                {
                    var capturedPiece = move.CapturedPiece();

                    if (capturedPiece != (int)Piece.None)
                    {
                        _pieceBitboards[capturedPiece].SetBit(targetSquare);
                        _occupancyBitboards[oppositeSide].SetBit(targetSquare);
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
                        _pieceBitboards[newPiece].PopBit(kingTargetSquare);
                        _occupancyBitboards[side].PopBit(kingTargetSquare);
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
                    _pieceBitboards[rookIndex].PopBit(rookTargetSquare);

                    // In DFRC the square where the rook ended could be occupied by the king before castling
                    // Since we set the king squares after the switch, we don't need the guard here
                    //if (rookTargetSquare != InitialKingSquares[side])
                    //{
                    _occupancyBitboards[side].PopBit(rookTargetSquare);
                    _board[rookTargetSquare] = (int)Piece.None;
                    //}

                    _pieceBitboards[rookIndex].SetBit(rookSourceSquare);
                    _occupancyBitboards[side].SetBit(rookSourceSquare);
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

                        _pieceBitboards[newPiece].PopBit(kingTargetSquare);
                        _occupancyBitboards[side].PopBit(kingTargetSquare);
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
                    _pieceBitboards[rookIndex].PopBit(rookTargetSquare);

                    // In DFRC the square where the rook ended could be occupied by the king before castling
                    // Since we set the king squares after the switch, we don't need the guard here
                    //if (rookTargetSquare != InitialKingSquares[side])
                    //{
                    _occupancyBitboards[side].PopBit(rookTargetSquare);
                    _board[rookTargetSquare] = (int)Piece.None;
                    //}

                    _pieceBitboards[rookIndex].SetBit(rookSourceSquare);
                    _occupancyBitboards[side].SetBit(rookSourceSquare);
                    _board[rookSourceSquare] = rookIndex;

                    break;
                }
            case SpecialMoveType.EnPassant:
                {
                    Debug.Assert(move.IsEnPassant());

                    var oppositePawnIndex = (int)Piece.p - offset;
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];

                    Utils.Assert(_occupancyBitboards[(int)Side.Both].GetBit(capturedPawnSquare) == default,
                        $"Expected empty {capturedPawnSquare}");

                    _pieceBitboards[oppositePawnIndex].SetBit(capturedPawnSquare);
                    _occupancyBitboards[oppositeSide].SetBit(capturedPawnSquare);
                    _board[capturedPawnSquare] = oppositePawnIndex;

                    break;
                }
        }

        _pieceBitboards[piece].SetBit(sourceSquare);
        _occupancyBitboards[side].SetBit(sourceSquare);
        _board[sourceSquare] = piece;

        _occupancyBitboards[2] = _occupancyBitboards[1] | _occupancyBitboards[0];

        // Updating saved values
        _castle = gameState.Castle;
        _enPassant = gameState.EnPassant;

        _uniqueIdentifier = gameState.ZobristKey;
        _kingPawnUniqueIdentifier = gameState.KingPawnKey;
        _minorHash = gameState.MinorKey;
        _majorHash = gameState.MajorKey;
        _nonPawnHash[(int)Side.White] = gameState.NonPawnWhiteKey;
        _nonPawnHash[(int)Side.Black] = gameState.NonPawnBlackKey;

        IncrementalEvalAccumulator = gameState.IncrementalEvalAccumulator;
        IncrementalPhaseAccumulator = gameState.IncrementalPhaseAccumulator;
        IsIncrementalEval = gameState.IsIncrementalEval;

        Validate();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullMoveGameState MakeNullMove()
    {
        var gameState = new NullMoveGameState(this);

        _uniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ ZobristTable.EnPassantHash((int)_enPassant);

        _side = (Side)Utils.OppositeSide((int)_side);
        _enPassant = BoardSquare.noSquare;

        Validate();

        return gameState;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnMakeNullMove(NullMoveGameState gameState)
    {
        _side = (Side)Utils.OppositeSide((int)_side);
        _enPassant = gameState.EnPassant;
        _uniqueIdentifier = gameState.ZobristKey;

        Validate();
    }

    /// <summary>
    /// False if any of the kings has been captured, or if the opponent king is in check.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsValid()
    {
        var offset = Utils.PieceOffset((int)_side);

        var kingBitboard = _pieceBitboards[(int)Piece.K + offset];
        var kingSquare = kingBitboard == default ? -1 : kingBitboard.GetLS1BIndex();

        var oppositeKingBitboard = _pieceBitboards[(int)Piece.k - offset];
        var oppositeKingSquare = oppositeKingBitboard == default ? -1 : oppositeKingBitboard.GetLS1BIndex();

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
        Debug.Assert(_pieceBitboards[(int)Piece.k - Utils.PieceOffset((int)_side)].CountBits() == 1);

        var oppositeKingSquare = _pieceBitboards[(int)Piece.k - Utils.PieceOffset((int)_side)].GetLS1BIndex();

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
    public ulong AllAttackersTo(int square, Bitboard occupancy, Bitboard rooks, Bitboard bishops)
    {
        Debug.Assert(square != (int)BoardSquare.noSquare);

        return (rooks & Attacks.RookAttacks(square, occupancy))
            | (bishops & Attacks.BishopAttacks(square, occupancy))
            | (_pieceBitboards[(int)Piece.p] & Attacks.PawnAttacks[(int)Side.White][square])
            | (_pieceBitboards[(int)Piece.P] & Attacks.PawnAttacks[(int)Side.Black][square])
            | (Knights & Attacks.KnightAttacks[square])
            | (Kings & Attacks.KingAttacks[square]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AllAttackersTo(int square)
    {
        Debug.Assert(square != (int)BoardSquare.noSquare);

        var occupancy = _occupancyBitboards[(int)Side.Both];
        var queens = Queens;
        var rooks = queens | Rooks;
        var bishops = queens | Bishops;

        return (rooks & Attacks.RookAttacks(square, occupancy))
            | (bishops & Attacks.BishopAttacks(square, occupancy))
            | (_pieceBitboards[(int)Piece.p] & Attacks.PawnAttacks[(int)Side.White][square])
            | (_pieceBitboards[(int)Piece.P] & Attacks.PawnAttacks[(int)Side.Black][square])
            | (Knights & Attacks.KnightAttacks[square])
            | (Kings & Attacks.KingAttacks[square]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AllSideAttackersTo(int square, int side)
    {
        Debug.Assert(square != (int)BoardSquare.noSquare);
        Debug.Assert(side != (int)Side.Both);

        var offset = Utils.PieceOffset(side);

        var occupancy = _occupancyBitboards[(int)Side.Both];

        var queens = _pieceBitboards[(int)Piece.q - offset];
        var rooks = queens | _pieceBitboards[(int)Piece.r - offset];
        var bishops = queens | _pieceBitboards[(int)Piece.b - offset];

        return (rooks & Attacks.RookAttacks(square, occupancy))
            | (bishops & Attacks.BishopAttacks(square, occupancy))
            | (_pieceBitboards[(int)Piece.p - offset] & Attacks.PawnAttacks[side][square])
            | (_pieceBitboards[(int)Piece.n - offset] & Attacks.KnightAttacks[square]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttacked(int squareIndex, Side sideToMove)
    {
        Debug.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);
        var bothSidesOccupancy = _occupancyBitboards[(int)Side.Both];

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

        var kingSquare = _pieceBitboards[(int)Piece.k - oppositeSideOffset].GetLS1BIndex();

        var bothSidesOccupancy = _occupancyBitboards[(int)Side.Both];

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

        return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & _pieceBitboards[offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByKnights(int squareIndex, int offset)
    {
        return (Attacks.KnightAttacks[squareIndex] & _pieceBitboards[(int)Piece.N + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByKing(int squareIndex, int offset)
    {
        return (Attacks.KingAttacks[squareIndex] & _pieceBitboards[(int)Piece.K + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByBishops(int squareIndex, int offset, Bitboard bothSidesOccupancy, out Bitboard bishopAttacks)
    {
        bishopAttacks = Attacks.BishopAttacks(squareIndex, bothSidesOccupancy);
        return (bishopAttacks & _pieceBitboards[(int)Piece.B + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByRooks(int squareIndex, int offset, Bitboard bothSidesOccupancy, out Bitboard rookAttacks)
    {
        rookAttacks = Attacks.RookAttacks(squareIndex, bothSidesOccupancy);
        return (rookAttacks & _pieceBitboards[(int)Piece.R + offset]) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSquareAttackedByQueens(int offset, Bitboard bishopAttacks, Bitboard rookAttacks)
    {
        var queenAttacks = Attacks.QueenAttacks(rookAttacks, bishopAttacks);
        return (queenAttacks & _pieceBitboards[(int)Piece.Q + offset]) != default;
    }

    #endregion

    public int CountPieces() => _pieceBitboards.Sum(b => b.CountBits());

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
                if (_pieceBitboards[pieceBoardIndex].GetBit(square))
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
            if ((_castle & (int)CastlingRights.WK) != default)
            {
                sb.Append('K');
            }
            if ((_castle & (int)CastlingRights.WQ) != default)
            {
                sb.Append('Q');
            }
            if ((_castle & (int)CastlingRights.BK) != default)
            {
                sb.Append('k');
            }
            if ((_castle & (int)CastlingRights.BQ) != default)
            {
                sb.Append('q');
            }
        }
        else
        {
            // Shredder-FEN style (always showing columns), no support for X-FEN style yet (showing KQkq when not-ambiguous)
            if ((_castle & (int)CastlingRights.WK) != default)
            {
                char file = (char)('A' + Constants.File[WhiteShortCastle.TargetSquare()]);
                sb.Append(file);
            }
            if ((_castle & (int)CastlingRights.WQ) != default)
            {
                char file = (char)('A' + Constants.File[WhiteLongCastle.TargetSquare()]);
                sb.Append(file);
            }
            if ((_castle & (int)CastlingRights.BK) != default)
            {
                char file = (char)('a' + Constants.File[BlackShortCastle.TargetSquare()]);
                sb.Append(file);
            }
            if ((_castle & (int)CastlingRights.BQ) != default)
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

        sb.Append(_enPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)_enPassant]);

        sb.Append(' ').Append(halfMovesWithoutCaptureOrPawnMove).Append(' ').Append(fullMoveClock);

        return sb.ToString();
    }

#pragma warning disable S106, S2228 // Standard outputs should not be used directly to log anything

    /// <summary>
    /// Combines <see cref="_pieceBitboards"/>, <see cref="_side"/>, <see cref="_castle"/> and <see cref="_enPassant"/>
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

                var squareIndex = BitboardExtensions.SquareIndex(rank, file);

                var piece = -1;

                for (int bbIndex = 0; bbIndex < _pieceBitboards.Length; ++bbIndex)
                {
                    if (_pieceBitboards[bbIndex].GetBit(squareIndex))
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
        Console.WriteLine($"    Enpassant:\t{(_enPassant == BoardSquare.noSquare ? "no" : Constants.Coordinates[(int)_enPassant])}");

        if (!Configuration.EngineSettings.IsChess960)
        {
            Console.WriteLine($"    Castling:\t" +
                $"{((_castle & (int)CastlingRights.WK) != default ? 'K' : '-')}" +
                $"{((_castle & (int)CastlingRights.WQ) != default ? 'Q' : '-')} | " +
                $"{((_castle & (int)CastlingRights.BK) != default ? 'k' : '-')}" +
                $"{((_castle & (int)CastlingRights.BQ) != default ? 'q' : '-')}");
        }
        else
        {
            char whiteKingSide = '-', whiteQueenside = '-', blackKingside = '-', blackQueenside = '-';

            if ((_castle & (int)CastlingRights.WK) != default)
            {
                whiteKingSide = (char)('A' + Constants.File[WhiteShortCastle.TargetSquare()]);
            }
            if ((_castle & (int)CastlingRights.WQ) != default)
            {
                whiteQueenside = (char)('A' + Constants.File[WhiteLongCastle.TargetSquare()]);
            }
            if ((_castle & (int)CastlingRights.BK) != default)
            {
                blackKingside = (char)('a' + Constants.File[BlackShortCastle.TargetSquare()]);
            }
            if ((_castle & (int)CastlingRights.BQ) != default)
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

                var squareIndex = BitboardExtensions.SquareIndex(rank, file);

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

        var whitePawns = _pieceBitboards[(int)Piece.P];
        var blackPawns = _pieceBitboards[(int)Piece.p];
        var whiteKnights = _pieceBitboards[(int)Piece.N];
        var whiteBishops = _pieceBitboards[(int)Piece.B];
        var whiteRooks = _pieceBitboards[(int)Piece.R];
        var whiteQueens = _pieceBitboards[(int)Piece.Q];
        var whiteKings = _pieceBitboards[(int)Piece.K];
        var blackKnights = _pieceBitboards[(int)Piece.n];
        var blackBishops = _pieceBitboards[(int)Piece.b];
        var blackRooks = _pieceBitboards[(int)Piece.r];
        var blackQueens = _pieceBitboards[(int)Piece.q];
        var blackKings = _pieceBitboards[(int)Piece.k];

        // No pawns in 1 and 8 ranks
        Debug.Assert((whitePawns & Constants.PawnSquares) == whitePawns, failureMessage, "White pawn(s) un 1-8");
        Debug.Assert((blackPawns & Constants.PawnSquares) == blackPawns, failureMessage, "Black pawn(s) un 1-8");

        // No side occupancy overlap
        Debug.Assert((_occupancyBitboards[(int)Side.White] & _occupancyBitboards[(int)Side.Black]) == 0, failureMessage, "White and Black overlap");

        // Side.Both occupancy overlap
        Debug.Assert((_occupancyBitboards[(int)Side.White] | _occupancyBitboards[(int)Side.Black]) == _occupancyBitboards[(int)Side.Both], failureMessage, "Occupancy not correct");

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
        if (_castle != 0)
        {
            var whiteKingSourceSquare = _initialKingSquares[(int)Side.White];

            // Castling rights and king/rook positions
            if ((_castle & (int)CastlingRights.WK) != 0)
            {
                Debug.Assert(whiteKings.GetBit(whiteKingSourceSquare), failureMessage, "No white king on e1 when short castling rights");

                Debug.Assert(_initialKingsideRookSquares[(int)Side.White] != CastlingData.DefaultValues, failureMessage, "White initial kingside rook not set");
                Debug.Assert(whiteRooks.GetBit(_initialKingsideRookSquares[(int)Side.White]), failureMessage, $"No white rook on {(BoardSquare)_initialKingsideRookSquares[(int)Side.White]} when short castling rights");
            }

            if ((_castle & (int)CastlingRights.WQ) != 0)
            {
                Debug.Assert(whiteKings.GetBit(whiteKingSourceSquare), failureMessage, "No white king on e1 when long castling rights");

                Debug.Assert(whiteRooks.GetBit(_initialQueensideRookSquares[(int)Side.White]), failureMessage, $"No white rook on {(BoardSquare)_initialQueensideRookSquares[(int)Side.White]} when long castling rights");
                Debug.Assert(_initialQueensideRookSquares[(int)Side.White] != CastlingData.DefaultValues, failureMessage, "White initial queenside rook not set");
            }

            var blackKingSourceSquare = _initialKingSquares[(int)Side.Black];

            if ((_castle & (int)CastlingRights.BK) != 0)
            {
                Debug.Assert(blackKings.GetBit(blackKingSourceSquare), failureMessage, "No black king on e8 when short castling rights");

                Debug.Assert(_initialKingsideRookSquares[(int)Side.Black] != CastlingData.DefaultValues, failureMessage, "Black initial kingside rook not set");
                Debug.Assert(blackRooks.GetBit(_initialKingsideRookSquares[(int)Side.Black]), failureMessage, $"No black rook on {(BoardSquare)_initialKingsideRookSquares[(int)Side.Black]} when short castling rights");
            }

            if ((_castle & (int)CastlingRights.BQ) != 0)
            {
                Debug.Assert(blackKings.GetBit(blackKingSourceSquare), failureMessage, "No black king on e8 when long castling rights");

                Debug.Assert(blackRooks.GetBit(_initialQueensideRookSquares[(int)Side.Black]), failureMessage, $"No black rook on {(BoardSquare)_initialQueensideRookSquares[(int)Side.Black]} when long castling rights");
                Debug.Assert(_initialQueensideRookSquares[(int)Side.Black] != CastlingData.DefaultValues, failureMessage, "Black initial queenside rook not set");
            }
        }
#endif

        // En-passant and pawn to be captured position
        if (_enPassant != BoardSquare.noSquare)
        {
            Debug.Assert(!_occupancyBitboards[(int)Side.Both].GetBit((int)_enPassant), failureMessage, $"Non-empty en passant square {_enPassant}");

            var rank = Constants.Rank[(int)_enPassant];
            Debug.Assert(rank == 2 || rank == 5, failureMessage, $"Wrong en-passant rank for {_enPassant}");

            var pawnToCaptureSquare = Constants.EnPassantCaptureSquares[(int)_enPassant];

            if (Side == Side.White)
            {
                Debug.Assert(blackPawns.GetBit(pawnToCaptureSquare), failureMessage, $"No black pawn on en-passant capture square for {_enPassant}");
            }
            else
            {
                Debug.Assert(whitePawns.GetBit(pawnToCaptureSquare), failureMessage, $"No white pawn on en-passant capture square for {_enPassant}");
            }
        }

        // Can't capture opponent's king
        Debug.Assert(!IsSquareAttacked(_pieceBitboards[(int)Piece.k - Utils.PieceOffset((int)_side)].GetLS1BIndex(), Side), failureMessage, "Can't capture opponent's king");

        Debug.Assert(Math.Min(MaxPhase, PhaseFromScratch()) == Phase(), failureMessage, $"Wrong incremental phase: {Phase()} vs from scratch {Math.Min(MaxPhase, PhaseFromScratch())}");
    }

    [Conditional("DEBUG")]
    private void AssertAttackPopulation(ref EvaluationContext evaluationContext)
    {
        var attacks = evaluationContext.Attacks;

        Debug.Assert(_pieceBitboards[(int)Piece.P] == 0 || attacks[(int)Piece.P] != 0);
        Debug.Assert(_pieceBitboards[(int)Piece.N] == 0 || attacks[(int)Piece.N] != 0);
        Debug.Assert(_pieceBitboards[(int)Piece.B] == 0 || attacks[(int)Piece.B] != 0);
        Debug.Assert(_pieceBitboards[(int)Piece.R] == 0 || attacks[(int)Piece.R] != 0);
        Debug.Assert(_pieceBitboards[(int)Piece.Q] == 0 || attacks[(int)Piece.Q] != 0);
        Debug.Assert(attacks[(int)Piece.K] != 0);
        Debug.Assert(evaluationContext.AttacksBySide[(int)Side.White] != 0);

        Debug.Assert(_pieceBitboards[(int)Piece.p] == 0 || attacks[(int)Piece.p] != 0);
        Debug.Assert(_pieceBitboards[(int)Piece.n] == 0 || attacks[(int)Piece.n] != 0);
        Debug.Assert(_pieceBitboards[(int)Piece.b] == 0 || attacks[(int)Piece.b] != 0);
        Debug.Assert(_pieceBitboards[(int)Piece.r] == 0 || attacks[(int)Piece.r] != 0);
        Debug.Assert(_pieceBitboards[(int)Piece.q] == 0 || attacks[(int)Piece.q] != 0);
        Debug.Assert(attacks[(int)Piece.k] != 0);
        Debug.Assert(evaluationContext.AttacksBySide[(int)Side.Black] != 0);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ArrayPool<Bitboard>.Shared.Return(_pieceBitboards);
                ArrayPool<Bitboard>.Shared.Return(_occupancyBitboards);

                ArrayPool<ulong>.Shared.Return(_nonPawnHash);
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
