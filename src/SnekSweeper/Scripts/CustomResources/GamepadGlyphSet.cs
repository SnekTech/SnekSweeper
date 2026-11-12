using GodotGadgets.InputProcessing;

namespace SnekSweeper.CustomResources;

[GlobalClass]
public partial class GamepadGlyphSet : Resource, IGamepadGlyphProvider
{
    [Export]
    public Texture2D West { get; set; } = null!;

    [Export]
    public Texture2D North { get; set; } = null!;

    [Export]
    public Texture2D East { get; set; } = null!;

    [Export]
    public Texture2D South { get; set; } = null!;

    public Texture2D Glyph(GamepadFaceButton button) =>
        button switch
        {
            GamepadFaceButton.West => West,
            GamepadFaceButton.North => North,
            GamepadFaceButton.East => East,
            GamepadFaceButton.South => South,
        };
}