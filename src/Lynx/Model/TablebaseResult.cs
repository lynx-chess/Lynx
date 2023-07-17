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
    public TablebaseEvaluationCategory Category
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    [JsonPropertyName("dtm")]
    public int? DistanceToMate
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    [JsonPropertyName("dtz")]
    public int? DistanceToZero
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    [JsonPropertyName("insufficient_material")]
    public bool IsInsufficientMaterial
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    [JsonPropertyName("checkmate")]
    public bool IsCheckmate
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    [JsonPropertyName("stalemate")]
    public bool IsStalemate
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public List<TablebaseEvalMove>? Moves
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }
}

public record class TablebaseEvalMove()
{
    public string Uci
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    } = string.Empty;

    public TablebaseEvaluationCategory Category
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    [JsonPropertyName("dtm")]
    public int? DistanceToMate
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    [JsonPropertyName("dtz")]
    public int? DistanceToZero
    {
        get;
#if NET8_0_OR_GREATER
        init;
#else
        set;
#endif
    }
}

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default)] // https://github.com/dotnet/runtime/issues/78602#issuecomment-1322004254
[JsonSerializable(typeof(TablebaseEvaluation))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}