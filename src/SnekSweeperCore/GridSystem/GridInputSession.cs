namespace SnekSweeperCore.GridSystem;

/// <summary>指针的哪个动作。Primary = 左键(揭示/chord)，Secondary = 右键(插旗)。</summary>
public enum GridButton
{
    Primary,
    Secondary,
}

/// <summary>会话走一步的结果：新的会话状态 + 这一步产生的网格意图（可能没有）。</summary>
public readonly record struct GridInputTransition(GridInputSession Session, GridInput? Intent);

/// <summary>
/// 指针会话：记住"指针在哪一格"和"是否有一次落在网格内的按下正在进行"，
/// 把指针动作翻译成 <see cref="GridInput"/>。
/// <para>
/// 纯状态机——不知道 Godot，也不知道鼠标。事件里的 <see cref="GridIndex"/>? 已由表现层按网格几何算好，
/// null 表示指针不在网格内（"没有指向任何格子"是语义，不是错误）。
/// </para>
/// <para>
/// 关键不变式：<b>只有本会话见过、且落在网格内的那次按下的抬起，才会产生揭示意图</b>，
/// 且意图作用在<b>抬起所在的格</b>。前半句修掉了"切场景时残留的指针动作误触新关卡"
/// （那时按下的那个 press 属于上一个场景，本会话从没见过它）；后半句是刻意保留的手感——
/// 按错了可以不松手滑到真正想按的格子再松开。
/// </para>
/// </summary>
public readonly record struct GridInputSession
{
    public static GridInputSession Initial => new();

    /// <summary>指针当前所在的格；null = 不在网格内（或还没收到过指针事件）。</summary>
    public GridIndex? Hovered { get; init; }

    /// <summary>正在进行中的按下是哪个键；null = 没有"落在网格内"的按下在进行。</summary>
    public GridButton? HeldButton { get; init; }

    /// <summary>
    /// 现在应该表现为"凹下去"的格（供表现层使用）；null = 没有按压。
    /// 它就是"按着的那个键 + 指针当前所在的格"，所以按下期间跟着指针滑动。
    /// </summary>
    public GridIndex? PressedPreview => HeldButton is GridButton.Primary ? Hovered : null;

    public GridInputTransition MoveTo(GridIndex? index) => new(this with { Hovered = index }, null);

    public GridInputTransition Press(GridButton button, GridIndex? index, bool isDoubleClick)
    {
        // 按下必须落在网格内，否则这次交互不算数（它的抬起不会产生意图）
        var session = this with
        {
            Hovered = index,
            HeldButton = index is null ? null : button,
        };

        GridInput? intent = (button, index) switch
        {
            (GridButton.Primary, { } chordIndex) when isDoubleClick => new ChordAt(chordIndex),
            (GridButton.Secondary, { } flagIndex) => new SwitchFlagAt(flagIndex),
            _ => null,
        };

        return new GridInputTransition(session, intent);
    }

    public GridInputTransition Release(GridButton button, GridIndex? index)
    {
        GridInput? intent = (button, HeldButton, index) switch
        {
            (GridButton.Primary, GridButton.Primary, { } revealIndex) => new RevealAt(revealIndex),
            _ => null,
        };

        var session = this with
        {
            Hovered = index,
            HeldButton = HeldButton == button ? null : HeldButton,
        };

        return new GridInputTransition(session, intent);
    }
}
