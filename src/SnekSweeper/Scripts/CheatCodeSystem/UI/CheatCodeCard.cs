using GodotGadgets.Extensions;
using GodotGadgets.TooltipSystem;
using SnekSweeper.Widgets;
using SnekTech.Tooltip;

namespace SnekSweeper.CheatCodeSystem.UI;

[SceneTree]
public partial class CheatCodeCard : PanelContainer, ISceneScript
{
    CheatCode _cheatCode = null!;

    public override void _EnterTree()
    {
        CheckButton.Pressed += OnCheckButtonPressed;
    }

    public override void _ExitTree()
    {
        CheckButton.Pressed -= OnCheckButtonPressed;
    }

    internal void Init(CheatCode cheatCode, ITooltipDisplay tooltipDisplay)
    {
        _cheatCode = cheatCode;

        NameLabel.Text = cheatCode.Data.Name;
        Icon.Texture = cheatCode.Icon;
        CheckButton.SetPressed(cheatCode.IsActivated);
        
        InitTooltip();
        return;

        void InitTooltip()
        {
            var tooltipTrigger = this.GetFirstChildOfType<ControlTooltipTrigger>();
            tooltipTrigger.SetTooltipContent(TooltipContent.New(cheatCode.Data.Name, cheatCode.Data.Description));
            tooltipTrigger.SetTooltipDisplay(tooltipDisplay);
        }
    }

    void OnCheckButtonPressed()
    {
        _cheatCode.IsActivated = !_cheatCode.IsActivated;
    }
}