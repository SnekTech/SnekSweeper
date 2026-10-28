using GodotGadgets.InputProcessing;

namespace SnekSweeper.Autoloads;

public partial class DeviceDetectorDriver : Node
{
    public static DeviceDetector Detector { get; } = new();

    public override void _Ready()
    {
        Input.JoyConnectionChanged += OnJoyConnectionChanged;
    }

    public override void _ExitTree()
    {
        Input.JoyConnectionChanged -= OnJoyConnectionChanged;
    }

    public override void _Input(InputEvent @event)
    {
        if (ToSample(@event) is { } sample) Detector.Handle(sample);
    }

    static InputSample? ToSample(InputEvent @event) => @event switch
    {
        InputEventMouse => InputSample.Mouse.Instance,
        InputEventKey => InputSample.Keyboard.Instance,
        InputEventJoypadButton { Device: var deviceId } when Gamepads.Find(deviceId) is { } gamepad =>
            new InputSample.GamepadButton(gamepad),
        InputEventJoypadMotion
        {
            Axis: JoyAxis.LeftX or JoyAxis.LeftY or JoyAxis.RightX or JoyAxis.RightY,
            Device: var deviceId, AxisValue: var value,
        } when Gamepads.Find(deviceId) is { } gamepad => new InputSample.GamepadAxis(gamepad, value),
        _ => null,
    };

    static void OnJoyConnectionChanged(long device, bool connected)
    {
        Gamepads.ForgetCache((int)device);
    }
}