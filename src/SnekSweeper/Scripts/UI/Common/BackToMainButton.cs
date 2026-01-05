using GodotGadgets.Tasks;

namespace SnekSweeper.UI.Common;

public partial class BackToMainButton : Button
{
    public override void _Ready()
    {
        Pressed += () => Autoload.SceneSwitcher.GotoSceneAsync<Main>().Fire();
    }
}