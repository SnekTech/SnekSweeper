using SnekSweeper.CheatCodeSystem.UI;
using SnekSweeper.Levels;
using SnekSweeper.UI.History;
using SnekSweeper.UI.Settings;

namespace SnekSweeper.UI;

[SceneTree]
public partial class MainMenu : CanvasLayer
{
    public override void _Ready()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        var sceneSwitcher = Autoload.SceneSwitcher;
        StartButton.Pressed += () => sceneSwitcher.GotoScene<Level1>();
        SettingsButton.Pressed += () => sceneSwitcher.GotoScene<SettingsPage>();
        CheatCodeButton.Pressed += () => sceneSwitcher.GotoScene<CheatCodePage>();
        HistoryButton.Pressed += () => sceneSwitcher.GotoScene<HistoryPage>();

        QuitButton.Pressed += () => GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}