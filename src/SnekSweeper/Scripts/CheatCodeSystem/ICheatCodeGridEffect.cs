using SnekSweeper.Autoloads;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.CheatCodeSystem;

interface ICheatCodeGridEffect
{
    void Trigger(IHumbleGrid humbleGrid);
}

sealed class SetGridCoverAlpha(float normalizedAlpha) : ICheatCodeGridEffect
{
    public void Trigger(IHumbleGrid humbleGrid)
    {
        foreach (var humbleCell in humbleGrid.HumbleCells)
        {
            humbleCell.Cover.SetAlpha(normalizedAlpha);
        }
    }
}

sealed class SendMessage : ICheatCodeGridEffect
{
    public void Trigger(IHumbleGrid humbleGrid)
    {
        MessageBox.Print("Hello from messenger!");
    }
}