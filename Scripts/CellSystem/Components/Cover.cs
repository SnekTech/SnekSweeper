using Godot;

namespace SnekSweeper.CellSystem.Components;

public partial class Cover : Sprite2D, ICover
{
    public void Reveal()
    {
        Hide();
    }

    public void PutOn()
    {
        Show();
    }
}