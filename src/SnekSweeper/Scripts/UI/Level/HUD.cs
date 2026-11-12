using System.Diagnostics;
using System.Runtime.CompilerServices;
using GodotGadgets.InputProcessing;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.CustomResources;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class HUD : CanvasLayer, ISceneScript
{
    [Export]
    GamepadGlyphCatalog _gamepadGlyphCatalog = null!;
    
    static DeviceDetector Detector => DeviceDetectorDriver.Detector;

    public override void _EnterTree()
    {
        Detector.DeviceChanged += OnDeviceChanged;
        UndoButton.Pressed += OnUndoPressed;
    }

    public override void _Ready()
    {
        UpdateDeviceText(Detector.Current);
        UpdateInputHint(Detector.Current);
    }

    public override void _ExitTree()
    {
        Detector.DeviceChanged -= OnDeviceChanged;
        UndoButton.Pressed -= OnUndoPressed;
    }

    public GDTask<PopupChoiceOnWin> ShowAndGetChoiceOnWinAsync(CancellationToken ct = default) =>
        _.PopupLayer.ShowAndGetChoiceOnWinAsync(ct);

    public GDTask<PopupChoiceOnLose> ShowAndGetChoiceOnLoseAsync(CancellationToken ct = default) =>
        _.PopupLayer.ShowAndGetChoiceOnLoseAsync(ct);

    void OnUndoPressed() => UndoRequested?.Invoke();

    void OnDeviceChanged(InputDevice previous, InputDevice detected)
    {
        UpdateDeviceText(detected);
        UpdateInputHint(detected);
    }

    public event Action? UndoRequested;

    public void UpdateBombCount(int bombCount) => BombCountLabel.Text = $"{bombCount} bombs";
    public void UpdateFlagCount(int flagCount) => FlagCountLabel.Text = $"{flagCount} flags";
    public void IncreaseCombo() => ComboRankCard.IncreaseCombo();

    public void ResetForNewRun()
    {
        ComboRankCard.Reset();
        _.PopupLayer.Reset();
    }

    void UpdateDeviceText(InputDevice device)
    {
        var deviceText = device switch
        {
            InputDevice.Mouse or InputDevice.Keyboard => "keyboard / mouse",
            InputDevice.Gamepad { Brand: var brand } => $"{nameof(InputDevice.Gamepad)} ({brand})",
            InputDevice.NoDevice => "-",
            _ => throw new UnreachableException(),
        };

        InputDeviceLabel.Text = $"device: {deviceText}";
    }

    void UpdateInputHint(InputDevice device)
    {
        GamepadHintList.ShowHints(DeviceHints(device, _gamepadGlyphCatalog));
    }

    static IReadOnlyList<InputHint> GamepadHints(IGamepadGlyphProvider glyphProvider) =>
    [
        new(glyphProvider.Glyph(new GamepadFaceButton.West()), "Reveal"),
        new(glyphProvider.Glyph(new GamepadFaceButton.East()), "Flag"),
        new(glyphProvider.Glyph(new GamepadFaceButton.North()), "Chord"),
    ];

    static IReadOnlyList<InputHint> DeviceHints(InputDevice device, GamepadGlyphCatalog catalog) => device switch
    {
        InputDevice.Gamepad { Brand: var brand } => GamepadHints(catalog.For(brand)),
        InputDevice.Mouse or InputDevice.Keyboard => [],
        InputDevice.NoDevice => [],
        _ => throw new SwitchExpressionException(),
    };
}