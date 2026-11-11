using GodotGadgets.InputProcessing;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common.InputHintView;

[SceneTree]
public partial class GamepadHintSegment : HBoxContainer, ISceneScript
{
    public void ShowHint(InputHint hint)
    {
        _.TextureRect.Texture = hint.Glyph;
        _.Label.Text = hint.Label;
    }
}