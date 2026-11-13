using GodotGadgets.InputProcessing;
using SnekSweeper.CustomResources;

namespace SnekSweeper.UI.Common.InputHintView;

static class InputHintPresenter
{
    static IReadOnlyList<InputHint> GamepadHints(IGamepadGlyphProvider glyphProvider) =>
    [
        new(glyphProvider.Glyph(new GamepadFaceButton.West()), "Reveal"),
        new(glyphProvider.Glyph(new GamepadFaceButton.East()), "Flag"),
        new(glyphProvider.Glyph(new GamepadFaceButton.North()), "Chord"),
    ];

    internal static IReadOnlyList<InputHint> DeviceHints(InputDevice device, GamepadGlyphCatalog catalog) => device switch
    {
        InputDevice.Gamepad { Brand: var brand } => GamepadHints(catalog.For(brand)),
        InputDevice.Mouse or InputDevice.Keyboard => [],
        InputDevice.NoDevice => [],
    };
}