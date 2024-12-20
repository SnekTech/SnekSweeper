using Godot;

namespace SnekSweeper.UI;

public partial class LoadingSquares : AnimatedSprite2D
{
    public override void _Ready()
    {
        Play();
    }
}