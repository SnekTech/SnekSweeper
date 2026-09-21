using System.Runtime.CompilerServices;

namespace SnekSweeperCore.GridSystem;

/// <summary>指针的哪个动作。Primary = 左键(揭示/chord)，Secondary = 右键(插旗)。</summary>
public enum GridButton
{
    Primary,
    Secondary,
}

/// <summary>
/// 指针指向哪里。"不在网格内"是一个正经状态，不是"缺失的值" —— 所以它是 DU 而不是 <c>GridIndex?</c>。
/// case 嵌在类型里，避免 <c>OnGrid</c> / <c>OffGrid</c> 这类很泛的名字污染 namespace
/// （与 <c>GridState.Input.Xxx</c> 同风格）。
/// </summary>
public abstract record Pointer
{
    public sealed record OnGrid(GridIndex Index) : Pointer;

    public sealed record OffGrid : Pointer;
}

/// <summary>
/// 会话状态。用单一 DU 而不是 { Hovered; HeldButton } 两个字段：
/// "手里按着一个键"只可能存在于 <see cref="Pressing"/> 里 ——
/// "记着按键却不知道按在哪""没按下却记着按键"这类矛盾组合在类型层面就不存在。
/// </summary>
public abstract record GridInputSession
{
    public sealed record Idle(Pointer Position) : GridInputSession;

    public sealed record Pressing(GridButton Button, Pointer Position) : GridInputSession;
}

/// <summary>会话走一步的结果：新的会话状态 + 这一步产生的网格意图（可能没有）。</summary>
public readonly record struct GridInputTransition(GridInputSession Session, GridInput? Intent);

public static class GridInputSessionExtensions
{
    extension(Pointer pointer)
    {
        /// <summary>指针指向的格；null = 不在网格内。</summary>
        public GridIndex? Index => pointer switch
        {
            Pointer.OnGrid onGrid => onGrid.Index,
            Pointer.OffGrid => null,
            _ => throw new SwitchExpressionException(),
        };
    }

    extension(GridInputSession session)
    {
        public static GridInputSession Initial => new GridInputSession.Idle(new Pointer.OffGrid());

        /// <summary>指针当前的位置（Idle 与 Pressing 都带着它）。</summary>
        public Pointer Position => session switch
        {
            GridInputSession.Idle idle => idle.Position,
            GridInputSession.Pressing pressing => pressing.Position,
            _ => throw new SwitchExpressionException(),
        };

        /// <summary>应该表现为"凹下去"的格（供表现层使用）；null = 没有按压。</summary>
        public GridIndex? PressedPreview => session switch
        {
            GridInputSession.Pressing { Button: GridButton.Primary } pressing => pressing.Position.Index,
            _ => null,
        };

        public GridInputTransition Move(Pointer pointer) => new(session.WithPointer(pointer), null);

        public GridInputTransition Press(GridButton button, Pointer pointer, bool isDoubleClick = false) =>
            pointer switch
            {
                Pointer.OnGrid onGrid => new(new GridInputSession.Pressing(button, pointer),
                    IntentOfPressedButton(button, isDoubleClick, onGrid.Index)),
                // 按下落在网格外：这次交互整个不算数（它的抬起也不会产生意图）
                _ => new(new GridInputSession.Idle(pointer), null),
            };

        public GridInputTransition Release(GridButton button, Pointer pointer)
        {
            var intent = (session, button, pointer) switch
            {
                (GridInputSession.Pressing { Button: GridButton.Primary }, GridButton.Primary, Pointer.OnGrid onGrid) =>
                    new RevealAt(onGrid.Index),
                _ => null,
            };

            var ownsPress = session is GridInputSession.Pressing { Button: var held } && held == button;
            var nextSession = ownsPress ? new GridInputSession.Idle(pointer) : session.WithPointer(pointer);

            return new(nextSession, intent);
        }

        GridInputSession WithPointer(Pointer pointer) => session switch
        {
            GridInputSession.Idle => new GridInputSession.Idle(pointer),
            GridInputSession.Pressing pressing => pressing with { Position = pointer },
            _ => throw new SwitchExpressionException(),
        };
    }

    /// <summary>按下当场产生的意图（右键插旗 / 双击 chord）；null = 这一步没有意图（揭示要等抬起才确认）。</summary>
    static GridInput? IntentOfPressedButton(GridButton button, bool isDoubleClick, GridIndex index) => (button, isDoubleClick) switch
    {
        (GridButton.Primary, true) => new ChordAt(index),
        (GridButton.Secondary, _) => new SwitchFlagAt(index),
        _ => null,
    };
}
