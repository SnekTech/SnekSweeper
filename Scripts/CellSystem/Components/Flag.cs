using Godot;

namespace SnekSweeper.CellSystem.Components;

public partial class Flag : Sprite2D, IFlag
{
    public void Raise()
    {
        Show();
    }

    public void PutDown()
    {
        Hide();
    }
}