using System.Runtime.CompilerServices;
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

        var itemsWithIndex = window.Select((s, i) => (s.Item, i))
            .ToDictionary(tuple => tuple.Item, tuple => tuple.i);

        foreach (var item in ItemsContainer.GetChildrenOfType<IScrollMenuItem>())
        {
            var node = (Control)item;

            var itemInWindow = itemsWithIndex.ContainsKey(item);
            if (itemInWindow)
            {
                var slotIndex = itemsWithIndex[item];
                var targetY = slotIndex * SlotHeight;
                TweenY(node, targetY, ct);

                node.Modulate = window[slotIndex].Prominence switch
                {
                    ItemProminence.Focused => Colors.White,
                    ItemProminence.Adjacent => Colors.White with { A = 0.5f },
                    _ => throw new SwitchExpressionException(),
                };
            }
            else
            {
                // todo: start y of the off screen item coming alive should depend on navigate direction
                var offScreenY = direction == NavigateDirection.Down
                    ? -SlotHeight
                    : window.Count * SlotHeight;
                TweenY(node, offScreenY, ct);
                node.Modulate = Colors.Transparent;
            }
        }
    }

    void TweenY(Control node, float targetY, CancellationToken ct)
    {
        node.TweenPositionY(targetY, TweenDuration)
            .PlayAsyncUntilNodeDestroy(this, ct)
            .Forget();
    }
}