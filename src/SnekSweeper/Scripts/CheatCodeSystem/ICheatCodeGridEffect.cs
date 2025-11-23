using SnekSweeper.GridSystem;

namespace SnekSweeper.CheatCodeSystem;

public interface ICheatCodeGridEffect
{
    void Trigger(IHumbleGrid humbleGrid);
}

public class SetGridCoverAlpha(float normalizedAlpha) : ICheatCodeGridEffect
{
    public void Trigger(IHumbleGrid humbleGrid)
    {
        foreach (var humbleCell in humbleGrid.HumbleCells)
        {
            humbleCell.Cover.SetAlpha(normalizedAlpha);
        }
    }
}