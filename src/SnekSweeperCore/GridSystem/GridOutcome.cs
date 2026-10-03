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