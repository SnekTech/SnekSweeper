using System;

namespace SnekSweeper.UI;

public class HUDEventBus
{
    public event Action? UndoPressed;

    public void EmitUndoPressed() => UndoPressed?.Invoke();
}