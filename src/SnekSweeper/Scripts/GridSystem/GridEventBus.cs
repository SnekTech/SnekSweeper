namespace SnekSweeper.GridSystem;

public class GridEventBus
{
    public event Action<int>? BombCountChanged;
    public event Action<int>? FlagCountChanged;
    public event Action? BatchRevealed;

    public void EmitFlagCountChanged(int flagCount) => FlagCountChanged?.Invoke(flagCount);
    public void EmitBombCountChanged(int bombCount) => BombCountChanged?.Invoke(bombCount);
    public void EmitBatchRevealed() => BatchRevealed?.Invoke();
}