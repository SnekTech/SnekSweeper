namespace SnekSweeperCore.GridSystem;

/// <summary>指针的哪个动作。Primary = 左键(揭示/chord)，Secondary = 右键(插旗)。</summary>
public enum GridButton
{
    Primary,
    Secondary,
}

/// <summary>
/// 指针在网格上的落点。"不在网格内"是一个正经状态，不是"缺失的值" —— 所以它是 DU 而不是 <c>GridIndex?</c>。
/// 名字说的是<b>落点</b>而不是指针本身：被建模的是"指针落在哪"，不是那个设备。
/// case 嵌在类型里，避免 <c>OnGrid</c> / <c>OffGrid</c> 这类很泛的名字污染 namespace
/// （与 <c>GridState.Input.Xxx</c> 同风格）。
/// </summary>
public abstract record PointerTarget
{
    public sealed record OnGrid(GridIndex Index) : PointerTarget;

    public sealed record OffGrid : PointerTarget;
}

/// <summary>按压状态：没按着键 / 正按着某个键。</summary>
public abstract record PressState
{
    public sealed record Idle : PressState;

    public sealed record Pressing(GridButton Button) : PressState;
}

/// <summary>
/// 会话状态 = 两个独立轴的组合：
/// <list type="bullet">
/// <item><see cref="Target"/>：指针落在哪（永远有意义，不管按没按）</item>
/// <item><see cref="PressState"/>：有没有按着键、按的哪个</item>
/// </list>
/// 两个轴各占一份存储、各自成层：位置不必靠投影拼出来，移动指针也不会带着按键信息搬家。
/// </summary>
public readonly record struct GridInputSession(PointerTarget Target, PressState PressState)
{
    // todo: implement the presentation layer
    /// <summary>应该表现为"凹下去"的格（供表现层使用）；null = 没有按压（或指针不在网格内）。</summary>
    public GridIndex? PressedPreview => (PressState, Target) switch
    {
        (PressState.Pressing { Button: GridButton.Primary }, PointerTarget.OnGrid onGrid) => onGrid.Index,
        _ => null,
    };
}

/// <summary>
/// 会话走一步的产出：新的会话状态 + 这一步发出的网格意图。
/// 意图为 null 表示"这一步什么都没发生"，这是精确语义，不需要再包一层 Option。
/// </summary>
public readonly record struct GridInputOutcome(GridInputSession Session, GridInput? Intent);

public static class GridInputSessionExtensions
{
    extension(GridInputSession session)
    {
        public static GridInputSession Initial => new(new PointerTarget.OffGrid(), new PressState.Idle());

        public GridInputOutcome Move(PointerTarget target) => new(session with { Target = target }, null);

        public GridInputOutcome Press(GridButton button, PointerTarget target, bool isDoubleClick = false) =>
            target switch
            {
                PointerTarget.OnGrid onGrid => new(
                    session with { Target = target, PressState = new PressState.Pressing(button) },
                    IntentOfPressedButton(button, isDoubleClick, onGrid.Index)),
                // 按下落在网格外：这次交互整个不算数（它的抬起也不会产生意图）
                _ => new(session with { Target = target, PressState = new PressState.Idle() }, null),
            };

        public GridInputOutcome Release(GridButton button, PointerTarget target)
        {
            var intent = (session.PressState, button, target) switch
            {
                (PressState.Pressing { Button: GridButton.Primary }, GridButton.Primary, PointerTarget.OnGrid onGrid) =>
                    new RevealAt(onGrid.Index),
                _ => null,
            };

            var ownsPress = session.PressState is PressState.Pressing { Button: var held } && held == button;
            PressState nextPressState = ownsPress ? new PressState.Idle() : session.PressState;

            return new(session with { Target = target, PressState = nextPressState }, intent);
        }
    }

    /// <summary>按下当场产生的意图（右键插旗 / 双击 chord）；null = 这一步没有意图（揭示要等抬起才确认）。</summary>
    static GridInput? IntentOfPressedButton(GridButton button, bool isDoubleClick, GridIndex index) => (button, isDoubleClick) switch
    {
        (GridButton.Primary, true) => new ChordAt(index),
        (GridButton.Secondary, _) => new SwitchFlagAt(index),
        _ => null,
    };
}
