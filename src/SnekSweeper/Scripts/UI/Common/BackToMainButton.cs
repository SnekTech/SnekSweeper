using SnekSweeper.Autoloads;

namespace SnekSweeper.UI.Common;

public partial class BackToMainButton : Button
{
    public override void _Ready()
    {
        Pressed += () => SceneSwitcher.Instance.GotoScene<Main>();
    }
}