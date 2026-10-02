using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Commands;

// todo: 参数绑定了 grid 就不再通用了，有必要改吗？
public interface ICommand
{
    string Name { get; }
    GridOutcome Execute(Grid grid);
    GridOutcome Undo(Grid grid);
}

public interface ICommandRecorder
{
    GridOutcome ExecuteAndRecord(Grid grid, ICommand command);
}
