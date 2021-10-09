using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lynx.Benchmark
{
    public class PositionIdGeneration : BaseBenchmark
    {
        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public string Struct_FENCalculatedOnTheFly(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new StructCustomPosition(fen);
            var newPosition = new StructCustomPosition(position, moves[0]);
            return newPosition.FEN;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string Struct_FENCalculatedWithinTheMoveConstructor(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new StructCustomPosition(fen);
            var newPosition = new StructCustomPosition(position, moves[0], default);

            return newPosition.FEN;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string ReadonlyStruct_FENCalculatedOnTheFly(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new ReadonlyStructCustomPosition(fen);
            var newPosition = new ReadonlyStructCustomPosition(position, moves[0]);

            return newPosition.FEN;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string ReadonlyStruct_FENCalculatedWithinTheMoveConstructor(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new ReadonlyStructCustomPosition(fen);
            var newPosition = new ReadonlyStructCustomPosition(position, moves[0], default);

            return newPosition.FEN;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string Class_FENCalculatedOnTheFly(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new ClassCustomPosition(fen);
            var newPosition = new ClassCustomPosition(position, moves[0]);

            return newPosition.FEN;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string Class_FENCalculatedWithinTheMoveConstructor(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new ClassCustomPosition(fen);
            var newPosition = new ClassCustomPosition(position, moves[0], default);

            return newPosition.FEN;
        }
        
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string RecordClass_FENCalculatedOnTheFly(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new RecordClassCustomPosition(fen);
            var newPosition = new RecordClassCustomPosition(position, moves[0]);

            return newPosition.FEN;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string RecordClass_FENCalculatedWithinTheMoveConstructor(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new RecordClassCustomPosition(fen);
            var newPosition = new RecordClassCustomPosition(position, moves[0], default);

            return newPosition.FEN;
        }
        
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string RecordStruct_FENCalculatedOnTheFly(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new RecordStructCustomPosition(fen);
            var newPosition = new RecordStructCustomPosition(position, moves[0]);

            return newPosition.FEN;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public string RecordStruct_FENCalculatedWithinTheMoveConstructor(string fen)
        {
            var moves = new Position(fen).AllPossibleMoves();

            var position = new RecordStructCustomPosition(fen);
            var newPosition = new RecordStructCustomPosition(position, moves[0], default);

            return newPosition.FEN;
        }

        public static IEnumerable<string> Data => new[] {
            //Constants.EmptyBoardFEN,
            Constants.InitialPositionFEN,
            Constants.TrickyTestPositionFEN,
            "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1",
            "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1",
            "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 1"
        };
    }

    struct StructCustomPosition
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private string? _fen;

        public string FEN
        {
            get => _fen ??= CalculateFEN();
            init => _fen = value;
        }

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

        public StructCustomPosition(string fen)
        {
            _fen = fen;

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
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="position"></param>
        public StructCustomPosition(StructCustomPosition position)
        {
            _fen = position.FEN;

            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = position.Side;
            Castle = position.Castle;
            EnPassant = position.EnPassant;
        }

        public StructCustomPosition(StructCustomPosition position, Move move) : this(position)
        {
            _fen = null;
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

        /// <summary>
        /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
        /// </summary>
        /// <param name="position"></param>
        /// <param name="move"></param>
        /// <param name="calculateFen"></param>
        public StructCustomPosition(StructCustomPosition position, Move move, bool calculateFen) : this(position)
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

            StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

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

            _fen = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
        }

        private readonly string CalculateFEN()
        {
            var sb = new StringBuilder(100);

            var squaresPerRow = 0;

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

                squaresPerRow = (squaresPerRow + 1) % 8;
                if (squaresPerRow == 0)
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
    }

    readonly struct ReadonlyStructCustomPosition
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public string FEN { get; private init; }

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

        public ReadonlyStructCustomPosition(string fen)
        {
            FEN = fen;

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
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="position"></param>
        public ReadonlyStructCustomPosition(ReadonlyStructCustomPosition position)
        {
            FEN = position.FEN;

            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = position.Side;
            Castle = position.Castle;
            EnPassant = position.EnPassant;
        }

        public ReadonlyStructCustomPosition(ReadonlyStructCustomPosition position, Move move) : this(position)
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

            FEN = CalculateFEN();
        }

        /// <summary>
        /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
        /// </summary>
        /// <param name="position"></param>
        /// <param name="move"></param>
        /// <param name="calculateFen"></param>
        public ReadonlyStructCustomPosition(ReadonlyStructCustomPosition position, Move move, bool calculateFen) : this(position)
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

            StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

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

            FEN = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
        }

        private readonly string CalculateFEN()
        {
            var sb = new StringBuilder(100);

            var squaresPerRow = 0;

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

                squaresPerRow = (squaresPerRow + 1) % 8;
                if (squaresPerRow == 0)
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
    }

    class ClassCustomPosition
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private string? _fen;

        public string FEN
        {
            get => _fen ??= CalculateFEN();
            init => _fen = value;
        }

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

        public ClassCustomPosition(string fen)
        {
            _fen = fen;

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
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="position"></param>
        public ClassCustomPosition(ClassCustomPosition position)
        {
            _fen = position.FEN;

            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = position.Side;
            Castle = position.Castle;
            EnPassant = position.EnPassant;
        }

        public ClassCustomPosition(ClassCustomPosition position, Move move) : this(position)
        {
            _fen = null;
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

        /// <summary>
        /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
        /// </summary>
        /// <param name="position"></param>
        /// <param name="move"></param>
        /// <param name="calculateFen"></param>
        public ClassCustomPosition(ClassCustomPosition position, Move move, bool calculateFen) : this(position)
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

            StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

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

            _fen = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
        }

        private string CalculateFEN()
        {
            var sb = new StringBuilder(100);

            var squaresPerRow = 0;

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

                squaresPerRow = (squaresPerRow + 1) % 8;
                if (squaresPerRow == 0)
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
    }

    record class RecordClassCustomPosition
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private string? _fen;

        public string FEN
        {
            get => _fen ??= CalculateFEN();
            init => _fen = value;
        }

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

        public RecordClassCustomPosition(string fen)
        {
            _fen = fen;

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
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="position"></param>
        public RecordClassCustomPosition(RecordClassCustomPosition position)
        {
            _fen = position.FEN;

            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = position.Side;
            Castle = position.Castle;
            EnPassant = position.EnPassant;
        }

        public RecordClassCustomPosition(RecordClassCustomPosition position, Move move) : this(position)
        {
            _fen = null;
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

        /// <summary>
        /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
        /// </summary>
        /// <param name="position"></param>
        /// <param name="move"></param>
        /// <param name="calculateFen"></param>
        public RecordClassCustomPosition(RecordClassCustomPosition position, Move move, bool calculateFen) : this(position)
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

            StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

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

            _fen = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
        }

        private string CalculateFEN()
        {
            var sb = new StringBuilder(100);

            var squaresPerRow = 0;

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

                squaresPerRow = (squaresPerRow + 1) % 8;
                if (squaresPerRow == 0)
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
    }

    record struct RecordStructCustomPosition
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private string? _fen;

        public string FEN
        {
            get => _fen ??= CalculateFEN();
            init => _fen = value;
        }

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

        public RecordStructCustomPosition(string fen)
        {
            _fen = fen;

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
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="position"></param>
        public RecordStructCustomPosition(RecordStructCustomPosition position)
        {
            _fen = position.FEN;

            PieceBitBoards = new BitBoard[12];
            Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

            OccupancyBitBoards = new BitBoard[3];
            Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

            Side = position.Side;
            Castle = position.Castle;
            EnPassant = position.EnPassant;
        }

        public RecordStructCustomPosition(RecordStructCustomPosition position, Move move) : this(position)
        {
            _fen = null;
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

        /// <summary>
        /// https://arxiv.org/ftp/arxiv/papers/2009/2009.03193.pdf
        /// </summary>
        /// <param name="position"></param>
        /// <param name="move"></param>
        /// <param name="calculateFen"></param>
        public RecordStructCustomPosition(RecordStructCustomPosition position, Move move, bool calculateFen) : this(position)
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

            StringBuilder fenSb = FENHelpers.UpdateFirstPartOfFEN(position, sourceSquare, targetSquare, piece);

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

            _fen = FENHelpers.UpdateSecondPartOfFEN(fenSb, Side, Castle, EnPassant);
        }

        private string CalculateFEN()
        {
            var sb = new StringBuilder(100);

            var squaresPerRow = 0;

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

                squaresPerRow = (squaresPerRow + 1) % 8;
                if (squaresPerRow == 0)
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
    }

    static class FENHelpers
    {
        public static StringBuilder UpdateFirstPartOfFEN(StructCustomPosition position, int sourceSquare, int targetSquare, int piece)
        {
            var fenSegments = position.FEN.Split('/');

            int sourceSegmentIndex = sourceSquare / 8;
            int sourceFile = sourceSquare % 8;
            var expandedSourceSegment = new StringBuilder();

            foreach (var item in fenSegments[sourceSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedSourceSegment.Append('1', item - '0');
                }
                else
                {
                    expandedSourceSegment.Append(item);
                }
            }

            expandedSourceSegment[sourceFile] = '1';

            var sourceSegment = new StringBuilder(8);
            int ones = 0;
            foreach (var item in expandedSourceSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        sourceSegment.Append(ones);
                        ones = 0;

                        sourceSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                sourceSegment.Append(ones);
            }

            fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

            int targetSegmentIndex = targetSquare / 8;
            int targetFile = targetSquare % 8;
            var expandedTargetSegment = new StringBuilder(8);
            foreach (var item in fenSegments[targetSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedTargetSegment.Append('1', item - '0');
                }
                else
                {
                    expandedTargetSegment.Append(item);
                }
            }

            expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

            var targetSegment = new StringBuilder();
            ones = 0;
            foreach (var item in expandedTargetSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        targetSegment.Append(ones);
                        ones = 0;

                        targetSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                targetSegment.Append(ones);
            }

            fenSegments[targetSegmentIndex] = targetSegment.ToString();

            fenSegments[7] = fenSegments[7].Split(' ')[0];

            var fenSb = new StringBuilder(string.Join('/', fenSegments));
            return fenSb;
        }

        public static StringBuilder UpdateFirstPartOfFEN(ReadonlyStructCustomPosition position, int sourceSquare, int targetSquare, int piece)
        {
            var fenSegments = position.FEN.Split('/');

            int sourceSegmentIndex = sourceSquare / 8;
            int sourceFile = sourceSquare % 8;
            var expandedSourceSegment = new StringBuilder();

            foreach (var item in fenSegments[sourceSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedSourceSegment.Append('1', item - '0');
                }
                else
                {
                    expandedSourceSegment.Append(item);
                }
            }

            expandedSourceSegment[sourceFile] = '1';

            var sourceSegment = new StringBuilder(8);
            int ones = 0;
            foreach (var item in expandedSourceSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        sourceSegment.Append(ones);
                        ones = 0;

                        sourceSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                sourceSegment.Append(ones);
            }

            fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

            int targetSegmentIndex = targetSquare / 8;
            int targetFile = targetSquare % 8;
            var expandedTargetSegment = new StringBuilder(8);
            foreach (var item in fenSegments[targetSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedTargetSegment.Append('1', item - '0');
                }
                else
                {
                    expandedTargetSegment.Append(item);
                }
            }

            expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

            var targetSegment = new StringBuilder();
            ones = 0;
            foreach (var item in expandedTargetSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        targetSegment.Append(ones);
                        ones = 0;

                        targetSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                targetSegment.Append(ones);
            }

            fenSegments[targetSegmentIndex] = targetSegment.ToString();

            fenSegments[7] = fenSegments[7].Split(' ')[0];

            var fenSb = new StringBuilder(string.Join('/', fenSegments));
            return fenSb;
        }

        public static StringBuilder UpdateFirstPartOfFEN(ClassCustomPosition position, int sourceSquare, int targetSquare, int piece)
        {
            var fenSegments = position.FEN.Split('/');

            int sourceSegmentIndex = sourceSquare / 8;
            int sourceFile = sourceSquare % 8;
            var expandedSourceSegment = new StringBuilder();

            foreach (var item in fenSegments[sourceSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedSourceSegment.Append('1', item - '0');
                }
                else
                {
                    expandedSourceSegment.Append(item);
                }
            }

            expandedSourceSegment[sourceFile] = '1';

            var sourceSegment = new StringBuilder(8);
            int ones = 0;
            foreach (var item in expandedSourceSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        sourceSegment.Append(ones);
                        ones = 0;

                        sourceSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                sourceSegment.Append(ones);
            }

            fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

            int targetSegmentIndex = targetSquare / 8;
            int targetFile = targetSquare % 8;
            var expandedTargetSegment = new StringBuilder(8);
            foreach (var item in fenSegments[targetSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedTargetSegment.Append('1', item - '0');
                }
                else
                {
                    expandedTargetSegment.Append(item);
                }
            }

            expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

            var targetSegment = new StringBuilder();
            ones = 0;
            foreach (var item in expandedTargetSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        targetSegment.Append(ones);
                        ones = 0;

                        targetSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                targetSegment.Append(ones);
            }

            fenSegments[targetSegmentIndex] = targetSegment.ToString();

            fenSegments[7] = fenSegments[7].Split(' ')[0];

            var fenSb = new StringBuilder(string.Join('/', fenSegments));
            return fenSb;
        }

        public static StringBuilder UpdateFirstPartOfFEN(RecordClassCustomPosition position, int sourceSquare, int targetSquare, int piece)
        {
            var fenSegments = position.FEN.Split('/');

            int sourceSegmentIndex = sourceSquare / 8;
            int sourceFile = sourceSquare % 8;
            var expandedSourceSegment = new StringBuilder();

            foreach (var item in fenSegments[sourceSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedSourceSegment.Append('1', item - '0');
                }
                else
                {
                    expandedSourceSegment.Append(item);
                }
            }

            expandedSourceSegment[sourceFile] = '1';

            var sourceSegment = new StringBuilder(8);
            int ones = 0;
            foreach (var item in expandedSourceSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        sourceSegment.Append(ones);
                        ones = 0;

                        sourceSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                sourceSegment.Append(ones);
            }

            fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

            int targetSegmentIndex = targetSquare / 8;
            int targetFile = targetSquare % 8;
            var expandedTargetSegment = new StringBuilder(8);
            foreach (var item in fenSegments[targetSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedTargetSegment.Append('1', item - '0');
                }
                else
                {
                    expandedTargetSegment.Append(item);
                }
            }

            expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

            var targetSegment = new StringBuilder();
            ones = 0;
            foreach (var item in expandedTargetSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        targetSegment.Append(ones);
                        ones = 0;

                        targetSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                targetSegment.Append(ones);
            }

            fenSegments[targetSegmentIndex] = targetSegment.ToString();

            fenSegments[7] = fenSegments[7].Split(' ')[0];

            var fenSb = new StringBuilder(string.Join('/', fenSegments));
            return fenSb;
        }

        public static StringBuilder UpdateFirstPartOfFEN(RecordStructCustomPosition position, int sourceSquare, int targetSquare, int piece)
        {
            var fenSegments = position.FEN.Split('/');

            int sourceSegmentIndex = sourceSquare / 8;
            int sourceFile = sourceSquare % 8;
            var expandedSourceSegment = new StringBuilder();

            foreach (var item in fenSegments[sourceSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedSourceSegment.Append('1', item - '0');
                }
                else
                {
                    expandedSourceSegment.Append(item);
                }
            }

            expandedSourceSegment[sourceFile] = '1';

            var sourceSegment = new StringBuilder(8);
            int ones = 0;
            foreach (var item in expandedSourceSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        sourceSegment.Append(ones);
                        ones = 0;

                        sourceSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                sourceSegment.Append(ones);
            }

            fenSegments[sourceSegmentIndex] = sourceSegment.ToString();

            int targetSegmentIndex = targetSquare / 8;
            int targetFile = targetSquare % 8;
            var expandedTargetSegment = new StringBuilder(8);
            foreach (var item in fenSegments[targetSegmentIndex])
            {
                if (char.IsDigit(item))
                {
                    expandedTargetSegment.Append('1', item - '0');
                }
                else
                {
                    expandedTargetSegment.Append(item);
                }
            }

            expandedTargetSegment[targetFile] = Constants.AsciiPieces[piece];

            var targetSegment = new StringBuilder();
            ones = 0;
            foreach (var item in expandedTargetSegment.ToString())
            {
                if (item == '1')
                {
                    ++ones;
                }
                else
                {
                    if (ones != 0)
                    {
                        targetSegment.Append(ones);
                        ones = 0;

                        targetSegment.Append(item);
                    }
                }
            }

            if (ones != 0)
            {
                targetSegment.Append(ones);
            }

            fenSegments[targetSegmentIndex] = targetSegment.ToString();

            fenSegments[7] = fenSegments[7].Split(' ')[0];

            var fenSb = new StringBuilder(string.Join('/', fenSegments));
            return fenSb;
        }

        /// <summary>
        /// Neeeds to be invoked with the updated <paramref name="side"/>, <paramref name="castle"/> and <paramref name="enPassant"/> properties for the new position
        /// </summary>
        /// <param name="fenSb">Result of <see cref="UpdateFirstPartOfFEN(Position, int, int, int)"/></param>
        /// <param name="side">Update <see cref="Position.Side"/></param>
        /// <param name="castle">Updated <see cref="Position.Castle"/></param>
        /// <param name="enPassant">Updated <see cref="Position.EnPassant"/></param>
        /// <returns></returns>
        public static string UpdateSecondPartOfFEN(StringBuilder fenSb, Side side, int castle, BoardSquare enPassant)
        {
            fenSb.Append(' ');
            fenSb.Append(side == Side.White ? 'w' : 'b');

            fenSb.Append(' ');
            var length = fenSb.Length;

            if ((castle & (int)CastlingRights.WK) != default)
            {
                fenSb.Append('K');
            }
            if ((castle & (int)CastlingRights.WQ) != default)
            {
                fenSb.Append('Q');
            }
            if ((castle & (int)CastlingRights.BK) != default)
            {
                fenSb.Append('k');
            }
            if ((castle & (int)CastlingRights.BQ) != default)
            {
                fenSb.Append('q');
            }

            if (fenSb.Length == length)
            {
                fenSb.Append('-');
            }

            fenSb.Append(' ');

            fenSb.Append(enPassant == BoardSquare.noSquare ? "-" : Constants.Coordinates[(int)enPassant]);

            fenSb.Append(" 0 1");

            return fenSb.ToString();
        }
    }
}
