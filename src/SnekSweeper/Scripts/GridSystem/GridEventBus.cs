using SnekSweeper.CellSystem;

namespace SnekSweeper.GridSystem;

public class GridEventBus
{
    public event Action<int>? BombCountChanged;
    public event Action<int>? FlagCountChanged;
    public event Action? BatchRevealed;
    public event Action<List<Cell>>? BombRevealed;
    public event Action? InitCompleted;

    public void EmitFlagCountChanged(int flagCount) => FlagCountChanged?.Invoke(flagCount);
    public void EmitBombCountChanged(int bombCount) => BombCountChanged?.Invoke(bombCount);
    public void EmitBatchRevealed() => BatchRevealed?.Invoke();
    public void EmitBombRevealed(List<Cell> bombCellsRevealed) => BombRevealed?.Invoke(bombCellsRevealed);
    public void EmitInitCompleted() => InitCompleted?.Invoke();
}