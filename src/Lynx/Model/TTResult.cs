namespace Lynx.Model;

public record struct TTResult(int Score, short BestMove, NodeType NodeType, int StaticEval, int Depth, bool WasPv);