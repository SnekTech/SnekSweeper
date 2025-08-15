using SnekSweeper.GridSystem;

namespace SnekSweeper.CheatCodeSystem;

public interface ICheatCodeGridEffect
{
    void TakeEffect(IHumbleGrid humbleGrid);
}

public class SetGridCoverAlpha(float normalizedAlpha) : ICheatCodeGridEffect
{
    public void TakeEffect(IHumbleGrid humbleGrid)
    {
        foreach (var humbleCell in humbleGrid.HumbleCells)
        {
            humbleCell.SetCoverAlpha(normalizedAlpha);
        }
    }
}