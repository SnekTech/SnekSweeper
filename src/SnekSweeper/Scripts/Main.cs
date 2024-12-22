using Godot;
using SnekSweeper.Roguelike;

namespace SnekSweeper;

public partial class Main : Node
{
    public override void _Ready()
    {
        Rand.Randomize();
    }
}