using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace Lynx.Model;

public enum TablebaseEvaluationCategory : byte
{
    Unknown,
    Draw,
    Win,
    Loss,
    [EnumMember(Value = "cursed-win")]
    CursedWin,
    [EnumMember(Value = "blessed-loss")]
    BlessedLoss,
    [EnumMember(Value = "maybe-win")]
    MaybeWin,
    [EnumMember(Value = "maybe-loss")]
    MaybeLoss
}

public record class TablebaseEvaluation()
{
    public TablebaseEvaluationCategory Category { get; init; }

    [JsonPropertyName("dtm")]
    public int DistanceToMate { get; init; }

    [JsonPropertyName("dtz")]
    public int DistanceToZero { get; init; }

    [JsonPropertyName("insufficient_material")]
    public bool IsInsufficientMaterial { get; init; }

    [JsonPropertyName("checkmate")]
    public bool IsCheckmate { get; init; }

    [JsonPropertyName("stalemate")]
    public bool IsStalemate { get; init; }

    public List<TablebaseEvalMove>? Moves { get; init; }
}

public enum TablebaseMainLineWinner
{
    W,
    B
}

public record class TablebaseEvalMove()
{
    public string? Uci { get; init; }

    public TablebaseEvaluationCategory Category { get; init; }

    [JsonPropertyName("dtm")]
    public int DistanceToMate { get; set; }

    [JsonPropertyName("dtz")]
    public int DistanceToZero { get; init; }
}

public record class TablebaseMainlineMove()
{
    public string? Uci { get; init; }
}

public record class TablebaseMainLine
{
    /// <summary>
    /// Null in case of draw or stalemate detected
    /// </summary>
    public TablebaseMainLineWinner? Winner { get; init; }

    public List<TablebaseMainlineMove>? MainLine { get; init; }

    [JsonPropertyName("dtz")]
    public int DistanceToZero { get; set; }
}
