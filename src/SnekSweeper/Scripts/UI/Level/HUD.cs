using System.Diagnostics;
using GodotGadgets.InputProcessing;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class HUD : CanvasLayer, ISceneScript
{
    static DeviceDetector Detector => DeviceDetectorDriver.Detector;

    public override void _EnterTree()
    {
        Detector.DeviceChanged += OnDeviceChanged;
        UndoButton.Pressed += OnUndoPressed;
    }

    public override void _Ready()
    {
        UpdateDeviceText(Detector.Current);
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

    void OnDeviceChanged(InputDevice previous, InputDevice detected) => UpdateDeviceText(detected);

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
            Mouse or Keyboard => "keyboard / mouse",
            Gamepad { Brand: var brand } => $"{nameof(Gamepad)} ({brand})",
            Unknown => nameof(Unknown),
            _ => throw new UnreachableException(),
        };

        InputDeviceLabel.Text = $"device: {deviceText}";
    }
}