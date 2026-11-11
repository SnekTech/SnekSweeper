using GodotGadgets.Extensions;
using GodotGadgets.InputProcessing;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common.InputHintView;

[SceneTree]
public partial class GamepadHintList : HBoxContainer
{
    public void ShowHints(IReadOnlyList<InputHint> hints)
    {
        this.ClearChildren();
        
        foreach (var inputHint in hints)
        {
            var segment = GamepadHintSegment.InstantiateOnParent(this);
            segment.ShowHint(inputHint);
        }
    }
}