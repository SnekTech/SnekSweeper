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

    public override void _Input(InputEvent evt)
    {
        Detector.Handle(evt);
    }

    static void OnJoyConnectionChanged(long device, bool connected)
    {
        Gamepads.ForgetCache((int)device);
    }
}