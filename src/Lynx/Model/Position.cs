using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using static Lynx.EvaluationConstants;
using static Lynx.EvaluationParams;
using static Lynx.EvaluationPSQTs;

namespace Lynx.Model;

public class Position : IDisposable
{
    private bool _disposedValue;

    internal int _incrementalEvalAccumulator;
    internal int _incrementalPhaseAccumulator;
    internal bool _isIncrementalEval;

    private ulong _uniqueIdentifier;
    private ulong _kingPawnUniqueIdentifier;
    private readonly ulong[] _nonPawnHash;
    private ulong _minorHash;
    private ulong _majorHash;

    private readonly ulong[] _pieceBitBoards;
    private readonly ulong[] _occupancyBitBoards;
    private readonly int[] _board;

    private byte _castle;

#pragma warning disable S3887, CA1051
    private readonly byte[] _castlingRightsUpdateConstants;
    public readonly ulong[] KingsideCastlingFreeSquares;
    public readonly ulong[] KingsideCastlingNonAttackedSquares;
    public readonly ulong[] QueensideCastlingFreeSquares;
    public readonly ulong[] QueensideCastlingNonAttackedSquares;

#pragma warning disable IDE1006 // Naming Styles
    internal readonly int WhiteShortCastle;
    internal readonly int WhiteLongCastle;
    internal readonly int BlackShortCastle;
    internal readonly int BlackLongCastle;
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

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards => _pieceBitBoards;

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards => _occupancyBitBoards;

    /// <summary>
    /// Piece location indexed by square
    /// </summary>
    public int[] Board => _board;

    public Side Side => _side;

    public BoardSquare EnPassant => _enPassant;

    /// <summary>
    /// See <see cref="<CastlingRights"/>
    /// </summary>
    public byte Castle { get => _castle; private set => _castle = value; }

#pragma warning restore RCS1085 // Use auto-implemented property

    public BitBoard Queens => _pieceBitBoards[(int)Piece.Q] | _pieceBitBoards[(int)Piece.q];
    public BitBoard Rooks => _pieceBitBoards[(int)Piece.R] | _pieceBitBoards[(int)Piece.r];
    public BitBoard Bishops => _pieceBitBoards[(int)Piece.B] | _pieceBitBoards[(int)Piece.b];
    public BitBoard Knights => _pieceBitBoards[(int)Piece.N] | _pieceBitBoards[(int)Piece.n];
    public BitBoard Kings => _pieceBitBoards[(int)Piece.K] | _pieceBitBoards[(int)Piece.k];

    public int WhiteKingSquare => _pieceBitBoards[(int)Piece.K].GetLS1BIndex();
    public int BlackKingSquare => _pieceBitBoards[(int)Piece.k].GetLS1BIndex();

    /// <summary>
    /// Beware, half move counter isn't take into account
    /// Use alternative constructor instead and set it externally if relevant
    /// </summary>
    public Position(string fen) : this(FENParser.ParseFEN(fen))
    {
    }

    public Position(ParseFENResult parsedFEN)
    {
        _pieceBitBoards = parsedFEN.PieceBitBoards;
        _occupancyBitBoards = parsedFEN.OccupancyBitBoards;
        _board = parsedFEN.Board;

        _side = parsedFEN.Side;
        _castle = parsedFEN.Castle;
        _enPassant = parsedFEN.EnPassant;

#pragma warning disable S3366 // "this" should not be exposed from constructors
        _nonPawnHash = ArrayPool<ulong>.Shared.Rent(2);
        _nonPawnHash[(int)Side.White] = ZobristTable.NonPawnSideHash(this, (int)Side.White);
        _nonPawnHash[(int)Side.Black] = ZobristTable.NonPawnSideHash(this, (int)Side.Black);

        _minorHash = ZobristTable.MinorHash(this);
        _majorHash = ZobristTable.MajorHash(this);
        _kingPawnUniqueIdentifier = ZobristTable.KingPawnHash(this);

        _uniqueIdentifier = ZobristTable.PositionHash(this, _kingPawnUniqueIdentifier, _nonPawnHash[(int)Side.White], _nonPawnHash[(int)Side.Black]);

        Debug.Assert(_uniqueIdentifier == ZobristTable.PositionHash(this));
#pragma warning restore S3366 // "this" should not be exposed from constructors

        _isIncrementalEval = false;

        _castlingRightsUpdateConstants = ArrayPool<byte>.Shared.Rent(64);
        Array.Fill(_castlingRightsUpdateConstants, Constants.NoUpdateCastlingRight, 0, 64);

        // It won't be possible to add castling rights to a position created froma FEN without them
        if (_castle == (int)CastlingRights.None)
        {
            KingsideCastlingFreeSquares = [];
            QueensideCastlingFreeSquares = [];
            KingsideCastlingNonAttackedSquares = [];
            QueensideCastlingNonAttackedSquares = [];

#if DEBUG
            _initialKingSquares = [];
            _initialKingsideRookSquares = [];
            _initialQueensideRookSquares = [];
#endif
        }
        else
        {
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

            KingsideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);
            KingsideCastlingNonAttackedSquares[(int)Side.White] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingSquare, Constants.WhiteKingShortCastleSquare);
            KingsideCastlingNonAttackedSquares[(int)Side.Black] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingSquare, Constants.BlackKingShortCastleSquare);

            QueensideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);
            QueensideCastlingNonAttackedSquares[(int)Side.White] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(whiteKingSquare, Constants.WhiteKingLongCastleSquare);
            QueensideCastlingNonAttackedSquares[(int)Side.Black] = BitBoardExtensions.MaskBetweenTwoSquaresSameRankInclusive(blackKingSquare, Constants.BlackKingLongCastleSquare);

            // This could be simplified/harcoded for standard chess, see FreeAndNonAttackedSquares
            KingsideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);

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

            // This could be simplified/harcoded for standard chess, see FreeAndNonAttackedSquares
            QueensideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);

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
            _initialKingSquares = ArrayPool<int>.Shared.Rent(2);
            _initialKingSquares[(int)Side.White] = whiteKingSquare;
            _initialKingSquares[(int)Side.Black] = blackKingSquare;

            _initialKingsideRookSquares = ArrayPool<int>.Shared.Rent(2);
            _initialKingsideRookSquares[(int)Side.White] = whiteKingsideRook;
            _initialKingsideRookSquares[(int)Side.Black] = blackKingsideRook;

            _initialQueensideRookSquares = ArrayPool<int>.Shared.Rent(2);
            _initialQueensideRookSquares[(int)Side.White] = whiteQueensideRook;
            _initialQueensideRookSquares[(int)Side.Black] = blackQueensideRook;
#endif
        }

        Validate();
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Position(Position position)
    {
        _uniqueIdentifier = position._uniqueIdentifier;
        _kingPawnUniqueIdentifier = position._kingPawnUniqueIdentifier;
        _minorHash = position._minorHash;
        _majorHash = position._majorHash;

        _nonPawnHash = ArrayPool<ulong>.Shared.Rent(2);
        _nonPawnHash[(int)Side.White] = position._nonPawnHash[(int)Side.White];
        _nonPawnHash[(int)Side.Black] = position._nonPawnHash[(int)Side.Black];

        _pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
        Array.Copy(position._pieceBitBoards, _pieceBitBoards, 12);

        _occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
        Array.Copy(position._occupancyBitBoards, _occupancyBitBoards, 3);

        _board = ArrayPool<int>.Shared.Rent(64);
        Array.Copy(position._board, _board, 64);

        _side = position._side;
        _castle = position._castle;
        _enPassant = position._enPassant;

        _isIncrementalEval = position._isIncrementalEval;
        _incrementalEvalAccumulator = position._incrementalEvalAccumulator;
        _incrementalPhaseAccumulator = position._incrementalPhaseAccumulator;

        _castlingRightsUpdateConstants = ArrayPool<byte>.Shared.Rent(64);
        Array.Copy(position._castlingRightsUpdateConstants, _castlingRightsUpdateConstants, 64);

        // Avoid allocating arrays when the position to clone never had castling rights
        if (position.KingsideCastlingNonAttackedSquares.Length == 0)
        {
#if DEBUG
            _initialKingSquares = [];
            _initialKingsideRookSquares = [];
            _initialQueensideRookSquares = [];
#endif

            KingsideCastlingFreeSquares = [];
            QueensideCastlingFreeSquares = [];
            KingsideCastlingNonAttackedSquares = [];
            QueensideCastlingNonAttackedSquares = [];
        }
        else
        {
#if DEBUG
            _initialKingSquares = ArrayPool<int>.Shared.Rent(2);
            _initialKingSquares[(int)Side.White] = position._initialKingSquares[(int)Side.White];
            _initialKingSquares[(int)Side.Black] = position._initialKingSquares[(int)Side.Black];

            _initialKingsideRookSquares = ArrayPool<int>.Shared.Rent(2);
            _initialKingsideRookSquares[(int)Side.White] = position._initialKingsideRookSquares[(int)Side.White];
            _initialKingsideRookSquares[(int)Side.Black] = position._initialKingsideRookSquares[(int)Side.Black];

            _initialQueensideRookSquares = ArrayPool<int>.Shared.Rent(2);
            _initialQueensideRookSquares[(int)Side.White] = position._initialQueensideRookSquares[(int)Side.White];
            _initialQueensideRookSquares[(int)Side.Black] = position._initialQueensideRookSquares[(int)Side.Black];
#endif

            KingsideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);
            KingsideCastlingFreeSquares[(int)Side.White] = position.KingsideCastlingFreeSquares[(int)Side.White];
            KingsideCastlingFreeSquares[(int)Side.Black] = position.KingsideCastlingFreeSquares[(int)Side.Black];

            KingsideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);
            KingsideCastlingNonAttackedSquares[(int)Side.White] = position.KingsideCastlingNonAttackedSquares[(int)Side.White];
            KingsideCastlingNonAttackedSquares[(int)Side.Black] = position.KingsideCastlingNonAttackedSquares[(int)Side.Black];

            QueensideCastlingFreeSquares = ArrayPool<ulong>.Shared.Rent(2);
            QueensideCastlingFreeSquares[(int)Side.White] = position.QueensideCastlingFreeSquares[(int)Side.White];
            QueensideCastlingFreeSquares[(int)Side.Black] = position.QueensideCastlingFreeSquares[(int)Side.Black];

            QueensideCastlingNonAttackedSquares = ArrayPool<ulong>.Shared.Rent(2);
            QueensideCastlingNonAttackedSquares[(int)Side.White] = position.QueensideCastlingNonAttackedSquares[(int)Side.White];
            QueensideCastlingNonAttackedSquares[(int)Side.Black] = position.QueensideCastlingNonAttackedSquares[(int)Side.Black];

            WhiteShortCastle = position.WhiteShortCastle;
            WhiteLongCastle = position.WhiteLongCastle;
            BlackShortCastle = position.BlackShortCastle;
            BlackLongCastle = position.BlackLongCastle;
        }

        Validate();
    }

    public int InitialKingSquare(int side) => side == (int)Side.White
        ? WhiteShortCastle.SourceSquare()
        : BlackShortCastle.SourceSquare();

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

        _pieceBitBoards[piece].PopBit(sourceSquare);
        _occupancyBitBoards[oldSide].PopBit(sourceSquare);
        _board[sourceSquare] = (int)Piece.None;

        _pieceBitBoards[newPiece].SetBit(targetSquare);
        _occupancyBitBoards[oldSide].SetBit(targetSquare);
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
                _isIncrementalEval = false;

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
        if (_isIncrementalEval)
        {
            var whiteKing = _pieceBitBoards[(int)Piece.K].GetLS1BIndex();
            var blackKing = _pieceBitBoards[(int)Piece.k].GetLS1BIndex();
            var whiteBucket = PSQTBucketLayout[whiteKing];
            var blackBucket = PSQTBucketLayout[blackKing ^ 56];

            int sameSideBucket = whiteBucket;
            int opposideSideBucket = blackBucket;
            if (_side == Side.Black)
            {
                (sameSideBucket, opposideSideBucket) = (opposideSideBucket, sameSideBucket);
            }

            _incrementalEvalAccumulator -= PSQT(0, sameSideBucket, piece, sourceSquare);
            _incrementalEvalAccumulator -= PSQT(1, opposideSideBucket, piece, sourceSquare);

            _incrementalEvalAccumulator += PSQT(0, sameSideBucket, newPiece, targetSquare);
            _incrementalEvalAccumulator += PSQT(1, opposideSideBucket, newPiece, targetSquare);

            _incrementalPhaseAccumulator += extraPhaseIfIncremental;

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

                            _incrementalEvalAccumulator -= PSQT(0, opposideSideBucket, capturedPiece, capturedSquare);
                            _incrementalEvalAccumulator -= PSQT(1, sameSideBucket, capturedPiece, capturedSquare);

                            _incrementalPhaseAccumulator -= GamePhaseByPiece[capturedPiece];
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
                        Utils.Assert(_pieceBitBoards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");

                        _pieceBitBoards[capturedPiece].PopBit(capturedSquare);
                        _occupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                        _board[capturedSquare] = (int)Piece.None;

                        var capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                        _uniqueIdentifier ^= capturedPawnHash;
                        _kingPawnUniqueIdentifier ^= capturedPawnHash;

                        _incrementalEvalAccumulator -= PSQT(0, opposideSideBucket, capturedPiece, capturedSquare);
                        _incrementalEvalAccumulator -= PSQT(1, sameSideBucket, capturedPiece, capturedSquare);

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

                        _pieceBitBoards[rookIndex].PopBit(rookSourceSquare);

                        var kingTargetSquare = Utils.KingShortCastleSquare(oldSide);

                        if (Configuration.EngineSettings.IsChess960)
                        {
                            // In DFRC castling moves are encoded as KxR, so the target square in the move isn't really the king target square
                            // We need to revert the incorect changes + apply the right ones
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

                            _uniqueIdentifier ^= hashFix;
                            _nonPawnHash[oldSide] ^= hashFix;
                            _kingPawnUniqueIdentifier ^= hashFix;
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

                        _pieceBitBoards[rookIndex].PopBit(rookSourceSquare);

                        var kingTargetSquare = Utils.KingLongCastleSquare(oldSide);

                        if (Configuration.EngineSettings.IsChess960)
                        {
                            // In DFRC castling moves are encoded as KxR, so the target square in the move isn't really the king target square
                            // We need to revert the incorect changes + apply the right ones
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

                            _uniqueIdentifier ^= hashFix;
                            _nonPawnHash[oldSide] ^= hashFix;
                            _kingPawnUniqueIdentifier ^= hashFix;
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
                        Utils.Assert(_pieceBitBoards[oppositePawnIndex].GetBit(capturedSquare), $"Expected {(Side)oppositeSide} pawn in {capturedSquare}");

                        _pieceBitBoards[capturedPiece].PopBit(capturedSquare);
                        _occupancyBitBoards[oppositeSide].PopBit(capturedSquare);
                        _board[capturedSquare] = (int)Piece.None;

                        ulong capturedPawnHash = ZobristTable.PieceHash(capturedSquare, capturedPiece);
                        _uniqueIdentifier ^= capturedPawnHash;
                        _kingPawnUniqueIdentifier ^= capturedPawnHash;

                        break;
                    }
            }
        }

        _side = (Side)oppositeSide;
        _occupancyBitBoards[2] = _occupancyBitBoards[1] | _occupancyBitBoards[0];

        // Updating castling rights
        _castle &= _castlingRightsUpdateConstants[sourceSquare];
        _castle &= _castlingRightsUpdateConstants[targetSquare];

        _uniqueIdentifier ^= ZobristTable.CastleHash(_castle);

        Debug.Assert(ZobristTable.PositionHash(this) == _uniqueIdentifier);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.White) == _nonPawnHash[(int)Side.White]);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.Black) == _nonPawnHash[(int)Side.Black]);
        Debug.Assert(ZobristTable.MinorHash(this) == _minorHash);
        Debug.Assert(ZobristTable.MajorHash(this) == _majorHash);

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

        _pieceBitBoards[newPiece].PopBit(targetSquare);
        _occupancyBitBoards[side].PopBit(targetSquare);
        _board[targetSquare] = (int)Piece.None;

        // We purposedly delay the sets here until after the switch

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
                        // have already been undone by the rook ops above, or don't matter (removig the king from the target square, where it isn't anyway)

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
                        // have already been undone by the rook ops above, or don't matter (removig the king from the target square, where it isn't anyway)

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

        // Updating saved values
        _castle = gameState.Castle;
        _enPassant = gameState.EnPassant;

        _uniqueIdentifier = gameState.ZobristKey;
        _kingPawnUniqueIdentifier = gameState.KingPawnKey;
        _minorHash = gameState.MinorKey;
        _majorHash = gameState.MajorKey;
        _nonPawnHash[(int)Side.White] = gameState.NonPawnWhiteKey;
        _nonPawnHash[(int)Side.Black] = gameState.NonPawnBlackKey;

        _incrementalEvalAccumulator = gameState.IncrementalEvalAccumulator;
        _incrementalPhaseAccumulator = gameState.IncrementalPhaseAccumulator;
        _isIncrementalEval = gameState.IsIncrementalEval;

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
            Debug.Assert(!_occupancyBitBoards[(int)Side.Both].GetBit((int)_enPassant), failureMessage, $"Non-empty en passant square {_enPassant}");

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
        Debug.Assert(!IsSquareAttacked(_pieceBitBoards[(int)Piece.k - Utils.PieceOffset((int)_side)].GetLS1BIndex(), Side), failureMessage, "Can't capture opponent's king");
    }

    #endregion

    #region Evaluation

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="_side"/>.
    /// </summary>
    public (int Score, int Phase) StaticEvaluation() => StaticEvaluation(0);

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="_side"/>.
    /// </summary>
    public (int Score, int Phase) StaticEvaluation(int movesWithoutCaptureOrPawnMove)
    {
        var kingPawnTable = new PawnTableElement[Constants.KingPawnHashSize];

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        return StaticEvaluation(movesWithoutCaptureOrPawnMove, kingPawnTable, ref evaluationContext);
    }

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="_side"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Score, int Phase) StaticEvaluation(int movesWithoutCaptureOrPawnMove, PawnTableElement[] pawnEvalTable, ref EvaluationContext evaluationContext)
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

        var whitePawns = _pieceBitBoards[(int)Piece.P];
        var blackPawns = _pieceBitBoards[(int)Piece.p];

        BitBoard whitePawnAttacks = whitePawns.ShiftUpRightAndLeft();
        BitBoard blackPawnAttacks = blackPawns.ShiftDownRightAndLeft();

        evaluationContext.AttacksBySide[(int)Side.White] = evaluationContext.Attacks[(int)Piece.P] = whitePawnAttacks;
        evaluationContext.AttacksBySide[(int)Side.Black] = evaluationContext.Attacks[(int)Piece.p] = blackPawnAttacks;

        var whiteKing = _pieceBitBoards[(int)Piece.K].GetLS1BIndex();
        var blackKing = _pieceBitBoards[(int)Piece.k].GetLS1BIndex();

        var whiteBucket = PSQTBucketLayout[whiteKing];
        var blackBucket = PSQTBucketLayout[blackKing ^ 56];

        if (_isIncrementalEval)
        {
            packedScore = _incrementalEvalAccumulator;
            gamePhase = _incrementalPhaseAccumulator;

            var kingPawnIndex = _kingPawnUniqueIdentifier & Constants.KingPawnHashMask;
            ref var entry = ref pawnEvalTable[kingPawnIndex];

            // pawnEvalTable hit: We can reuse cached eval for pawn additional evaluation + PieceProtectedByPawnBonus + KingShieldBonus
            if (entry.Key == _kingPawnUniqueIdentifier)
            {
                packedScore += entry.PackedScore;
            }
            // Not hit in pawnEvalTable table
            else
            {
                var pawnScore = 0;

                // White pawns

                // King pawn shield bonus
                pawnScore += KingPawnShield(whiteKing, whitePawns);

                // Pieces protected by pawns bonus
                pawnScore += PieceProtectedByPawnBonus[(int)Piece.P] * (whitePawnAttacks & whitePawns).CountBits();

                // Bitboard copy that we 'empty'
                var whitePawnsCopy = whitePawns;
                while (whitePawnsCopy != default)
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    pawnScore += PawnAdditionalEvaluation(ref evaluationContext, whiteBucket, blackBucket, pieceSquareIndex, (int)Piece.P, whiteKing, blackKing);
                }

                // Black pawns

                // King pawn shield bonus
                pawnScore -= KingPawnShield(blackKing, blackPawns);

                // Pieces protected by pawns bonus
                pawnScore -= PieceProtectedByPawnBonus[(int)Piece.P] * (blackPawnAttacks & blackPawns).CountBits();

                // Bitboard copy that we 'empty'
                var blackPawnsCopy = blackPawns;
                while (blackPawnsCopy != default)
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    pawnScore -= PawnAdditionalEvaluation(ref evaluationContext, blackBucket, whiteBucket, pieceSquareIndex, (int)Piece.p, blackKing, whiteKing);
                }

                // Pawn islands
                pawnScore += PawnIslands(whitePawns, blackPawns);

                entry.Update(_kingPawnUniqueIdentifier, pawnScore);
                packedScore += pawnScore;
            }

            // White pieces additional eval and pawn attacks, except pawn and king
            for (int pieceIndex = (int)Piece.N; pieceIndex < (int)Piece.K; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                packedScore += PieceProtectedByPawnBonus[pieceIndex] * (whitePawnAttacks & bitboard).CountBits();

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    packedScore += AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, whiteBucket, blackBucket, pieceIndex, (int)Side.White, blackPawnAttacks, blackKing);
                }
            }

            // Black pieces additional eval and pawn attacks, except pawn and king
            for (int pieceIndex = (int)Piece.n; pieceIndex < (int)Piece.k; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                // Pieces protected by pawns bonus
                packedScore -= PieceProtectedByPawnBonus[pieceIndex - 6] * (blackPawnAttacks & bitboard).CountBits();

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    packedScore -= AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, blackBucket, whiteBucket, pieceIndex, (int)Side.Black, whitePawnAttacks, whiteKing);
                }
            }
        }
        else
        {
            _incrementalEvalAccumulator = 0;
            _incrementalPhaseAccumulator = 0;

            var kingPawnIndex = _kingPawnUniqueIdentifier & Constants.KingPawnHashMask;
            ref var entry = ref pawnEvalTable[kingPawnIndex];

            // pawnTable hit: We can reuse cached eval for pawn additional evaluation + PieceProtectedByPawnBonus + KingShieldBonus
            if (entry.Key == _kingPawnUniqueIdentifier)
            {
                packedScore += entry.PackedScore;

                // White pawns
                // No PieceProtectedByPawnBonus - included in pawn table | packedScore += PieceProtectedByPawnBonus[...]

                // Bitboard copy that we 'empty'
                var whitePawnsCopy = _pieceBitBoards[(int)Piece.P];
                while (whitePawnsCopy != default)
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    _incrementalEvalAccumulator += PSQT(0, whiteBucket, (int)Piece.P, pieceSquareIndex)
                                                + PSQT(1, blackBucket, (int)Piece.P, pieceSquareIndex);

                    // No incremental eval - included in pawn table | packedScore += AdditionalPieceEvaluation(...);
                }

                // Black pawns
                // No PieceProtectedByPawnBonus - included in pawn table | packedScore -= PieceProtectedByPawnBonus .Length[...]

                // Bitboard copy that we 'empty'
                var blackPawnsCopy = _pieceBitBoards[(int)Piece.p];
                while (blackPawnsCopy != default)
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    _incrementalEvalAccumulator += PSQT(0, blackBucket, (int)Piece.p, pieceSquareIndex)
                                                + PSQT(1, whiteBucket, (int)Piece.p, pieceSquareIndex);

                    // No incremental eval - included in pawn table | packedScore -= AdditionalPieceEvaluation(...);
                }
            }
            // Not hit in pawnTable table
            else
            {
                var pawnScore = 0;

                // White pawns

                // King pawn shield bonus
                pawnScore += KingPawnShield(whiteKing, whitePawns);

                // Pieces protected by pawns bonus
                pawnScore += PieceProtectedByPawnBonus[(int)Piece.P] * (whitePawnAttacks & whitePawns).CountBits();

                // Bitboard copy that we 'empty'
                var whitePawnsCopy = _pieceBitBoards[(int)Piece.P];
                while (whitePawnsCopy != default)
                {
                    whitePawnsCopy = whitePawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    _incrementalEvalAccumulator += PSQT(0, whiteBucket, (int)Piece.P, pieceSquareIndex)
                                                + PSQT(1, blackBucket, (int)Piece.P, pieceSquareIndex);

                    pawnScore += PawnAdditionalEvaluation(ref evaluationContext, whiteBucket, blackBucket, pieceSquareIndex, (int)Piece.P, whiteKing, blackKing);
                }

                // Black pawns

                // King pawn shield bonus
                pawnScore -= KingPawnShield(blackKing, blackPawns);

                // Pieces protected by pawns bonus
                pawnScore -= PieceProtectedByPawnBonus[(int)Piece.P] * (blackPawnAttacks & blackPawns).CountBits();

                // Bitboard copy that we 'empty'
                var blackPawnsCopy = _pieceBitBoards[(int)Piece.p];
                while (blackPawnsCopy != default)
                {
                    blackPawnsCopy = blackPawnsCopy.WithoutLS1B(out var pieceSquareIndex);

                    _incrementalEvalAccumulator += PSQT(0, blackBucket, (int)Piece.p, pieceSquareIndex)
                                                + PSQT(1, whiteBucket, (int)Piece.p, pieceSquareIndex);

                    pawnScore -= PawnAdditionalEvaluation(ref evaluationContext, blackBucket, whiteBucket, pieceSquareIndex, (int)Piece.p, blackKing, whiteKing);
                }

                // Pawn islands
                pawnScore += PawnIslands(whitePawns, blackPawns);

                entry.Update(_kingPawnUniqueIdentifier, pawnScore);
                packedScore += pawnScore;
            }

            // White pieces PSQTs and additional eval and pawn attacks, except king and pawn
            for (int pieceIndex = (int)Piece.N; pieceIndex < (int)Piece.K; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                packedScore += PieceProtectedByPawnBonus[pieceIndex] * (whitePawnAttacks & bitboard).CountBits();

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    _incrementalEvalAccumulator += PSQT(0, whiteBucket, pieceIndex, pieceSquareIndex)
                                                + PSQT(1, blackBucket, pieceIndex, pieceSquareIndex);

                    _incrementalPhaseAccumulator += GamePhaseByPiece[pieceIndex];

                    packedScore += AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, whiteBucket, blackBucket, pieceIndex, (int)Side.White, blackPawnAttacks, blackKing);
                }
            }

            // Black pieces PSQTs and additional eval and pawn attacks, except king and pawn
            for (int pieceIndex = (int)Piece.n; pieceIndex < (int)Piece.k; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                // Pieces protected by pawns bonus
                packedScore -= PieceProtectedByPawnBonus[pieceIndex - 6] * (blackPawnAttacks & bitboard).CountBits();

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    _incrementalEvalAccumulator += PSQT(0, blackBucket, pieceIndex, pieceSquareIndex)
                                                + PSQT(1, whiteBucket, pieceIndex, pieceSquareIndex);

                    _incrementalPhaseAccumulator += GamePhaseByPiece[pieceIndex];

                    packedScore -= AdditionalPieceEvaluation(ref evaluationContext, pieceSquareIndex, blackBucket, whiteBucket, pieceIndex, (int)Side.Black, whitePawnAttacks, whiteKing);
                }
            }

            packedScore += _incrementalEvalAccumulator;
            gamePhase += _incrementalPhaseAccumulator;
            _isIncrementalEval = true;
        }

        // Kings - they can't be incremental due to the king buckets
        packedScore +=
            PSQT(0, whiteBucket, (int)Piece.K, whiteKing)
            + PSQT(1, blackBucket, (int)Piece.K, whiteKing)
            + PSQT(0, blackBucket, (int)Piece.k, blackKing)
            + PSQT(1, whiteBucket, (int)Piece.k, blackKing);

        packedScore +=
            KingAdditionalEvaluation(ref evaluationContext, whiteKing, whiteBucket, (int)Side.White, blackPawnAttacks)
            - KingAdditionalEvaluation(ref evaluationContext, blackKing, blackBucket, (int)Side.Black, whitePawnAttacks);

        AssertAttackPopulation(ref evaluationContext);

        // Total king rings ttacks
        packedScore +=
            TotalKingRingAttacksBonus[Math.Min(13, evaluationContext.WhiteKingRingAttacks)]
            - TotalKingRingAttacksBonus[Math.Min(13, evaluationContext.BlackKingRingAttacks)];

        // Bishop pair bonus
        if (_pieceBitBoards[(int)Piece.B].CountBits() >= 2)
        {
            packedScore += BishopPairBonus;
        }

        if (_pieceBitBoards[(int)Piece.b].CountBits() >= 2)
        {
            packedScore -= BishopPairBonus;
        }

        // Pieces attacked by pawns bonus
        packedScore += PieceAttackedByPawnPenalty
            * ((blackPawnAttacks & _occupancyBitBoards[(int)Side.White] /* & (~whitePawns) */).CountBits()
                - (whitePawnAttacks & _occupancyBitBoards[(int)Side.Black] /* & (~blackPawns) */).CountBits());

        // Threats
        packedScore += Threats(evaluationContext, oppositeSide: (int)Side.Black)
            - Threats(evaluationContext, oppositeSide: (int)Side.White);

        // Checks
        packedScore += Checks(evaluationContext, (int)Side.White, (int)Side.Black)
            - Checks(evaluationContext, (int)Side.Black, (int)Side.White);

        if (gamePhase > MaxPhase)    // Early promotions
        {
            gamePhase = MaxPhase;
        }

        int endGamePhase = MaxPhase - gamePhase;

        var middleGameScore = Utils.UnpackMG(packedScore);
        var endGameScore = Utils.UnpackEG(packedScore);
        var eval = ((middleGameScore * gamePhase) + (endGameScore * endGamePhase)) / MaxPhase;

        int totalPawnsCount = whitePawns.CountBits() + blackPawns.CountBits();

        // Few pieces endgames
        if (gamePhase <= 5)
        {
            // Pawnless endgames
            if (totalPawnsCount == 0)
            {
                switch (gamePhase)
                {
                    case 5:
                        {
                            // RB vs R, RN vs R - scale it down due to the chances of it being a draw
                            if (_pieceBitBoards[(int)Piece.R].CountBits() == 1 && _pieceBitBoards[(int)Piece.r].CountBits() == 1)
                            {
                                eval >>= 1; // /2
                            }

                            break;
                        }
                    case 4:
                        {
                            // Rook vs 2 minors and R vs r should be a draw
                            if ((_pieceBitBoards[(int)Piece.R] != 0 && (_pieceBitBoards[(int)Piece.B] | _pieceBitBoards[(int)Piece.N]) == 0)
                                || (_pieceBitBoards[(int)Piece.r] != 0 && (_pieceBitBoards[(int)Piece.b] | _pieceBitBoards[(int)Piece.n]) == 0))
                            {
                                eval >>= 1; // /2
                            }

                            break;
                        }
                    case 3:
                        {
                            var winningSideOffset = Utils.PieceOffset(eval >= 0);

                            if (_pieceBitBoards[(int)Piece.N + winningSideOffset].CountBits() == 2)      // NN vs N, NN vs B
                            {
                                return (0, gamePhase);
                            }

                            // Rook vs a minor is a draw
                            // Without rooks, only BB vs N is a win and BN vs N can have some chances

                            eval >>= 1; // /2

                            break;
                        }
                    case 2:
                        {
                            var whiteKnightsCount = _pieceBitBoards[(int)Piece.N].CountBits();

                            if (whiteKnightsCount + _pieceBitBoards[(int)Piece.n].CountBits() == 2            // NN vs -, N vs N
                                    || whiteKnightsCount + _pieceBitBoards[(int)Piece.B].CountBits() == 1)    // B vs N, B vs B
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
            else
            {
                var winningSideOffset = Utils.PieceOffset(eval >= 0);

                if (gamePhase == 1)
                {
                    // Bishop vs A/H pawns: if the defending king reaches the corner, and the corner is the opposite color of the bishop, it's a draw
                    // TODO implement that
                    // For now, we reduce all endgames that only have one bishop and A/H pawns
                    if (_pieceBitBoards[(int)Piece.B + winningSideOffset] != 0
                        && (_pieceBitBoards[(int)Piece.P + winningSideOffset] & Constants.NotAorH) == 0)
                    {
                        eval >>= 1; // /2
                    }
                }
                else if (gamePhase == 2)
                {
                    var whiteBishops = _pieceBitBoards[(int)Piece.B];
                    var blackBishops = _pieceBitBoards[(int)Piece.b];

                    // Opposite color bishop endgame with pawns are drawish
                    if (whiteBishops > 0
                        && blackBishops > 0
                        && Constants.DarkSquares[whiteBishops.GetLS1BIndex()] !=
                            Constants.DarkSquares[blackBishops.GetLS1BIndex()])
                    {
                        eval >>= 1; // /2
                    }
                }
            }
        }

        // Endgame scaling with pawn count, formula yoinked from Sirius
        eval = (int)(eval * ((80 + (totalPawnsCount * 7)) / 128.0));

        // 50 moves rule distance scaling
        eval = ScaleEvalWith50MovesDrawDistance(eval, movesWithoutCaptureOrPawnMove);

        eval = Math.Clamp(eval, MinStaticEval, MaxStaticEval);

        var sideEval = _side == Side.White
            ? eval
            : -eval;

        return (sideEval, gamePhase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Phase()
    {
        int gamePhase =
             ((_pieceBitBoards[(int)Piece.N] | _pieceBitBoards[(int)Piece.n]).CountBits() * GamePhaseByPiece[(int)Piece.N])
            + ((_pieceBitBoards[(int)Piece.B] | _pieceBitBoards[(int)Piece.b]).CountBits() * GamePhaseByPiece[(int)Piece.B])
            + ((_pieceBitBoards[(int)Piece.R] | _pieceBitBoards[(int)Piece.r]).CountBits() * GamePhaseByPiece[(int)Piece.R])
            + ((_pieceBitBoards[(int)Piece.Q] | _pieceBitBoards[(int)Piece.q]).CountBits() * GamePhaseByPiece[(int)Piece.Q]);

        if (gamePhase > MaxPhase)    // Early promotions
        {
            gamePhase = MaxPhase;
        }

        return gamePhase;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int TaperedEvaluation(int taperedEvaluationTerm, int phase)
    {
        return ((Utils.UnpackMG(taperedEvaluationTerm) * phase) + (Utils.UnpackEG(taperedEvaluationTerm) * (24 - phase))) / 24;
    }

    /// <summary>
    /// Assuming a current position has no legal moves (<see cref="AllPossibleMoves"/> doesn't produce any <see cref="IsValid"/> position),
    /// this method determines if a position is a result of either a loss by checkmate or a draw by stalemate.
    /// NegaMax style
    /// </summary>
    /// <param name="ply">Modulates the output, favouring positions with lower ply (i.e. Checkmate in less moves)</param>
    /// <returns>At least <see cref="CheckMateEvaluation"/> if Position.Side lost (more extreme values when <paramref name="ply"/> increases)
    /// or 0 if Position.Side was stalemated</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int EvaluateFinalPosition(int ply, bool isInCheck)
    {
        if (isInCheck)
        {
            // Checkmate evaluation, but not as bad/shallow as it looks like since we're already searching at a certain depth
            return -CheckMateBaseEvaluation + ply;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Doesn't include <see cref="Piece.P"/>, <see cref="Piece.p"/>, <see cref="Piece.K"/> and <see cref="Piece.k"/> evaluation
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdditionalPieceEvaluation(ref EvaluationContext evaluationContext, int pieceSquareIndex, int bucket, int oppositeSideBucket, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        return pieceIndex switch
        {
            (int)Piece.R or (int)Piece.r => RookAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, bucket, oppositeSideBucket, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.B or (int)Piece.b => BishopAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.N or (int)Piece.n => KnightAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.Q or (int)Piece.q => QueenAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            _ => 0
        };
    }

    /// <summary>
    /// Doesn't include <see cref="Piece.K"/> and <see cref="Piece.k"/> evaluation
    /// </summary>
    [Obsolete("Test only")]
    internal int AdditionalPieceEvaluation(ref EvaluationContext evaluationContext, int bucket, int oppositeSideBucket, int pieceSquareIndex, int pieceIndex, int pieceSide, int sameSideKingSquare, int oppositeSideKingSquare, BitBoard enemyPawnAttacks)
    {
        return pieceIndex switch
        {
            (int)Piece.P or (int)Piece.p => PawnAdditionalEvaluation(ref evaluationContext, bucket, oppositeSideBucket, pieceSquareIndex, pieceIndex, sameSideKingSquare, oppositeSideKingSquare),

            (int)Piece.R or (int)Piece.r => RookAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, bucket, oppositeSideBucket, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.B or (int)Piece.b => BishopAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.N or (int)Piece.n => KnightAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            (int)Piece.Q or (int)Piece.q => QueenAdditionalEvaluation(ref evaluationContext, pieceSquareIndex, pieceIndex, pieceSide, enemyPawnAttacks, oppositeSideKingSquare),
            _ => 0
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PawnAdditionalEvaluation(ref EvaluationContext evaluationContext, int bucket, int oppositeSideBucket, int squareIndex, int pieceIndex, int sameSideKingSquare, int oppositeSideKingSquare)
    {
        int packedBonus = 0;

        var rank = Constants.Rank[squareIndex];
        var side = (int)Side.White;
        var oppositeSide = (int)Side.Black;
        ulong passedPawnsMask;
        int pushSquare;

        if (pieceIndex == (int)Piece.p)
        {
            rank = 7 - rank;
            (side, oppositeSide) = (oppositeSide, side);
            passedPawnsMask = Masks.BlackPassedPawnMasks[squareIndex];
            pushSquare = squareIndex + 8;
        }
        else
        {
            passedPawnsMask = Masks.WhitePassedPawnMasks[squareIndex];
            pushSquare = squareIndex - 8;
        }

        var oppositeSidePawnsIndex = (int)Piece.p - pieceIndex;
        var oppositeSidePawns = _pieceBitBoards[oppositeSidePawnsIndex];
        var oppositeSidePieces = _occupancyBitBoards[oppositeSide];

        var thisPawnAttacks = Attacks.PawnAttacks[side][squareIndex];
        var thisSidePawnAttacks = evaluationContext.Attacks[pieceIndex];
        var oppositeSidePawnAttacks = evaluationContext.Attacks[oppositeSidePawnsIndex];

        // Isolated pawn
        if ((_pieceBitBoards[pieceIndex] & Masks.IsolatedPawnMasks[squareIndex]) == 0
            && (thisPawnAttacks & oppositeSidePawns) == 0)
        {
            packedBonus += IsolatedPawnPenalty[Constants.File[squareIndex]];
        }
        // Backwards pawn
        else if (!thisSidePawnAttacks.GetBit(squareIndex)
            && (oppositeSidePawns.GetBit(pushSquare)                                            // Blocked
                || oppositeSidePawnAttacks.GetBit(pushSquare)))    // Push square attacked by opponent pawns
        {
            packedBonus += BackwardsPawnPenalty[rank];
        }

        // Passed pawn
        if ((oppositeSidePawns & passedPawnsMask) == default)
        {
            // Passed pawn without opponent pieces ahead (in its passed pawn mask)
            if ((passedPawnsMask & oppositeSidePieces) == 0)
            {
                packedBonus += PassedPawnBonusNoEnemiesAheadBonus[bucket][rank];
                packedBonus += PassedPawnBonusNoEnemiesAheadEnemyBonus[oppositeSideBucket][rank];
            }

            // King distance to passed pawn
            var friendlyKingDistance = Constants.ChebyshevDistance[squareIndex][sameSideKingSquare];

            // Enemy king distance to passed pawn
            var enemyKingDistance = Constants.ChebyshevDistance[squareIndex][oppositeSideKingSquare];

            packedBonus += PassedPawnBonus[bucket][rank]
                + PassedPawnEnemyBonus[oppositeSideBucket][rank]
                + FriendlyKingDistanceToPassedPawnBonus[friendlyKingDistance]
                + EnemyKingDistanceToPassedPawnPenalty[enemyKingDistance];
        }

        // Pawn phalanx
        if (Constants.File[squareIndex] != 7 && _pieceBitBoards[pieceIndex].GetBit(squareIndex + 1))
        {
            packedBonus += PawnPhalanxBonus[rank];
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int RookAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int bucket, int oppositeSideBucket, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToRookOffset = (int)Piece.R - (int)Piece.P;
        var sameSidePawns = _pieceBitBoards[pieceIndex - pawnToRookOffset];

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var attacks = Attacks.RookAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.R + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        var packedBonus = RookMobilityBonus[mobility];

        // King ring attacks
        var kingRing = KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += RookKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        var fileMask = Masks.FileMask(squareIndex);

        // Rook on open file
        if (((_pieceBitBoards[(int)Piece.P] | _pieceBitBoards[(int)Piece.p]) & fileMask) == default)
        {
            var file = Constants.File[squareIndex];
            packedBonus += OpenFileRookBonus[bucket][file];
            packedBonus += OpenFileRookEnemyBonus[oppositeSideBucket][file];
        }
        // Rook on semi-open file
        else if ((sameSidePawns & fileMask) == default)
        {
            var file = Constants.File[squareIndex];
            packedBonus += SemiOpenFileRookBonus[bucket][file];
            packedBonus += SemiOpenFileRookEnemyBonus[oppositeSideBucket][file];
        }

        // Connected rooks
        if ((attacks & _pieceBitBoards[pieceIndex]).CountBits() >= 1)
        {
            var rank = Constants.Rank[squareIndex];

            if (pieceIndex == (int)Piece.r)
            {
                rank = 7 - rank;
            }

            packedBonus += ConnectedRooksBonus[rank];
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int KnightAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToKnightOffset = (int)Piece.N - (int)Piece.P;
        var sameSidePawns = _pieceBitBoards[pieceIndex - pawnToKnightOffset];

        var attacks = Attacks.KnightAttacks[squareIndex];
        evaluationContext.Attacks[(int)Piece.N + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        var packedBonus = KnightMobilityBonus[mobility];

        // King ring attacks
        var kingRing = KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += KnightKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BishopAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToBishopOffset = (int)Piece.B - (int)Piece.P;
        var sameSidePawns = _pieceBitBoards[pieceIndex - pawnToBishopOffset];

        var offset = Utils.PieceOffset(pieceSide);

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var attacks = Attacks.BishopAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.B + offset] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        var packedBonus = BishopMobilityBonus[mobility];

        // King ring attacks
        var kingRing = KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += BishopKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        // Bad bishop
        var sameColorPawns = sameSidePawns &
            (Constants.DarkSquares[squareIndex] == 1
                ? Constants.DarkSquaresBitBoard
                : Constants.LightSquaresBitBoard);

        // Allowing playing positions with > 8 pawns
        var sameColorPawnsCount = sameColorPawns.CountBits() % 9;

        packedBonus += BadBishop_SameColorPawnsPenalty[sameColorPawnsCount];

        // Blocked central pawns
        var sameSideCentralPawns = sameSidePawns & Constants.CentralFiles;

        var pawnBlockerSquares = pieceSide == (int)Side.White
            ? sameSideCentralPawns.ShiftUp()
            : sameSideCentralPawns.ShiftDown();

        var pawnBlockers = pawnBlockerSquares & _occupancyBitBoards[Utils.OppositeSide(pieceSide)];

        packedBonus += BadBishop_BlockedCentralPawnsPenalty[pawnBlockers.CountBits()];

        // Bishop in unblocked long diagonals
        if ((attacks & Constants.CentralSquares).CountBits() == 2)
        {
            packedBonus += BishopInUnblockedLongDiagonalBonus;
        }

        if (!Configuration.EngineSettings.IsChess960        // Can't happen in standard chess
            || !Constants.Corners.GetBit(squareIndex))      // Saves some checks if no bishop in a corner at all
        {
            return packedBonus;
        }

        // Cornered/trapped bishop
        if (pieceIndex == (int)Piece.B)
        {
            if (squareIndex == (int)BoardSquare.a1 && _board[(int)BoardSquare.b2] == (int)Piece.P)
            {
                if (_board[(int)BoardSquare.b3] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }

            }
            else if (squareIndex == (int)BoardSquare.h1 && _board[(int)BoardSquare.g2] == (int)Piece.P)
            {
                if (_board[(int)BoardSquare.g3] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }
            }
        }
        else
        {

            if (squareIndex == (int)BoardSquare.a8 && _board[(int)BoardSquare.b7] == (int)Piece.p)
            {
                if (_board[(int)BoardSquare.b6] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }
            }
            else if (squareIndex == (int)BoardSquare.h8 && _board[(int)BoardSquare.g7] == (int)Piece.p)
            {
                if (_board[(int)BoardSquare.g6] == (int)Piece.None)
                {
                    packedBonus += BishopCorneredPenalty;
                }
                else
                {
                    packedBonus += BishopCorneredAndBlockedPenalty;
                }
            }
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int QueenAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int pieceIndex, int pieceSide, BitBoard enemyPawnAttacks, int oppositeSideKingSquare)
    {
        const int pawnToQueenOffset = (int)Piece.Q - (int)Piece.P;
        var sameSidePawns = _pieceBitBoards[pieceIndex - pawnToQueenOffset];

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var attacks = Attacks.QueenAttacks(squareIndex, occupancy);
        evaluationContext.Attacks[(int)Piece.Q + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Mobility
        var squaresToExcludeFromMobility = ~(sameSidePawns | enemyPawnAttacks);
        var mobility = (attacks & squaresToExcludeFromMobility).CountBits();
        var packedBonus = QueenMobilityBonus[mobility];

        // King ring attacks
        var kingRing = KingRing[oppositeSideKingSquare];
        var kingRingAttacksCount = (attacks & kingRing).CountBits();
        packedBonus += QueenKingRingAttacksBonus * kingRingAttacksCount;

        evaluationContext.IncreaseKingRingAttacks(pieceSide, kingRingAttacksCount);

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int KingAdditionalEvaluation(ref EvaluationContext evaluationContext, int squareIndex, int bucket, int pieceSide, BitBoard enemyPawnAttacks)
    {
        var attacks = Attacks.KingAttacks[squareIndex];
        evaluationContext.Attacks[(int)Piece.K + Utils.PieceOffset(pieceSide)] |= attacks;
        evaluationContext.AttacksBySide[pieceSide] |= attacks;

        // Virtual mobility (as if Queen)
        var attacksCount =
            (Attacks.QueenAttacks(squareIndex, _occupancyBitBoards[(int)Side.Both])
            & ~(_occupancyBitBoards[pieceSide] | enemyPawnAttacks)).CountBits();
        int packedBonus = VirtualKingMobilityBonus[attacksCount];

        var kingSideOffset = Utils.PieceOffset(pieceSide);

        // Opposite side rooks or queens on the board
        if (_pieceBitBoards[(int)Piece.r - kingSideOffset] + _pieceBitBoards[(int)Piece.q - kingSideOffset] != 0)
        {
            var file = Masks.FileMask(squareIndex);

            // King on open file
            if (((_pieceBitBoards[(int)Piece.P] | _pieceBitBoards[(int)Piece.p]) & file) == 0)
            {
                packedBonus += OpenFileKingPenalty[bucket][Constants.File[squareIndex]];
            }
            // King on semi-open file
            else if ((_pieceBitBoards[(int)Piece.P + kingSideOffset] & file) == 0)
            {
                packedBonus += SemiOpenFileKingPenalty[bucket][Constants.File[squareIndex]];
            }
        }

        // Pawn king shield included next to pawn additional eval

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int KingPawnShield(int squareIndex, BitBoard sameSidePawns)
    {
        var ownPawnsAroundKingCount = (Attacks.KingAttacks[squareIndex] & sameSidePawns).CountBits();

        return ownPawnsAroundKingCount * KingShieldBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PawnIslands(BitBoard whitePawns, BitBoard blackPawns)
    {
        var whiteIslandCount = CountPawnIslands(whitePawns);
        var blackIslandCount = CountPawnIslands(blackPawns);

        return PawnIslandsBonus[whiteIslandCount] - PawnIslandsBonus[blackIslandCount];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CountPawnIslands(BitBoard pawns)
        {
            const int n = 1;
            Span<int> files = stackalloc int[8];

            while (pawns != default)
            {
                pawns = pawns.WithoutLS1B(out var squareIndex);

                files[Constants.File[squareIndex]] = n;
            }

            var islandCount = 0;
            var isIsland = false;

            for (int file = 0; file < files.Length; ++file)
            {
                if (files[file] == n)
                {
                    if (!isIsland)
                    {
                        isIsland = true;
                        ++islandCount;
                    }
                }
                else
                {
                    isIsland = false;
                }
            }

            return islandCount;
        }
    }

    private static readonly int[][] _defendedThreatsBonus =
    [
        [],
        KnightThreatsBonus_Defended,
        BishopThreatsBonus_Defended,
        RookThreatsBonus_Defended,
        QueenThreatsBonus_Defended,
        KingThreatsBonus_Defended
    ];

    private static readonly int[][] _undefendedThreatsBonus =
    [
        [],
        KnightThreatsBonus,
        BishopThreatsBonus,
        RookThreatsBonus,
        QueenThreatsBonus,
        KingThreatsBonus
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CalculateThreats(ref EvaluationContext evaluationContext)
    {
        var occupancy = _occupancyBitBoards[(int)Side.Both];

        ref var attacksRef = ref MemoryMarshal.GetReference(evaluationContext.Attacks);
        ref var attacksBySideRef = ref MemoryMarshal.GetReference(evaluationContext.AttacksBySide);

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref Unsafe.Add(ref attacksRef, pieceIndex);
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            Unsafe.Add(ref attacksBySideRef, (int)Side.White) |= existingAttacks;
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref Unsafe.Add(ref attacksRef, pieceIndex);
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            Unsafe.Add(ref attacksBySideRef, (int)Side.Black) |= existingAttacks;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Threats(EvaluationContext evaluationContext, int oppositeSide)
    {
        var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
        var oppositeSidePieces = _occupancyBitBoards[oppositeSide];
        int packedBonus = 0;

        var attacks = evaluationContext.Attacks;
        var board = _board;
        var defendedThreatsBonus = _defendedThreatsBonus;
        var undefendedThreatsBonus = _undefendedThreatsBonus;

        var defendedSquares = attacks[(int)Piece.P + oppositeSideOffset];

        for (int i = (int)Piece.N; i <= (int)Piece.K; ++i)
        {
            var threats = attacks[6 + i - oppositeSideOffset] & oppositeSidePieces;

            var defended = threats & defendedSquares;
            while (defended != 0)
            {
                defended = defended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                packedBonus += defendedThreatsBonus[i][attackedPiece - oppositeSideOffset];
            }

            var undefended = threats & ~defendedSquares;
            while (undefended != 0)
            {
                undefended = undefended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                packedBonus += undefendedThreatsBonus[i][attackedPiece - oppositeSideOffset];
            }
        }

        return packedBonus;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Checks(EvaluationContext evaluationContext, int side, int oppositeSide)
    {
        int packedBonus = 0;

        var offset = Utils.PieceOffset(side);
        var occupancy = _occupancyBitBoards[(int)Side.Both];

        var oppositeSideKingSquare = _pieceBitBoards[(int)Piece.k - offset].GetLS1BIndex();
        var oppositeSideAttacks = evaluationContext.AttacksBySide[oppositeSide];

        Span<BitBoard> checkThreats = stackalloc BitBoard[5];

        var bishopAttacks = Attacks.BishopAttacks(oppositeSideKingSquare, occupancy);
        var rookAttacks = Attacks.RookAttacks(oppositeSideKingSquare, occupancy);

        checkThreats[(int)Piece.N] = Attacks.KnightAttacks[oppositeSideKingSquare];
        checkThreats[(int)Piece.B] = bishopAttacks;
        checkThreats[(int)Piece.R] = rookAttacks;
        checkThreats[(int)Piece.Q] = Attacks.QueenAttacks(rookAttacks, bishopAttacks);

        ref var attacksRef = ref MemoryMarshal.GetReference(evaluationContext.Attacks);

        for (int piece = (int)Piece.N; piece < (int)Piece.K; ++piece)
        {
            var checks = Unsafe.Add(ref attacksRef, piece + offset) & checkThreats[piece];
            var checksCount = checks.CountBits();

            var unsafeChecksCount = (checks & oppositeSideAttacks).CountBits();
            var safeChecksCount = checksCount - unsafeChecksCount;

            packedBonus += SafeCheckBonus[piece] * safeChecksCount;
            packedBonus += UnsafeCheckBonus[piece] * unsafeChecksCount;
        }

        return packedBonus;
    }

    /// <summary>
    /// Scales <paramref name="eval"/> with <paramref name="movesWithoutCaptureOrPawnMove"/>, so that
    /// an eval with 100 halfmove counter is half of the value of one with 0 halfmove counter
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ScaleEvalWith50MovesDrawDistance(int eval, int movesWithoutCaptureOrPawnMove) =>
    eval * (200 - movesWithoutCaptureOrPawnMove) / 200;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CountPieces() => _pieceBitBoards.Sum(b => b.CountBits());

    /// <summary>
    /// Based on Stormphrax
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int PieceAt(int square)
    {
        var bit = BitBoardExtensions.SquareBit(square);

        Side color;

        if ((_occupancyBitBoards[(int)Side.Black] & bit) != default)
        {
            color = Side.Black;
        }
        else if ((_occupancyBitBoards[(int)Side.White] & bit) != default)
        {
            color = Side.White;
        }
        else
        {
            return (int)Piece.None;
        }

        var offset = Utils.PieceOffset((int)color);

        for (int pieceIndex = offset; pieceIndex < 6 + offset; ++pieceIndex)
        {
            if (!(_pieceBitBoards[pieceIndex] & bit).Empty())
            {
                return pieceIndex;
            }
        }

        Debug.Fail($"Bit set in {_side} occupancy bitboard, but not piece found");

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
            // Shredder-FEN style (always showing columns), no support for X-FEN style yet (showking KQkq when not-ambiguous)
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
    /// Combines <see cref="_pieceBitBoards"/>, <see cref="_side"/>, <see cref="_castle"/> and <see cref="_enPassant"/>
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
            char whitekingSide = '-', whiteQueenside = '-', blackKingside = '-', blackQueenside = '-';

            if ((_castle & (int)CastlingRights.WK) != default)
            {
                whitekingSide = (char)('A' + Constants.File[WhiteShortCastle.TargetSquare()]);
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
                whitekingSide +
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
                ArrayPool<BitBoard>.Shared.Return(_pieceBitBoards, clearArray: true);
                ArrayPool<BitBoard>.Shared.Return(_occupancyBitBoards, clearArray: true);
                ArrayPool<ulong>.Shared.Return(_nonPawnHash, clearArray: true);

                ArrayPool<byte>.Shared.Return(_castlingRightsUpdateConstants, clearArray: true);
                ArrayPool<ulong>.Shared.Return(KingsideCastlingFreeSquares, clearArray: true);
                ArrayPool<ulong>.Shared.Return(QueensideCastlingFreeSquares, clearArray: true);
                ArrayPool<ulong>.Shared.Return(KingsideCastlingNonAttackedSquares, clearArray: true);
                ArrayPool<ulong>.Shared.Return(QueensideCastlingNonAttackedSquares, clearArray: true);

#if DEBUG
                ArrayPool<int>.Shared.Return(_initialKingSquares, clearArray: true);
                ArrayPool<int>.Shared.Return(_initialKingsideRookSquares, clearArray: true);
                ArrayPool<int>.Shared.Return(_initialQueensideRookSquares, clearArray: true);
#endif

                // No need to clear, since we always have to initialize it to Piece.None after renting it anyway
#pragma warning disable S3254 // Default parameter values should not be passed as arguments
                ArrayPool<int>.Shared.Return(_board, clearArray: false);
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
