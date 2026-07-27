using GodotGadgets.Extensions;
using GodotGadgets.Tasks;
using GodotGadgets.UI.ScrollMenuCore;
using GodotTask;
using GTweensGodot.Extensions;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.MainScreen;

[SceneTree]
public partial class ScrollMenuView : Control
{
    const float SlotHeight = 48;
    const float TweenDuration = 0.2f;

    ScrollMenu _menu = null!;
    CancellationTokenSource? _tweenCts;
    IReadOnlyList<VisibleSlot>? _previousWindow;

    public override void _Ready()
    {
        var items = ItemsContainer.GetChildrenOfType<IScrollMenuItem>().ToArray();
        var config = new ScrollMenuConfig(items, visibleCount: 3);
        _menu = new ScrollMenu(config);

        PlaceAllItems(items);

        _menu.FocusChanged += OnMenuFocusChanged;
        ApplyWindow(_menu.VisibleWindow, NavigateDirection.Down); // up or down doesn't matter here
    }

    public override void _ExitTree()
    {
        _tweenCts?.CancelAndDispose();
        _menu.FocusChanged -= OnMenuFocusChanged;
    }

    // todo: custom input handling strategy for mouse | keyboard | joypad 
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_up"))
        {
            _menu.NavigateUp();
        }
        else if (@event.IsActionPressed("ui_down"))
        {
            _menu.NavigateDown();
        }
        else if (@event.IsActionPressed("ui_accept"))
        {
            ConfirmFocused();
        }
    }

    void ConfirmFocused()
    {
        if (_menu.CurrentFocused is Button button)
        {
            button.EmitSignal(BaseButton.SignalName.Pressed);
        }
    }

    void OnMenuFocusChanged(IReadOnlyList<VisibleSlot> window, NavigateDirection direction) =>
        ApplyWindow(window, direction);

    void PlaceAllItems(IReadOnlyList<IScrollMenuItem> items)
    {
        var itemsWithInitialPosition = items.Select((item, i) =>
        {
            var itemControl = (Control)item;

            // center the item horizontally
            var x = (ItemsContainer.Size.X - itemControl.Size.X) / 2;
            var y = i * SlotHeight;
            return (controlNode: itemControl, new Vector2(x, y));
        });
        foreach (var (itemControl, initialPosition) in itemsWithInitialPosition)
        {
            itemControl.Position = initialPosition;
        }
    }

    /// <summary>
    /// Animate items in response to a navigation event.
    ///
    /// Items in the visible window tween to their slot Y positions.
    /// Items leaving the window tween off-screen (up or down depending on
    /// navigation direction) and fade to transparent.
    /// Items entering the window emerge from the opposite edge and fade in.
    /// </summary>
    void ApplyWindow(IReadOnlyList<VisibleSlot> window, NavigateDirection direction)
    {
        _tweenCts?.CancelAndDispose();
        _tweenCts = new CancellationTokenSource();
        var ct = _tweenCts.Token;

        var slotted = window.Index().ToDictionary(x => x.Item.Item, x => new SlottedItem(x.Index, x.Item.Prominence));
        var previousItemSet = _previousWindow?.Select(s => s.Item).ToHashSet() ?? [];

        var actions = ItemsContainer.GetChildrenOfType<IScrollMenuItem>()
            .Select(item => Classify((Control)item, slotted, previousItemSet, window.Count, direction))
            .ToList();

        foreach (var a in actions.Where(a => a.SnapToY is not null))
        {
            a.Node.Position = a.Node.Position with { Y = a.SnapToY!.Value };
        }

        foreach (var a in actions)
        {
            TweenY(a.Node, a.TargetY, ct);
            a.Node.Modulate = a.Node.Modulate with { A = a.Alpha };
        }

        _previousWindow = window;
    }

    readonly record struct ItemAction(Control Node, float TargetY, float Alpha, float? SnapToY);

    /// <summary>
    /// The position and visual role of an item within the current visible window.
    /// Pure data — computed in ApplyWindow, consumed by Classify.
    /// </summary>
    readonly record struct SlottedItem(int Index, ItemProminence Prominence);

    static ItemAction Classify(
        Control node,
        Dictionary<IScrollMenuItem, SlottedItem> slotted,
        HashSet<IScrollMenuItem> previousSet,
        int visibleCount,
        NavigateDirection direction
    )
    {
        var item = (IScrollMenuItem)node;

        // in window
        if (slotted.TryGetValue(item, out var slot))
        {
            var targetY = slot.Index * SlotHeight;
            var alpha = slot.Prominence == ItemProminence.Focused ? 1f : 0.5f;

            // Newly entering from outside: snap to the opposite edge
            // so the tween crosses the screen, carrying the scroll illusion.
            // No snap on first render: previousSet is empty for all items.
            var snap = ShouldSnap() ? EntryEdge() : (float?)null;

            return new ItemAction(node, targetY, alpha, snap);
        }

        // just left the window
        if (previousSet.Contains(item))
        {
            return new ItemAction(node, ExitEdge(), 0f, null);
        }

        // never entered, or already outside
        return new ItemAction(node, node.Position.Y, 0f, null);

        bool ShouldSnap() => previousSet.Count > 0 && !previousSet.Contains(item);
        float EntryEdge() => direction == NavigateDirection.Down ? visibleCount * SlotHeight : -SlotHeight;
        float ExitEdge() => direction == NavigateDirection.Down ? -SlotHeight : visibleCount * SlotHeight;
    }

    void TweenY(Control node, float targetY, CancellationToken ct)
    {
        node.TweenPositionY(targetY, TweenDuration)
            .PlayAsyncUntilNodeDestroy(this, ct)
            .Forget();
    }
}