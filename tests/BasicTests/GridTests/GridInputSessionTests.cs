using AwesomeAssertions;
using SnekSweeperCore.GridSystem;

namespace BasicTests.GridTests;

/// <summary>
/// 指针会话的语义锁。核心不变式：只有<b>本会话见过、且落在网格内</b>的那次按下的抬起才产生揭示意图，
/// 且意图作用在<b>抬起所在的格</b>。
/// </summary>
public class GridInputSessionTests
{
    static readonly GridIndex Cell1 = new(1, 1);
    static readonly GridIndex Cell2 = new(2, 2);

    static readonly PointerTarget OnCell1 = new PointerTarget.OnGrid(Cell1);
    static readonly PointerTarget OnCell2 = new PointerTarget.OnGrid(Cell2);
    static readonly PointerTarget Outside = new PointerTarget.OffGrid();

    /// <summary>
    /// 把一串会话步骤跑完：返回最终 session 和这一路上发出的所有意图。
    /// 会话一步的产出是（新 session + 意图），直接链式会被 .Session 隔断 ——
    /// 但"要连走好几步"只发生在测试里（生产端每个鼠标事件只走一步），所以这个 fold 放在测试侧。
    /// </summary>
    static (GridInputSession Session, List<GridInput> Intents) Run(
        params Func<GridInputSession, GridInputOutcome>[] steps)
    {
        var session = GridInputSession.Initial;
        var intents = new List<GridInput>();
        
        foreach (var step in steps)
        {
            var outcome = step(session);
            session = outcome.Session;
            if (outcome.Intent is { } intent) intents.Add(intent);
        }

        return (session, intents);
    }

    [Test]
    public void move_alone_produces_no_intent()
    {
        var trace = GridInputSession.Initial.Move(OnCell1);

        trace.Intent.Should().BeNull();
        trace.Session.Target.Should().Be(OnCell1);
    }

    /// <summary>
    /// 本次线上 bug 的回归：按下发生在上一屏（比如开始菜单的按钮上），
    /// 切场景后新场景只收到了这个抬起 —— 它不属于本场景，不能触发任何东西。
    /// </summary>
    [Test]
    public void release_without_a_press_seen_in_this_scene_produces_no_intent()
    {
        var trace = GridInputSession.Initial.Release(GridButton.Primary, OnCell1);

        trace.Intent.Should().BeNull();
    }

    /// <summary>按下必须落在网格内，否则这次交互不算数（它的抬起也不该生效）。</summary>
    [Test]
    public void press_outside_grid_then_release_inside_produces_no_intent()
    {
        var (session, intents) = Run(
            s => s.Press(GridButton.Primary, Outside),
            s => s.Release(GridButton.Primary, OnCell1));

        intents.Should().BeEmpty();
        session.PressState.Should().Be(new PressState.Idle());
        session.Target.Should().Be(OnCell1); // 指针位置照样跟随
    }

    /// <summary>刻意保留的手感：按错了不松手滑到真正想按的格子，松开生效在那一格。</summary>
    [Test]
    public void release_on_a_different_cell_reveals_that_cell()
    {
        var (session, intents) = Run(
            s => s.Press(GridButton.Primary, OnCell1),
            s => s.Move(OnCell2),
            s => s.Release(GridButton.Primary, OnCell2));

        intents.Should().Equal(new RevealAt(Cell2));
        session.Target.Should().Be(OnCell2);
    }

    [Test]
    public void release_outside_grid_produces_no_intent()
    {
        var trace = GridInputSession.Initial
            .Press(GridButton.Primary, OnCell1)
            .Session.Release(GridButton.Primary, Outside);

        trace.Intent.Should().BeNull();
        trace.Session.PressedPreview.Should().BeNull();
    }

    [Test]
    public void primary_double_click_chords_at_cell()
    {
        var trace = GridInputSession.Initial.Press(GridButton.Primary, OnCell1, isDoubleClick: true);

        trace.Intent.Should().Be(new ChordAt(Cell1));
    }

    /// <summary>插旗是低风险动作：按下即生效，抬起不再做事。</summary>
    [Test]
    public void secondary_press_flags_immediately()
    {
        var (session, intents) = Run(
            s => s.Press(GridButton.Secondary, OnCell1),
            s => s.Release(GridButton.Secondary, OnCell1));

        intents.Should().Equal(new SwitchFlagAt(Cell1));
        session.PressState.Should().Be(new PressState.Idle());
    }

    /// <summary>抬起一个不是"手里那个"的键：这次按下还作数，只是指针跟着走了。</summary>
    [Test]
    public void releasing_a_button_that_is_not_held_keeps_pressing()
    {
        var (session, intents) = Run(
            s => s.Press(GridButton.Primary, OnCell1),
            s => s.Release(GridButton.Secondary, OnCell2));

        intents.Should().BeEmpty();
        session.PressState.Should().BeOfType<PressState.Pressing>();
        session.Target.Should().Be(OnCell2);
        session.PressedPreview.Should().Be(Cell2);
    }

    [Test]
    public void pressed_preview_follows_the_pointer_only_while_pressing()
    {
        var session = GridInputSession.Initial;
        session.PressedPreview.Should().BeNull();

        session = session.Press(GridButton.Primary, OnCell1).Session;
        session.PressedPreview.Should().Be(Cell1);

        session = session.Move(OnCell2).Session;
        session.PressedPreview.Should().Be(Cell2);

        session = session.Release(GridButton.Primary, OnCell2).Session;
        session.PressedPreview.Should().BeNull();
    }
}
