namespace SnekSweeperCore.Commands;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
    Task UndoAsync(CancellationToken cancellationToken);
}

public interface ICommandRecorder
{
    Task ExecuteAndRecordAsync(ICommand command, CancellationToken ct = default);
}