//using Lynx.Model;

//namespace Lynx;

//public partial class Engine
//{
//    private static readonly IMovegenStage[] _stages =
//    [
//        new TTStage(),

//        new OtherStage(),
//    ];

//    public IEnumerable<(Move move, int score)> GenerateMoves(ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply)
//    {
//        foreach (var stage in _stages)
//        {
//            foreach (var move in stage.GenerateMoves(ttMove, position, oppositeSideAttacks, ply))
//            {
//                var score = ScoreMove(position, move, ply, oppositeSideAttacks, ttMove);

//                yield return (move, score);
//            }
//        }
//    }

//    private interface IMovegenStage
//    {
//        Move[] GenerateMoves(ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply);
//    }

//    private sealed class TTStage : IMovegenStage
//    {
//        public Move[] GenerateMoves(ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply)
//        {
//            var fullTTMove = MoveGenerator.GenerateFullTTMove(ttMove, position, oppositeSideAttacks);

//            if (fullTTMove != 0)
//            {
//                return [fullTTMove];
//            }

//            return [];
//        }
//    }

//    private sealed class OtherStage : IMovegenStage
//    {
//        public Move[] GenerateMoves(ShortMove ttMove, Position position, Bitboard oppositeSideAttacks, int ply)
//        {
//            Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
//            var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves, oppositeSideAttacks);

//            return pseudoLegalMoves.ToArray();
//        }
//    }
//}
