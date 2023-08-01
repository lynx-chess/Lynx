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
    MaybeLoss,
    Cancelled = byte.MaxValue
}

public record class TablebaseEvaluation()
{
    public TablebaseEvaluationCategory Category { get; init; }

    [JsonPropertyName("dtm")]
    public int? DistanceToMate { get; init; }

    [JsonPropertyName("dtz")]
    public int? DistanceToZero { get; init; }
    [JsonPropertyName("insufficient_material")]
    public bool IsInsufficientMaterial { get; init; }

    [JsonPropertyName("checkmate")]
    public bool IsCheckmate { get; init; }

    [JsonPropertyName("stalemate")]
    public bool IsStalemate { get; init; }

    public List<TablebaseEvalMove>? Moves { get; init; }
}

public record class TablebaseEvalMove()
{
    public string Uci { get; init; } = string.Empty;

    public TablebaseEvaluationCategory Category { get; init; }

    [JsonPropertyName("dtm")]
    public int? DistanceToMate { get; init; }

    [JsonPropertyName("dtz")]
    public int? DistanceToZero { get; init; }
}

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default)] // https://github.com/dotnet/runtime/issues/78602#issuecomment-1322004254
[JsonSerializable(typeof(TablebaseEvaluation))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}