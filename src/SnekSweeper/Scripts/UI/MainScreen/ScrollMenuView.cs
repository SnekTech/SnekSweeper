using System.Runtime.CompilerServices;
using GodotGadgets.Extensions;
using GodotGadgets.Tasks;
using GodotGadgets.UI.ScrollMenuCore;

namespace SnekSweeper.UI.MainScreen;

[SceneTree]
public partial class ScrollMenuView : Control
{
    const float SlotHeight = 48;

    ScrollMenu _menu = null!;
    ScrollMenuViewAnimator _animator = null!;
    ScrollMenuInputMapper _inputMapper = null!;
    CancellationTokenSource? _tweenCts;
    IReadOnlyList<Control> _itemNodes = [];
    Dictionary<IScrollMenuItem, Action> _bindings = null!;

    public void Init(IReadOnlyList<MenuItemBinding> bindings)
    {
        ItemsContainer.ClearChildren();
        
        _itemNodes =
        [
            .. bindings.Select(b =>
            {
                var node = b.Item as Control
                           ?? throw new ArgumentException(
                               $"Item '{b.Item.GetType().Name}' must be a Control to participate in scroll menu layout.");
                return node;
            }),
        ];

        foreach (var control in _itemNodes)
        {
            ItemsContainer.AddChild(control);
        }

        var items = bindings.Select(b => b.Item).ToArray();
        
        var config = new ScrollMenuConfig(items, visibleCount: 3);
        _menu = new ScrollMenu(config);
        _animator = new ScrollMenuViewAnimator(SlotHeight);
        _bindings = bindings.ToDictionary(b => b.Item, b => b.OnConfirm);

        PlaceAllItems(items);

        _inputMapper = new ScrollMenuInputMapper(HitTest);
        ApplyWindow(_menu.VisibleWindow, NavigateDirection.Down); // up or down doesn't matter here
    }

    public override void _ExitTree()
    {
        _tweenCts?.CancelAndDispose();
    }

    public override void _Input(InputEvent @event)
    {
        if (_inputMapper.Map(@event) is not { } command) return;
        Apply(_menu.Handle(command));
    }

    void Apply(ScrollMenuEffect effect)
    {
        switch (effect)
        {
            case FocusMoved moved:
                ApplyWindow(moved.Window, moved.Direction);
                break;
            case ConfirmRequested confirm:
                RunBinding(confirm.Item);
                break;
            default:
                throw new SwitchExpressionException();
        }
    }

    void RunBinding(IScrollMenuItem item)
    {
        if (_bindings.TryGetValue(item, out var action))
        {
            action();
        }
    }

    IScrollMenuItem? HitTest(Vector2 viewportPosition)
    {
        foreach (var node in _itemNodes)
        {
            if (node.GetGlobalRect().HasPoint(viewportPosition))
            {
                return (IScrollMenuItem)node;
            }
        }

        return null;
    }

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

    void ApplyWindow(IReadOnlyList<VisibleSlot> window, NavigateDirection direction)
    {
        _tweenCts?.CancelAndDispose();
        _tweenCts = new CancellationTokenSource();
        _animator.Apply(window, direction, _itemNodes, _tweenCts.Token);
    }
}
