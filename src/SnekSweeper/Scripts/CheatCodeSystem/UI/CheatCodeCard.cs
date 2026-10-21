using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.TooltipSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.CheatCodeSystem.UI;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class CheatCodeCard : PanelContainer, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    CheatCode _cheatCode = null!;
    IDisposable? _tooltip;

    public override void _EnterTree() => CheckButton.Toggled += OnCheckButtonToggled;

    public override void _ExitTree()
    {
        CheckButton.Toggled -= OnCheckButtonToggled;
        _tooltip?.Dispose();
    }

    internal void Init(CheatCode cheatCode, ITooltipAttacher tooltipAttacher)
    {
        _cheatCode = cheatCode;

        NameLabel.Text = cheatCode.Data.Name;
        Icon.Texture = cheatCode.Icon;
        CheckButton.SetPressed(cheatCode.IsActivatedIn(SaveData.State.ActivatedCheatCodeSet));

        _tooltip?.Dispose();
        _tooltip = tooltipAttacher.Attach(new TooltipSource.FromControl(this),
            new TooltipContent(cheatCode.Data.Name, cheatCode.Data.Description));
    }

    void OnCheckButtonToggled(bool toggledOn)
    {
        SaveData.UpdateActivatedCheatCodeSet(set => _cheatCode.SetActivatedIn(set, toggledOn));
    }
}