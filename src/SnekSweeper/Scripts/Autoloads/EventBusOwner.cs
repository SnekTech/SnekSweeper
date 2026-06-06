using SnekSweeper.UI.Level;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.Autoloads;

public static class EventBusOwner
{
    // todo: use AutoInject in Level1 instead of this static instance
    public static GridEventBus GridEventBus { get; private set; } = new();
    public static HUDEventBus HUDEventBus { get; private set; } = new();
}