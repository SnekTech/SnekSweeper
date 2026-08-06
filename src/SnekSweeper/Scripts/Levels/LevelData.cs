using SnekSweeperCore.Commands;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.Levels;

public sealed record LevelData(GridEventBus GridEventBus, CommandInvoker GridCommandInvoker);