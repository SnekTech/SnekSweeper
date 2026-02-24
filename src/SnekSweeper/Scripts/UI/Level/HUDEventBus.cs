namespace SnekSweeper.UI.Level;

public class HUDEventBus
{
    public event Action? UndoPressed;

    public void EmitUndoPressed() => UndoPressed?.Invoke();
}