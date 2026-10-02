using System.Runtime.CompilerServices;
using SnekSweeperCore.CellSystem;

namespace SnekSweeperCore.GridSystem;

public abstract record GridOutcome
{
    public sealed record BatchRevealed(IReadOnlyList<CellOutcome> Cells) : GridOutcome;
    public sealed record BatchCovered(IReadOnlyList<CellOutcome> Cells) : GridOutcome;

    public sealed record FlagToggled(CellOutcome CellOutcome) : GridOutcome
    {
        public bool IsRaised => CellOutcome.Event is CellEvent.FlagRaised;
    }

    public sealed record ErrorsMarked(IReadOnlyList<CellOutcome> Cells) : GridOutcome;
    public sealed record NothingHappens : GridOutcome;
}

public static class GridOutcomeExtensions
{
    extension(GridOutcome gridOutcome)
    {
        // 用于折叠同质的 outcome，混合 outcome 没意义所以 throw
        // 目前仅用于 CompoundCommand 中 BatchRevealed 和 BatchCovered 的合并
        public GridOutcome MergeWith(GridOutcome another) =>
            (gridOutcome, another) switch
            {
                (GridOutcome.BatchRevealed batchRevealedA, GridOutcome.BatchRevealed batchRevealedB) =>
                    new GridOutcome.BatchRevealed([.. batchRevealedA.Cells, .. batchRevealedB.Cells]),
                (GridOutcome.BatchCovered batchCoveredA, GridOutcome.BatchCovered batchCoveredB) =>
                    new GridOutcome.BatchCovered([.. batchCoveredA.Cells, .. batchCoveredB.Cells]),
                (GridOutcome.NothingHappens, _) => another,
                (_, GridOutcome.NothingHappens) => gridOutcome,
                _ => throw new SwitchExpressionException(),
            };
    }
}