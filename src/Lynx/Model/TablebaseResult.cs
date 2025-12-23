using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace Lynx.Model;

#pragma warning disable S4022 // Enumerations should have "Int32" storage
public enum TablebaseEvaluationCategory : byte
#pragma warning restore S4022 // Enumerations should have "Int32" storage
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

public record class TablebaseEvaluation
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

#pragma warning disable CA1002 // Do not expose generic lists
    public List<TablebaseEvalMove>? Moves { get; init; }
#pragma warning restore CA1002 // Do not expose generic lists
}

public record class TablebaseEvalMove
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
internal sealed partial class SourceGenerationContext : JsonSerializerContext;