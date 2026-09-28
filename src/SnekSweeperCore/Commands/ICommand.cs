using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Commands;

// todo: 参数绑定了 grid 就不再通用了，有必要改吗？
public interface ICommand
{
    string Name { get; }
    void Execute(Grid grid);
    void Undo(Grid grid);
}

public interface ICommandRecorder
{
    void ExecuteAndRecord(Grid grid, ICommand command);
}
