using Lynx.Model;

namespace Lynx.Datagen;

/// <summary>
/// Implementation based on viriformat filtering
/// </summary>
public class ViriformatFilter
{
    private const int MaxNumberOfPositionsPerGame = Constants.MaxNumberMovesInAGame * 2;

    private enum FilterWDL
    {
        Loss = 0,
        Draw = 1,
        Win = 2,
    }

    public int MinPly { get; set; } = 16;

    public int MinPieces { get; set; } = 4;

    public uint MaxInitialEval { get; set; } = 500;

    public uint MaxEval { get; set; } = 20_000;

    public bool FilterTactical { get; set; } = true;

    public bool FilterCheck { get; set; } = true;

    public bool FilterCastling { get; set; }

    public uint MaxEvalIncorrectness { get; set; } = uint.MaxValue;

    public bool LimitPositionsPerGame { get; set; }

    public int MaxPositionsPerGame { get; set; } = MaxNumberOfPositionsPerGame;

    public bool LimitPositionsPerPhasePerGame { get; set; }

    public int MaxPositionsPerPhasePerGame { get; set; } = MaxNumberOfPositionsPerGame;

    public bool RandomFenSkipping { get; set; }

    public double RandomFenSkipProbability { get; set; }

    public bool MaterialCountFiltered { get; set; }

    /// <summary>
    /// 32 items expected
    /// </summary>
    public double[] MaterialCountProbabilities { get; set; } = [];

    #region WDL filtering

    public bool WdlFiltered { get; set; }

    /// <summary>
    /// 4 items expected
    /// </summary>
    public double[] WdlModelParamsA { get; set; } = [];

    /// <summary>
    /// 4 items expected
    /// </summary>
    public double[] WdlModelParamsB { get; set; } = [];

    public int MaterialMin { get; set; } = 17;

    public int MaterialMax { get; set; } = 78;

    public int MomTarget { get; set; } = 58;

    public double WdlHeuristicScale { get; set; } = 1.5;

    #endregion

    public static ViriformatFilter Unrestricted => new()
    {
        MinPly = 0,
        MinPieces = 0,
        MaxInitialEval = uint.MaxValue,
        MaxEval = uint.MaxValue,
        FilterTactical = false,
        FilterCheck = false,
        FilterCastling = false,
        MaxEvalIncorrectness = uint.MaxValue,
        LimitPositionsPerGame = false,
        MaxPositionsPerGame = MaxNumberOfPositionsPerGame,
        LimitPositionsPerPhasePerGame = false,
        MaxPositionsPerPhasePerGame = MaxNumberOfPositionsPerGame,
        RandomFenSkipping = false,
        RandomFenSkipProbability = 0.0,
        WdlFiltered = false,
        WdlModelParamsA = [],
        WdlModelParamsB = [],
        MaterialMin = 17,
        MaterialMax = 78,
        MomTarget = 58,
        WdlHeuristicScale = 1.0,
        MaterialCountFiltered = false,
        MaterialCountProbabilities = [],
    };

    // Compute FilterWDL model probabilities (win, draw, loss) mirroring the Rust implementation.
    private (double win, double draw, double loss) WdlModel(int material, int eval)
    {
        var m = Math.Clamp(material, MaterialMin, MaterialMax) / (double)MomTarget;
        var pAs = WdlModelParamsA;
        var pBs = WdlModelParamsB;

        double a = (((((pAs[0] * m) + pAs[1]) * m) + pAs[2]) * m) + pAs[3];
        double b = (((((pBs[0] * m) + pBs[1]) * m) + pBs[2]) * m) + pBs[3];
        b *= WdlHeuristicScale;

        double x = eval;
        double w = 1.0 / (1.0 + Math.Exp((a - x) / b));
        double l = 1.0 / (1.0 + Math.Exp((a + x) / b));
        double d = Math.Max(0.0, 1.0 - w - l);
        return (w, d, l);
    }

    private double ResultChance(int material, int eval, FilterWDL wdl)
    {
        var (w, d, l) = WdlModel(material, eval);
        return wdl switch
        {
            FilterWDL.Win => w,
            FilterWDL.Draw => d,
            FilterWDL.Loss => l,
            _ => 0.0,
        };
    }

    public bool ShouldDrop(Move mv, int eval, Position position, byte wdlPacked, int ply, Random rng, bool firstGameMove = false)
    {
        if (ply < MinPly)
        {
            return true;
        }

        if(firstGameMove && Math.Abs(eval) >= MaxInitialEval)
        {
            return true;
        }

        if (Math.Abs(eval) >= MaxEval)
        {
            return true;
        }

        if (position.CountPieces() < MinPieces)
        {
            return true;
        }

        if (FilterTactical)
        {
            if (mv.CapturedPiece() != (int)Piece.None || mv.IsPromotion())
            {
                return true;
            }
        }

        if (FilterCheck && position.IsInCheck())
        {
            return true;
        }

        if (FilterCastling && mv.IsCastle())
        {
            return true;
        }

        if (RandomFenSkipping && rng.NextDouble() < RandomFenSkipProbability)
        {
            return true;
        }

        if (WdlFiltered)
        {
            var outcome = wdlPacked switch { 2 => FilterWDL.Win, 1 => FilterWDL.Draw, 0 => FilterWDL.Loss, _ => FilterWDL.Draw };
            double chance = ResultChance(position.CountPieces(), eval, outcome);
            if (rng.NextDouble() < (1.0 - chance))
            {
                return true;
            }
        }

        if (MaterialCountFiltered)
        {
            int index = Math.Min(position.CountPieces(), 32);
            double prob = MaterialCountProbabilities[index];
            if (rng.NextDouble() < prob)
            {
                return true;
            }
        }

        if (MaxEvalIncorrectness != uint.MaxValue)
        {
            if (wdlPacked == (byte)FilterWDL.Draw && Math.Abs(eval) > MaxEvalIncorrectness)
            {
                return true;
            }

            int winnerPovEval = wdlPacked == (byte)FilterWDL.Win ? eval : -eval;
            if (Math.Abs(Math.Min(winnerPovEval, 0)) > MaxEvalIncorrectness)
            {
                return true;
            }
        }

        return false;
    }
}
