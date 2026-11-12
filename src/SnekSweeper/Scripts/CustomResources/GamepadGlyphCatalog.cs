using System.Runtime.CompilerServices;
using GodotGadgets.InputProcessing;

namespace SnekSweeper.CustomResources;

[GlobalClass]
public partial class GamepadGlyphCatalog : Resource
{
    [Export]
    public GamepadGlyphSet Xbox { get; set; } = null!;
    [Export]
    public GamepadGlyphSet PlayStation { get; set; } = null!;
    [Export]
    public GamepadGlyphSet Nintendo { get; set; } = null!;

    public IGamepadGlyphProvider For(GamepadBrand brand) => brand switch
    {
        GamepadBrand.Xbox => Xbox,
        GamepadBrand.PlayStation => PlayStation,
        GamepadBrand.Nintendo => Nintendo,
        _ => throw new SwitchExpressionException(),
    };
}