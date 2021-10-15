using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lynx.Model
{
    public class Position
    {
        internal const int CheckMateEvaluation = 1_000_000_000;

        internal const int DepthFactor = 1_000_000;

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

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
                Logger.Error($"Error parsing FEN {fen}");
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

        public Position(Position position, Move move, bool todo) : this(position)
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

            if (move.IsCapture())
            {
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Piece.P + oppositeSideOffset;

                if (move.IsEnPassant())
                {
                    var capturedPawnSquare = Constants.EnPassantCaptureSquares[targetSquare];
                    Debug.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

                    PieceBitBoards[oppositePawnIndex].PopBit(capturedPawnSquare);
                    OccupancyBitBoards[oppositeSide].PopBit(capturedPawnSquare);
                }
                else
                {
                    var limit = (int)Piece.K + oppositeSideOffset;
                    for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                    {
                        if (PieceBitBoards[pieceIndex].GetBit(targetSquare))
                        {
                            PieceBitBoards[pieceIndex].PopBit(targetSquare);
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
                Debug.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

                EnPassant = (BoardSquare)enPassantSquare;
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
            }

            Side = (Side)oppositeSide;
            OccupancyBitBoards[(int)Side.Both] = new BitBoard(OccupancyBitBoards[(int)Side.White].Board | OccupancyBitBoards[(int)Side.Black].Board);

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];
        }

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
                    Debug.Assert(PieceBitBoards[oppositePawnIndex].GetBit(capturedPawnSquare), $"Expected {(Side)oppositeSide} pawn in {capturedPawnSquare}");

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
                Debug.Assert(Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare), $"Unexpected en passant square : {enPassantSquare}");

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
            OccupancyBitBoards[(int)Side.Both] = new BitBoard(OccupancyBitBoards[(int)Side.White].Board | OccupancyBitBoards[(int)Side.Black].Board);

            // Updating castling rights
            Castle &= Constants.CastlingRightsUpdateConstants[sourceSquare];
            Castle &= Constants.CastlingRightsUpdateConstants[targetSquare];

            UniqueIdentifier ^= ZobristTable.CastleHash(Castle);
        }

        /// <summary>
        /// False if any of the kings has been captured, or if the opponent king is in check.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            var kingSquare = PieceBitBoards[(int)Piece.K + Utils.PieceOffset(Side)].GetLS1BIndex();
            var oppositeKingSquare = PieceBitBoards[(int)Piece.K + Utils.PieceOffset((Side)Utils.OppositeSide(Side))].GetLS1BIndex();

            return kingSquare >= 0 && oppositeKingSquare >= 0
                && !Attacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
        }

        /// <summary>
        /// Lightweight version of <see cref="IsValid"/>
        /// False if the opponent king is in check.
        /// This method is meant to be invoked only after <see cref="Position(Position, Move)"/>
        /// </summary>
        /// <returns></returns>
        public bool WasProduceByAValidMove()
        {
            var oppositeKingSquare = PieceBitBoards[(int)Piece.K + Utils.PieceOffset((Side)Utils.OppositeSide(Side))].GetLS1BIndex();

            return oppositeKingSquare >= 0 && !Attacks.IsSquaredAttacked(oppositeKingSquare, Side, PieceBitBoards, OccupancyBitBoards);
        }

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
        /// <returns></returns>
        public int StaticEvaluation(Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove)
        {
            var eval = 0;

            if (positionHistory.Values.Any(val => val >= 3) || movesWithoutCaptureOrPawnMove >= 50)
            {
                return eval;
            }

            for (int pieceIndex = 0; pieceIndex < PieceBitBoards.Length; ++pieceIndex)
            {
                // Bitboard 'copy'. Use long directly to avoid the extra allocations
                var bitboard = PieceBitBoards[pieceIndex].Board;

                while (bitboard != default)
                {
                    var pieceSquareIndex = BitBoard.GetLS1BIndex(bitboard);
                    bitboard = BitBoard.ResetLS1B(bitboard);

                    // Material evaluation
                    eval += EvaluationConstants.MaterialScore[pieceIndex];

                    // Positional evaluation
                    eval += EvaluationConstants.PositionalScore[pieceIndex][pieceSquareIndex];
                }
            }

            return Side == Side.White
                ? eval
                : -eval;
        }

        public IOrderedEnumerable<Move> AllPossibleMoves(int[,]? killerMoves = null, int? plies = null) => MoveGenerator.GenerateAllMoves(this, killerMoves, plies);

        public IOrderedEnumerable<Move> AllCapturesMoves() => MoveGenerator.GenerateAllMoves(this, capturesOnly: true);

        /// <summary>
        /// Assuming a current position has no legal moves (<see cref="AllPossibleMoves"/> doesn't produce any <see cref="IsValid"/> position),
        /// this method determines if a position is a result of either a loss by Checkmate or a draw by stalemate.
        /// NegaMax style
        /// </summary>
        /// <param name="depth">Modulates the output, favouring positions with lower depth left (i.e. Checkmate in less moves)</param>
        /// <returns>At least <see cref="CheckMateEvaluation"/> if Position.Side lost (more extreme values when <paramref name="depth"/> increases)
        /// or 0 if Position.Side was stalemated</returns>
        public int EvaluateFinalPosition(int depth, Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove)
        {
            if (positionHistory.Values.Any(val => val >= 3) || movesWithoutCaptureOrPawnMove >= 50)
            {
                return 0;
            }

            if (Attacks.IsSquaredAttackedBySide(
                PieceBitBoards[(int)Piece.K + Utils.PieceOffset(Side)].GetLS1BIndex(),
                this,
                (Side)Utils.OppositeSide(Side)))
            {
                return -CheckMateEvaluation + (DepthFactor * depth);
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

                    var squareIndex = BitBoard.SquareIndex(rank, file);

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

                    var squareIndex = BitBoard.SquareIndex(rank, file);

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
}
