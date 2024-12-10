using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper.UI;

[Scene]
public partial class WinningPage : MarginContainer
{
    [Node] private Button backToMainButton = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        backToMainButton.Pressed += () => SceneManager.Instance.GotoScene(SceneName.Main);
    }
}