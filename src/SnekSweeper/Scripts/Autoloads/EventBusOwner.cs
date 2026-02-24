using SnekSweeper.UI.Level;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.Autoloads;

public static class EventBusOwner
{
    public static GridEventBus GridEventBus { get; private set; } = new();
    public static HUDEventBus HUDEventBus { get; private set; } = new();
}