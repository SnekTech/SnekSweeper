using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;

namespace SnekSweeper.Levels;

[SceneTree]
public partial class Level1 : Node2D, ISceneScript
{
    public override void _ExitTree()
    {
        HouseKeeper.SaveCurrentPlayerData();
    }
}