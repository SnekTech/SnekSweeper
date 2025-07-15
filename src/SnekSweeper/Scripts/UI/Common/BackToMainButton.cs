using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper.UI.Common;

public partial class BackToMainButton : Button
{
    public override void _Ready()
    {
        Pressed += () => SceneSwitcher.Instance.GotoScene(SceneName.Main);
    }
}