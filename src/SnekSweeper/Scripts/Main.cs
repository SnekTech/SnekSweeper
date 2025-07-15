using SnekSweeper.Widgets;
using Widgets.Roguelike;

namespace SnekSweeper;

[SceneTree]
public partial class Main : Node, ISceneScript
{
    public override void _Ready()
    {
        Rand.Randomize();
    }
}