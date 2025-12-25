using GodotGadgets.Extensions;
using GodotGadgets.TooltipSystem;
using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekTech.Tooltip;

namespace SnekSweeper.CheatCodeSystem.UI;

[SceneTree]
public partial class CheatCodeCard : PanelContainer, ISceneScript
{
    CheatCode _cheatCode = null!;
    readonly ActivatedCheatCodeSet _activatedCheatCodeSet = HouseKeeper.ActivatedCheatCodeSet;

    public override void _EnterTree()
    {
        CheckButton.Toggled += OnCheckButtonToggled;
    }

    public override void _ExitTree()
    {
        CheckButton.Toggled -= OnCheckButtonToggled;
    }

    internal void Init(CheatCode cheatCode, ITooltipDisplay tooltipDisplay)
    {
        _cheatCode = cheatCode;

        NameLabel.Text = cheatCode.Data.Name;
        Icon.Texture = cheatCode.Icon;
        CheckButton.SetPressed(cheatCode.IsActivatedIn(_activatedCheatCodeSet));
        
        InitTooltip();
        return;

        void InitTooltip()
        {
            var tooltipTrigger = this.GetFirstChildOfType<ControlTooltipTrigger>();
            tooltipTrigger.SetTooltipContent(TooltipContent.New(cheatCode.Data.Name, cheatCode.Data.Description));
            tooltipTrigger.SetTooltipDisplay(tooltipDisplay);
        }
    }

    void OnCheckButtonToggled(bool toggledOn)
    {
        _cheatCode.SetActivatedIn(_activatedCheatCodeSet, toggledOn);
    }
}