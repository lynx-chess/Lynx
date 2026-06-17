using Lynx.Model;
using NLog;
using System.Buffers.Binary;
using System.Diagnostics;

namespace Lynx.Datagen;

public static class ViriformatLoader
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

#pragma warning disable MA0008 // Add StructLayoutAttribute
    private record struct Stats
    {
        public ulong GameCount;

        public ulong PositonsCount;

        public ulong FilteredPositionsCount;

        public readonly void Print(string path, double elapsedMilliseconds)
        {
            _logger.Warn("Source file: {Path}", path);
            _logger.Warn("Total games: {GameCount}", GameCount);
            _logger.Warn("Total positions: {PositonsCount}", PositonsCount);
            _logger.Warn("Positions after filtering: {FilteredPositionsCount} ({FilteredPositionsPercentage}%)", FilteredPositionsCount, (100 * FilteredPositionsCount / (double)PositonsCount).ToString("F2"));
            _logger.Warn("Positions/game: {PositonsPerGameCount}", GameCount > 0 ? (ulong)Math.Round(FilteredPositionsCount / (double)GameCount) : 0);
            _logger.Warn("Total time: {Time}", Utils.TimeToString(elapsedMilliseconds));
        }
    }
#pragma warning restore MA0008 // Add StructLayoutAttribute

    public static void LoadFile(string path, ViriformatFilter? filter = null)
    {
        var stats = new Stats();
        var sw = Stopwatch.StartNew();

        try
        {
            using var sourceFile = File.OpenRead(path);
            using var outputFile = new StreamWriter(path + ".epd", append: false);

            var destinationPath = path + ".fen";

            // Reusable buffers to avoid per-game allocations
            byte[] boardBufArr = new byte[32];
            byte[] pairBufArr = new byte[4];
            Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

            Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
            var evalCtx = new EvaluationContext(buffer);

            (string FEN, short eval)[] validPositionsPerGame = new (string FEN, short eval)[Constants.MaxNumberMovesInAGame];

            while (true)
            {
                int firstRead = ReadFull(sourceFile, boardBufArr);
                if (firstRead == 0)
                {
                    break; // no more games
                }

                if (firstRead != boardBufArr.Length)
                {
                    throw new InvalidDataException("Unexpected EOF while reading PackedBoard");
                }

                Array.Clear(validPositionsPerGame);

                var parsedFEN = PackedBoardToFEN(boardBufArr);

                // Parse initial position eval (i16le at bytes 28..29) and WDL (byte at 30)
                var (initialPositionScore, wdlByte) = ParseEvalAndWDL(boardBufArr);

                // Map WDL byte to a human-readable game result. Unknown/default -> "*"
                string gameResult = MapWdlToResult(wdlByte);

                using var game = new Game(parsedFEN);

                var rng = new Random();
                var ply = game.Ply;
                var initialFEN = game.FEN;

                if (initialPositionScore != 0)
                {
                    ++stats.PositonsCount;
                    outputFile.WriteLine($"{initialFEN}; {initialPositionScore}; [{gameResult}]");
                }

                int positionsPerGame = 0;

                while (true)
                {
                    int read = ReadFull(sourceFile, pairBufArr);

                    if (read == 0)
                    {
                        // EOF reached while reading moves — treat as end of last game
                        break;
                    }

                    if (read != 4)
                    {
                        throw new InvalidDataException("Unexpected EOF while reading move+score pair");
                    }

                    ushort rawMove = BinaryPrimitives.ReadUInt16LittleEndian(pairBufArr.AsSpan(0, 2));
                    short eval = BinaryPrimitives.ReadInt16LittleEndian(pairBufArr.AsSpan(2, 2));

                    if (rawMove == 0 && eval == 0)
                    {
                        // end of this game
                        break;
                    }

                    var uci = ViriformatMoveToUci(rawMove);

                    // Reset evaluation context before generating moves (we reuse the same buffer)
                    evalCtx.Reset();
                    var generated = MoveGenerator.GenerateAllMoves(game.CurrentPosition, ref evalCtx, movePool);

                    if (!MoveExtensions.TryParseFromUCIString(uci.AsSpan(), generated, out var move))
                    {
                        _logger.Warn("Unable to parse move {0} in current position", uci);
                        throw new InvalidDataException($"Unable to parse move ({uci}) in current position ({game.FEN})");
                    }

                    var fen = game.FEN;

                    // Apply filter if provided. Filter examines the position before the move (the eval belongs to this position).
                    bool filteredOut = false;
                    if (filter is not null)
                    {
                        filteredOut = filter.ShouldDrop(move!.Value, eval, game.CurrentPosition, wdlByte, ply, rng);
                    }

                    if (!filteredOut)
                    {
                        validPositionsPerGame[positionsPerGame] = (fen, eval);
                        ++positionsPerGame;
                    }

                    game.MakeMove(move!.Value);

                    ++stats.PositonsCount;
                    ++ply;
                }

                var selectedPositionsPerGame = validPositionsPerGame.AsSpan()[..positionsPerGame];

                if (filter?.LimitPositionsPerGame == true && positionsPerGame > filter.MaxPositionsPerGame)
                {
                    Random.Shared.Shuffle(selectedPositionsPerGame);
                    selectedPositionsPerGame = selectedPositionsPerGame[..filter.MaxPositionsPerGame];
                }

                foreach (var (selectedFEN, selectedEval) in selectedPositionsPerGame)
                {
                    outputFile.WriteLine($"{selectedFEN}; {selectedEval}; [{gameResult}]");
                }

                stats.FilteredPositionsCount += (ulong)selectedPositionsPerGame.Length;

                // Empty line between games
                if (Configuration.EngineSettings.Datagen_EmptyLineBetweenGames)
                {
                    outputFile.WriteLine();
                }

                // _logger.Debug("Loaded game {0} from startpos {1} with {2} move scores", gameCount, fen, scores.Count);
                ++stats.GameCount;

                const int SampleRate = 10_000;
                if (stats.GameCount % SampleRate == 0)
                {
                    var ms = sw.ElapsedMilliseconds;
                    _logger.Warn("[{0}s] Loaded {1} games, {2} games/s", ms / 1000, stats.GameCount, 1000 * (ulong)stats.GameCount / (ulong)ms);
                }
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while loading games from {0}", path);
        }

        stats.Print(path, sw.ElapsedMilliseconds);
    }

    private static int ReadFull(Stream fs, byte[] buffer)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int r = fs.Read(buffer, offset, buffer.Length - offset);
            if (r == 0)
            {
                break;
            }

            offset += r;
        }
        return offset;
    }

    internal static (short score, byte wdl) ParseEvalAndWDL(ReadOnlySpan<byte> packed)
    {
        short score = BinaryPrimitives.ReadInt16LittleEndian(packed[28..30]);
        byte wdl = packed[30];

        return (score, wdl);
    }

    // Map viriformat WDL byte to a result string per spec (0=black win,1=draw,2=white win)
    internal static string MapWdlToResult(byte wdlByte)
    {
        return wdlByte switch
        {
            0 => "0.0",
            1 => "0.5",
            2 => "1.0",
            _ => "*",
        };
    }

    // Minimal conversion from PackedBoard bytes -> FEN string. This is a limited port of viriformat.marlinformat.unpack
    private static string PackedBoardToFEN(ReadOnlySpan<byte> packed)
    {
        // Layout per viriformat: occupancy u64le (8), pieces U4Array32 (16), stm_ep_square (1), halfmove (1), fullmove u16le (2), eval i16le (2), wdl (1), extra (1)
        ulong occupancy = BinaryPrimitives.ReadUInt64LittleEndian(packed[..8]);
        ReadOnlySpan<byte> pieces = packed[8..24];

        byte stm_ep = packed[24];
        byte halfmove = packed[25];
        ushort fullmove = BinaryPrimitives.ReadUInt16LittleEndian(packed[26..28]);

        Span<char> sq = stackalloc char[64];
        sq.Fill('.');

        // Track castling info with sentinel -1
        bool seenWhiteKing = false;
        bool seenBlackKing = false;
        int whiteRookQueen0 = -1, whiteRookQueen1 = -1;
        int blackRookQueen0 = -1, blackRookQueen1 = -1;

        int pieceIdx = 0;
        for (int virSq = 0; virSq < 64; ++virSq)
        {
            if (((occupancy >> virSq) & 1UL) == 0) continue;

            int byteIndex = pieceIdx / 2;
            int isHigh = pieceIdx % 2;
            int v = (pieces[byteIndex] >> (isHigh * 4)) & 0xF;
            pieceIdx++;

            int pieceCode = v & 0b0111;
            int colour = (v >> 3) & 1; // 0 white, 1 black

            // Map viriformat square (A1=0..H8=63) to Lynx index (a8=0..h1=63)
            int virRank = virSq / 8;
            int virFile = virSq % 8;
            int lynxIndex = ((7 - virRank) * 8) + virFile;

            char pc = PieceCodeToChar(pieceCode, colour != 0);
            sq[lynxIndex] = pc;

            // Castling detection: UNMOVED_ROOK == 6 in marlin
            const int UNMOVED_ROOK = 6;

            if (pieceCode == UNMOVED_ROOK)
            {
                if (colour == 0)
                {
                    // white: if king already seen on iteration, this rook is kingside, else queenside
                    if (seenWhiteKing)
                    {
                        whiteRookQueen1 = lynxIndex;
                    }
                    else
                    {
                        whiteRookQueen0 = lynxIndex;
                    }
                }
                else
                {
                    if (seenBlackKing)
                    {
                        blackRookQueen1 = lynxIndex;
                    }
                    else
                    {
                        blackRookQueen0 = lynxIndex;
                    }
                }
            }

            // Detect king seen
            if (pc == 'K')
            {
                seenWhiteKing = true;
            }
            if (pc == 'k')
            {
                seenBlackKing = true;
            }
        }

        // Build FEN ranks from a8 to h1; Lynx expects a8 first
        var sb = new System.Text.StringBuilder(80);
        for (int rank = 0; rank < 8; ++rank)
        {
            int empty = 0;
            for (int file = 0; file < 8; ++file)
            {
                int sqIndex = (rank * 8) + file;
                char c = sq[sqIndex];
                if (c == '.')
                {
                    ++empty;
                }
                else
                {
                    if (empty != 0) { sb.Append(empty); empty = 0; }
                    sb.Append(c);
                }
            }
            if (empty != 0) sb.Append(empty);
            if (rank != 7) sb.Append('/');
        }

        // side to move and en-passant: stm_ep's highest bit is turn (1=black)
        bool stmBlack = (stm_ep & 0x80) != 0;
        int ep = stm_ep & 0x7F;
        string side = stmBlack ? "b" : "w";

        string epStr;
        if (ep >= 64)
        {
            epStr = "-";
        }
        else
        {
            int virRank = ep / 8;
            int virFile = ep % 8;
            int lynxEp = ((7 - virRank) * 8) + virFile;
            epStr = lynxEp >= 0 && lynxEp < 64 ? Constants.Coordinates[lynxEp] : "-";
        }

        // Build castling string. Prefer standard KQkq when rooks are on initial squares and kings on initial squares.
        var castlingSb = new System.Text.StringBuilder(4);

        int whiteKingSquare = sq.IndexOf('K');
        bool whiteStandard = whiteKingSquare == Constants.InitialWhiteKingSquare;
        int blackKingSquare = sq.IndexOf('k');
        bool blackStandard = blackKingSquare == Constants.InitialBlackKingSquare;

        // White kingside
        if (whiteRookQueen1 != -1)
        {
            int rs = whiteRookQueen1;
            if (whiteStandard && rs == Constants.InitialWhiteKingsideRookSquare)
            {
                castlingSb.Append('K');
            }
            else
            {
                castlingSb.Append(char.ToUpperInvariant(Constants.Coordinates[rs]![0]));
            }
        }
        // White queenside
        if (whiteRookQueen0 != -1)
        {
            int rs = whiteRookQueen0;
            if (whiteStandard && rs == Constants.InitialWhiteQueensideRookSquare)
            {
                castlingSb.Append('Q');
            }
            else
            {
                castlingSb.Append(char.ToUpperInvariant(Constants.Coordinates[rs]![0]));
            }
        }

        // Black kingside
        if (blackRookQueen1 != -1)
        {
            int rs = blackRookQueen1;
            if (blackStandard && rs == Constants.InitialBlackKingsideRookSquare)
            {
                castlingSb.Append('k');
            }
            else
            {
                castlingSb.Append(char.ToLowerInvariant(Constants.Coordinates[rs]![0]));
            }
        }

        // Black queenside
        if (blackRookQueen0 != -1)
        {
            int rs = blackRookQueen0;
            if (blackStandard && rs == Constants.InitialBlackQueensideRookSquare)
            {
                castlingSb.Append('q');
            }
            else
            {
                castlingSb.Append(char.ToLowerInvariant(Constants.Coordinates[rs]![0]));
            }
        }

        var castling = castlingSb.Length == 0 ? "-" : castlingSb.ToString();

        string fen = $"{sb} {side} {castling} {epStr} {halfmove} {fullmove}";
        return fen;
    }

    private static char PieceCodeToChar(int pieceCode, bool isBlack)
    {
        // viriformat::piece mapping: 0..5 -> Pawn, Knight, Bishop, Rook, Queen, King
        return pieceCode switch
        {
            0 => isBlack ? 'p' : 'P',
            1 => isBlack ? 'n' : 'N',
            2 => isBlack ? 'b' : 'B',
            3 => isBlack ? 'r' : 'R',
            4 => isBlack ? 'q' : 'Q',
            5 => isBlack ? 'k' : 'K',
            6 => isBlack ? 'r' : 'R', // UNMOVED_ROOK encoded as 6 -> rook
            _ => '.',
        };
    }

    private static string ViriformatMoveToUci(ushort raw)
    {
        // Bits: low 6 bits = from, next 6 bits = to, promo bits at 12..13, promo flag at high bits
        int from = raw & 0b11_1111;
        int to = (raw >> 6) & 0b11_1111;
        int promo = (raw >> 12) & 0b11; // 0..3 representing promotion type - 1 in Rust

        // Flags per viriformat: PROMO_FLAG_BITS = 0xC000, EP_FLAG_BITS = 0x4000, CASTLE_FLAG_BITS = 0x8000
        bool isPromo = (raw & 0xC000) == 0xC000;
        bool isEp = (raw & 0x4000) != 0 && (raw & 0x8000) == 0;
        bool isCastle = (raw & 0x8000) != 0 && (raw & 0x4000) == 0;

        // Convert viriformat square indices (A1=0..H8=63) to Lynx indexing (a8=0..h1=63)
        int fromLynx = from ^ 56;
        int toLynx = to ^ 56;
        var fromStr = Constants.Coordinates[fromLynx];
        var toStr = Constants.Coordinates[toLynx];

        if (isPromo)
        {
            // in viriformat promotion mapping, promo = inner-1; inner: 1.. => Queen=4? We map by using order Queen,Rook,Bishop,Knight maybe
            // Rust code: promotion stored as piece_type.inner()-1; piece_type inner: Pawn=0, Knight=1, Bishop=2, Rook=3, Queen=4, King=5
            // So promo 0 => Knight? But promotions exclude Pawn/King per Rust - they use inner()-1. So mapping: 0->Knight,1->Bishop,2->Rook,3->Queen
            char pchar = promo switch { 0 => 'n', 1 => 'b', 2 => 'r', 3 => 'q', _ => 'q' };
            return string.Concat(fromStr, toStr, pchar);
        }

        if (isCastle)
        {
            // Determine side by original 'from' rank (viriformat rank 0 == white)
            int fromVirRank = from / 8;
            bool isWhite = fromVirRank == 0;

            bool kingside = to > from;

#pragma warning disable S3358 // Ternary operators should not be nested
            int target = kingside
                ? (isWhite ? Constants.WhiteKingShortCastleSquare : Constants.BlackKingShortCastleSquare)
                : (isWhite ? Constants.WhiteKingLongCastleSquare : Constants.BlackKingLongCastleSquare);
#pragma warning restore S3358 // Ternary operators should not be nested

            var toCastle = Constants.Coordinates[target];
            return string.Concat(fromStr, toCastle);
        }

        if (isEp)
        {
            // En-passant move: UCI is still from+to (landing square). Make explicit handling for clarity.
            return string.Concat(fromStr, toStr);
        }

        // Normal: UCI is from+to
        return string.Concat(fromStr, toStr);
    }
}
