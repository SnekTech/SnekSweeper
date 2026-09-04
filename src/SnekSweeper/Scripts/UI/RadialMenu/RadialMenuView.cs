using GodotGadgets.FSCore.RadialMenuDomain;
using Radial = GodotGadgets.FSCore.RadialMenuDomain.RadialMenu;

namespace SnekSweeper.UI.RadialMenu;

/// <summary>
/// 轮盘选择器的 Godot 表现层（演示/测试用）。所有决策都在纯 F# 核心
/// （GodotGadgets.FSCore.RadialMenuDomain）里，本脚本只做三件事：
///  1) 把输入翻译成核心的边界调用（openDial/aimDial/closeDial）；
///  2) 持有核心返回的 WheelState（跨帧状态）；
///  3) 把核心返回的事件（Opened/HoverChanged/Picked/Cancelled）执行到自建的可视节点上。
/// 用法：测试场景中把它挂到任意 Control；按住 HoldButton 打开轮盘，
/// 用左摇杆（无手柄输入时回退为鼠标相对中心偏移）瞄准，松开确认 / 取消。
/// </summary>
public partial class RadialMenuView : Control
{
    [Export] public JoyButton HoldButton { get; set; } = JoyButton.RightShoulder;
    [Export] public float Radius { get; set; } = 200f;
    [Export] public Vector2 SlotSize { get; set; } = new(64, 64);
    [Export] public bool MouseAimFallback { get; set; } = true;

    readonly RadialConfig _config = RadialConfig.Default;
    WheelState _state = Radial.initial;

    ColorRect[] _slots = [];
    Label _status = null!;
    int _hovered = -1;
    float _axisX;
    float _axisY;

    static readonly Color HighlightColor = new(1.35f, 1.2f, 0.6f);

    /// 松开肩键确认了某个扇区（下标）。
    public event Action<int>? SlotPicked;

    /// 回中松开肩键（取消）。
    public event Action? WheelCancelled;

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        BuildSlots();
        BuildStatusLabel();
        Visible = false;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized)
        {
            LayoutSlots();
        }
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventJoypadButton { Pressed: var pressed, ButtonIndex: var button } when button == HoldButton:
                Step(pressed
                    ? Radial.openDial(_config, _state)
                    : Radial.closeDial(_config, _state));
                break;
            case InputEventJoypadMotion { Axis: JoyAxis.LeftX, AxisValue: var v }:
                _axisX = v;
                break;
            case InputEventJoypadMotion { Axis: JoyAxis.LeftY, AxisValue: var v }:
                _axisY = v;
                break;
        }
    }

    public override void _Process(double _)
    {
        // 轮盘没开就不轮询
        if (_state is not WheelState.Open) return;

        var x = _axisX;
        var y = _axisY;

        // 无手柄轴输入时，用鼠标相对轮盘中心的偏移瞄准（方便没手柄调试）
        if (MouseAimFallback && Mathf.Abs(x) < 0.05f && Mathf.Abs(y) < 0.05f)
        {
            var offset = GetGlobalMousePosition() - GetGlobalRect().GetCenter();
            if (offset.Length() > 1f)
            {
                x = offset.X / Radius;
                y = offset.Y / Radius;
            }
        }

        Step(Radial.aimDial(_config, x, y, _state));
    }

    // ---- 核心接线：只翻译 + 只执行，不做任何决策 ----

    void Step(StepResult result)
    {
        _state = result.State;

        foreach (var e in result.Events)
        {
            Dispatch(e);
        }
    }

    // F# 空分支（Opened/Cancelled/None）以静态单例暴露，值分支以子类暴露：
    // 空分支用引用相等判断，值分支用类型模式 + .Item 读载荷。
    void Dispatch(WheelEvent e)
    {
        if (ReferenceEquals(e, WheelEvent.Opened))
        {
            ShowWheel();
            return;
        }

        if (ReferenceEquals(e, WheelEvent.Cancelled))
        {
            OnCancelled();
            return;
        }

        switch (e)
        {
            case WheelEvent.HoverChanged hover:
                ApplyHover(hover.Item);
                break;
            case WheelEvent.Picked picked:
                OnPicked(picked.Item);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(e), "unhandled wheel event");
        }
    }

    void ApplyHover(Selection selection)
    {
        if (ReferenceEquals(selection, Selection.None))
        {
            SetHovered(-1);
            return;
        }

        if (selection is Selection.Sector sector)
        {
            SetHovered(sector.Item);
            return;
        }

        throw new ArgumentOutOfRangeException(nameof(selection), "unhandled selection");
    }

    void ShowWheel()
    {
        Visible = true;
        SetHovered(-1);
    }

    void OnPicked(int slot)
    {
        Visible = false;
        SetHovered(-1);
        _status.Text = $"picked {slot}";
        SlotPicked?.Invoke(slot);
    }

    void OnCancelled()
    {
        Visible = false;
        SetHovered(-1);
        _status.Text = "cancelled";
        WheelCancelled?.Invoke();
    }

    void SetHovered(int slot)
    {
        if (slot == _hovered) return;
        _hovered = slot;

        for (var i = 0; i < _slots.Length; i++)
        {
            var highlighted = i == slot;
            _slots[i].Modulate = highlighted ? HighlightColor : Colors.White;
            _slots[i].Scale = highlighted ? Vector2.One * 1.15f : Vector2.One;
        }
    }

    // ---- 自建演示视觉（正式接入时替换成真实图标节点即可）----

    void BuildSlots()
    {
        var n = Radial.slotCount(_config);

        for (var i = 0; i < n; i++)
        {
            var rect = new ColorRect { Color = SlotColor(i, n) };
            rect.Size = SlotSize;
            rect.PivotOffset = SlotSize / 2f;

            var label = new Label
            {
                Text = i.ToString(),
                Size = SlotSize,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            rect.AddChild(label);
            AddChild(rect);
        }

        _slots = GetChildren().OfType<ColorRect>().ToArray();
        LayoutSlots();
    }

    static Color SlotColor(int i, int n)
    {
        var t = n <= 1 ? 0f : i / (float)(n - 1);
        return new Color(0.15f + 0.55f * t, 0.3f, 0.55f, 0.9f);
    }

    void BuildStatusLabel()
    {
        _status = new Label { Text = "", HorizontalAlignment = HorizontalAlignment.Center };
        _status.SetAnchorsPreset(LayoutPreset.BottomWide);
        _status.Position = new Vector2(_status.Position.X, _status.Position.Y - 28);
        AddChild(_status);
    }

    void LayoutSlots()
    {
        if (_slots.Length == 0) return;

        var center = Size / 2f;
        for (var i = 0; i < _slots.Length; i++)
        {
            var rad = Radial.centerAngle(_config, i);
            var dir = new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
            _slots[i].Position = center + dir * Radius - SlotSize / 2f;
        }
    }
}
