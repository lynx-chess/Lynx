using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

using static Lynx.EvaluationConstants;
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

    private readonly ulong[] _pieceBitBoards;
    private readonly ulong[] _occupancyBitBoards;
    private readonly int[] _board;
    private byte _castle;

    private BoardSquare _enPassant;
    private Side _side;

#pragma warning disable RCS1085 // Use auto-implemented property

    public ulong UniqueIdentifier => _uniqueIdentifier;

    public ulong KingPawnUniqueIdentifier => _kingPawnUniqueIdentifier;

    public ulong[] NonPawnHash => _nonPawnHash;

    public ulong MinorHash => _minorHash;

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

    public Position((BitBoard[] _pieceBitBoards, BitBoard[] _occupancyBitBoards, int[] _board, Side Side, byte _castle, BoardSquare _enPassant,
        int _/*, int FullMoveCounter*/) parsedFEN)
    {
        _pieceBitBoards = parsedFEN._pieceBitBoards;
        _occupancyBitBoards = parsedFEN._occupancyBitBoards;
        _board = parsedFEN._board;
        _side = parsedFEN.Side;
        _castle = parsedFEN._castle;
        _enPassant = parsedFEN._enPassant;

#pragma warning disable S3366 // "this" should not be exposed from constructors
        _nonPawnHash = ArrayPool<ulong>.Shared.Rent(2);
        _nonPawnHash[(int)Side.White] = ZobristTable.NonPawnSideHash(this, (int)Side.White);
        _nonPawnHash[(int)Side.Black] = ZobristTable.NonPawnSideHash(this, (int)Side.Black);

        _minorHash = ZobristTable.MinorHash(this);
        _kingPawnUniqueIdentifier = ZobristTable.KingPawnHash(this);

        _uniqueIdentifier = ZobristTable.PositionHash(this, _kingPawnUniqueIdentifier, _nonPawnHash[(int)Side.White], _nonPawnHash[(int)Side.Black]);

        Debug.Assert(_uniqueIdentifier == ZobristTable.PositionHash(this));
#pragma warning restore S3366 // "this" should not be exposed from constructors

        _isIncrementalEval = false;
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

        _nonPawnHash = ArrayPool<ulong>.Shared.Rent(2);
        _nonPawnHash[(int)Side.White] = position._nonPawnHash[(int)Side.White];
        _nonPawnHash[(int)Side.Black] = position._nonPawnHash[(int)Side.Black];

        _pieceBitBoards = ArrayPool<BitBoard>.Shared.Rent(12);
        Array.Copy(position._pieceBitBoards, _pieceBitBoards, position._pieceBitBoards.Length);

        _occupancyBitBoards = ArrayPool<BitBoard>.Shared.Rent(3);
        Array.Copy(position._occupancyBitBoards, _occupancyBitBoards, position._occupancyBitBoards.Length);

        _board = ArrayPool<int>.Shared.Rent(64);
        Array.Copy(position._board, _board, position._board.Length);

        _side = position._side;
        _castle = position._castle;
        _enPassant = position._enPassant;

        _isIncrementalEval = position._isIncrementalEval;
        _incrementalEvalAccumulator = position._incrementalEvalAccumulator;
        _incrementalPhaseAccumulator = position._incrementalPhaseAccumulator;
    }

    #region Move making

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameState MakeMove(Move move)
    {
        Debug.Assert(ZobristTable.PositionHash(this) == _uniqueIdentifier);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.White) == _nonPawnHash[(int)Side.White]);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.Black) == _nonPawnHash[(int)Side.Black]);
        Debug.Assert(ZobristTable.MinorHash(this) == _minorHash);

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
            extraPhaseIfIncremental = TunableEvalParameters.GamePhaseByPiece[promotedPiece]; // - GamePhaseByPiece[piece];
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsMinorPiece(int piece) =>
            piece == (int)Piece.N || piece == (int)Piece.n
            || piece == (int)Piece.B || piece == (int)Piece.b;

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

                if (IsMinorPiece(newPiece))
                {
                    _minorHash ^= targetPieceHash;
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
            else if (IsMinorPiece(piece))
            {
                _minorHash ^= fullPieceMovementHash;
            }
        }

        _enPassant = BoardSquare.noSquare;

        // _incrementalEvalAccumulator updates
        if (_isIncrementalEval)
        {
            _incrementalEvalAccumulator -= PSQT(piece, sourceSquare);
            _incrementalEvalAccumulator += PSQT(newPiece, targetSquare);

            _incrementalPhaseAccumulator += extraPhaseIfIncremental;

            switch (move.SpecialMoveFlag())
            {
                case SpecialMoveType.None:
                    {
                        if (move.IsCapture())
                        {
                            var capturedSquare = targetSquare;
                            var capturedPiece = move.CapturedPiece();

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

                                if (IsMinorPiece(capturedPiece))
                                {
                                    _minorHash ^= capturedPieceHash;
                                }
                            }

                            _incrementalEvalAccumulator -= PSQT(capturedPiece, capturedSquare);

                            _incrementalPhaseAccumulator -= TunableEvalParameters.GamePhaseByPiece[capturedPiece];
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
                        var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                        _occupancyBitBoards[oldSide].PopBit(rookSourceSquare);
                        _board[rookSourceSquare] = (int)Piece.None;

                        _pieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                        _occupancyBitBoards[oldSide].SetBit(rookTargetSquare);
                        _board[rookTargetSquare] = rookIndex;

                        var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        _uniqueIdentifier ^= hashChange;
                        _nonPawnHash[oldSide] ^= hashChange;

                        _incrementalEvalAccumulator -= PSQT(rookIndex, rookSourceSquare);
                        _incrementalEvalAccumulator += PSQT(rookIndex, rookTargetSquare);

                        break;
                    }
                case SpecialMoveType.LongCastle:
                    {
                        var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                        _occupancyBitBoards[oldSide].PopBit(rookSourceSquare);
                        _board[rookSourceSquare] = (int)Piece.None;

                        _pieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                        _occupancyBitBoards[oldSide].SetBit(rookTargetSquare);
                        _board[rookTargetSquare] = rookIndex;

                        var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        _uniqueIdentifier ^= hashChange;
                        _nonPawnHash[oldSide] ^= hashChange;

                        _incrementalEvalAccumulator -= PSQT(rookIndex, rookSourceSquare);
                        _incrementalEvalAccumulator += PSQT(rookIndex, rookTargetSquare);

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

                        _incrementalEvalAccumulator -= PSQT(capturedPiece, capturedSquare);

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
                        if (move.IsCapture())
                        {
                            var capturedSquare = targetSquare;
                            var capturedPiece = move.CapturedPiece();

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

                                if (IsMinorPiece(capturedPiece))
                                {
                                    _minorHash ^= capturedPieceHash;
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
                        var rookSourceSquare = Utils.ShortCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.ShortCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                        _occupancyBitBoards[oldSide].PopBit(rookSourceSquare);
                        _board[rookSourceSquare] = (int)Piece.None;

                        _pieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                        _occupancyBitBoards[oldSide].SetBit(rookTargetSquare);
                        _board[rookTargetSquare] = rookIndex;

                        var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        _uniqueIdentifier ^= hashChange;
                        _nonPawnHash[oldSide] ^= hashChange;

                        break;
                    }
                case SpecialMoveType.LongCastle:
                    {
                        var rookSourceSquare = Utils.LongCastleRookSourceSquare(oldSide);
                        var rookTargetSquare = Utils.LongCastleRookTargetSquare(oldSide);
                        var rookIndex = (int)Piece.R + offset;

                        _pieceBitBoards[rookIndex].PopBit(rookSourceSquare);
                        _occupancyBitBoards[oldSide].PopBit(rookSourceSquare);
                        _board[rookSourceSquare] = (int)Piece.None;

                        _pieceBitBoards[rookIndex].SetBit(rookTargetSquare);
                        _occupancyBitBoards[oldSide].SetBit(rookTargetSquare);
                        _board[rookTargetSquare] = rookIndex;

                        var hashChange = ZobristTable.PieceHash(rookSourceSquare, rookIndex)
                            ^ ZobristTable.PieceHash(rookTargetSquare, rookIndex);

                        _uniqueIdentifier ^= hashChange;
                        _nonPawnHash[oldSide] ^= hashChange;

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
        _castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        _castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

        _uniqueIdentifier ^= ZobristTable.CastleHash(_castle);

        Debug.Assert(ZobristTable.PositionHash(this) == _uniqueIdentifier);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.White) == _nonPawnHash[(int)Side.White]);
        Debug.Assert(ZobristTable.NonPawnSideHash(this, (int)Side.Black) == _nonPawnHash[(int)Side.Black]);
        Debug.Assert(ZobristTable.MinorHash(this) == _minorHash);

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

        _pieceBitBoards[piece].SetBit(sourceSquare);
        _occupancyBitBoards[side].SetBit(sourceSquare);
        _board[sourceSquare] = piece;

        switch (move.SpecialMoveFlag())
        {
            case SpecialMoveType.None:
                {
                    if (move.IsCapture())
                    {
                        var capturedPiece = move.CapturedPiece();

                        _pieceBitBoards[capturedPiece].SetBit(targetSquare);
                        _occupancyBitBoards[oppositeSide].SetBit(targetSquare);
                        _board[targetSquare] = capturedPiece;
                    }

                    break;
                }
            case SpecialMoveType.ShortCastle:
                {
                    var rookSourceSquare = Utils.ShortCastleRookSourceSquare(side);
                    var rookTargetSquare = Utils.ShortCastleRookTargetSquare(side);
                    var rookIndex = (int)Piece.R + offset;

                    _pieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                    _occupancyBitBoards[side].SetBit(rookSourceSquare);
                    _board[rookSourceSquare] = rookIndex;

                    _pieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                    _occupancyBitBoards[side].PopBit(rookTargetSquare);
                    _board[rookTargetSquare] = (int)Piece.None;

                    break;
                }
            case SpecialMoveType.LongCastle:
                {
                    var rookSourceSquare = Utils.LongCastleRookSourceSquare(side);
                    var rookTargetSquare = Utils.LongCastleRookTargetSquare(side);
                    var rookIndex = (int)Piece.R + offset;

                    _pieceBitBoards[rookIndex].SetBit(rookSourceSquare);
                    _occupancyBitBoards[side].SetBit(rookSourceSquare);
                    _board[rookSourceSquare] = rookIndex;

                    _pieceBitBoards[rookIndex].PopBit(rookTargetSquare);
                    _occupancyBitBoards[side].PopBit(rookTargetSquare);
                    _board[rookTargetSquare] = (int)Piece.None;

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

        _occupancyBitBoards[2] = _occupancyBitBoards[1] | _occupancyBitBoards[0];

        // Updating saved values
        _castle = gameState.Castle;
        _enPassant = gameState.EnPassant;

        _uniqueIdentifier = gameState.ZobristKey;
        _kingPawnUniqueIdentifier = gameState.KingPawnKey;
        _minorHash = gameState.MinorKey;
        _nonPawnHash[(int)Side.White] = gameState.NonPawnWhiteKey;
        _nonPawnHash[(int)Side.Black] = gameState.NonPawnBlackKey;

        _incrementalEvalAccumulator = gameState.IncrementalEvalAccumulator;
        _incrementalPhaseAccumulator = gameState.IncrementalPhaseAccumulator;
        _isIncrementalEval = gameState.IsIncrementalEval;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullMoveGameState MakeNullMove()
    {
        var gameState = new NullMoveGameState(this);

        _uniqueIdentifier ^=
            ZobristTable.SideHash()
            ^ ZobristTable.EnPassantHash((int)_enPassant);

        _side = (Side)Utils.OppositeSide(_side);
        _enPassant = BoardSquare.noSquare;

        return gameState;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnMakeNullMove(NullMoveGameState gameState)
    {
        _side = (Side)Utils.OppositeSide(_side);
        _enPassant = gameState.EnPassant;
        _uniqueIdentifier = gameState.ZobristKey;
    }

    /// <summary>
    /// False if any of the kings has been captured, or if the opponent king is in check.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsValid()
    {
        var offset = Utils.PieceOffset(_side);

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
        Debug.Assert(_pieceBitBoards[(int)Piece.k - Utils.PieceOffset(_side)].CountBits() == 1);
        var oppositeKingSquare = _pieceBitBoards[(int)Piece.k - Utils.PieceOffset(_side)].GetLS1BIndex();

        return !IsSquareAttacked(oppositeKingSquare, _side);
    }

    #endregion

    #region Evaluation

    /// <summary>
    /// Evaluates material and position in a NegaMax style.
    /// That is, positive scores always favour playing <see cref="_side"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Score, int Phase) StaticEvaluation()
    {
        int packedScore = 0;
        int gamePhase = 0;

        if (_isIncrementalEval)
        {
            packedScore = _incrementalEvalAccumulator;
            gamePhase = _incrementalPhaseAccumulator;
        }
        else
        {
            _incrementalEvalAccumulator = 0;
            _incrementalPhaseAccumulator = 0;

            for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
            {
                // Bitboard copy that we 'empty'
                var bitboard = _pieceBitBoards[pieceIndex];

                while (bitboard != default)
                {
                    bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                    _incrementalEvalAccumulator += PSQT(pieceIndex, pieceSquareIndex);

                    _incrementalPhaseAccumulator += TunableEvalParameters.GamePhaseByPiece[pieceIndex];
                }
            }

            packedScore += _incrementalEvalAccumulator;
            gamePhase += _incrementalPhaseAccumulator;
            _isIncrementalEval = true;
        }

        if (gamePhase > MaxPhase)    // Early promotions
        {
            gamePhase = MaxPhase;
        }

        int endGamePhase = MaxPhase - gamePhase;

        var middleGameScore = Utils.UnpackMG(packedScore);
        var endGameScore = Utils.UnpackEG(packedScore);
        var eval = ((middleGameScore * gamePhase) + (endGameScore * endGamePhase)) / MaxPhase;

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
             ((_pieceBitBoards[(int)Piece.N] | _pieceBitBoards[(int)Piece.n]).CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.N])
            + ((_pieceBitBoards[(int)Piece.B] | _pieceBitBoards[(int)Piece.b]).CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.B])
            + ((_pieceBitBoards[(int)Piece.R] | _pieceBitBoards[(int)Piece.r]).CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.R])
            + ((_pieceBitBoards[(int)Piece.Q] | _pieceBitBoards[(int)Piece.q]).CountBits() * TunableEvalParameters.GamePhaseByPiece[(int)Piece.Q]);

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
    public bool IsSquareAttackedBySide(int squaredIndex, Side sideToMove) => IsSquareAttacked(squaredIndex, sideToMove);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttacked(int squareIndex, Side sideToMove)
    {
        Utils.Assert(sideToMove != Side.Both);

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
    public bool IsInCheck()
    {
        var oppositeSideInt = Utils.OppositeSide(_side);
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

        var offset = Utils.PieceOffset(color);

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
        var length = sb.Length;

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

        if (sb.Length == length)
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

                for (int bbIndex = 0; bbIndex < _pieceBitBoards.Length; ++bbIndex)
                {
                    if (_pieceBitBoards[bbIndex].GetBit(squareIndex))
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
        Console.WriteLine($"    Side:\t{_side}");
        Console.WriteLine($"    Enpassant:\t{(_enPassant == BoardSquare.noSquare ? "no" : Constants.Coordinates[(int)_enPassant])}");
        Console.WriteLine($"    Castling:\t" +
            $"{((_castle & (int)CastlingRights.WK) != default ? 'K' : '-')}" +
            $"{((_castle & (int)CastlingRights.WQ) != default ? 'Q' : '-')} | " +
            $"{((_castle & (int)CastlingRights.BK) != default ? 'k' : '-')}" +
            $"{((_castle & (int)CastlingRights.BQ) != default ? 'q' : '-')}"
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

#pragma warning restore S106, S2228 // Standard outputs should not be used directly to log anything

    public void FreeResources()
    {
        ArrayPool<BitBoard>.Shared.Return(_pieceBitBoards, clearArray: true);
        ArrayPool<BitBoard>.Shared.Return(_occupancyBitBoards, clearArray: true);
        ArrayPool<ulong>.Shared.Return(_nonPawnHash, clearArray: true);

        // No need to clear, since we always have to initialize it to Piece.None after renting it anyway
#pragma warning disable S3254 // Default parameter values should not be passed as arguments
        ArrayPool<int>.Shared.Return(_board, clearArray: false);
#pragma warning restore S3254 // Default parameter values should not be passed as arguments

        _disposedValue = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                FreeResources();
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
