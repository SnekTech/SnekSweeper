using SnekSweeper.GridSystem;

namespace SnekSweeper.Autoloads;

public static class EventBusOwner
{
    public static GridEventBus GridEventBus { get; private set; } = new();
}