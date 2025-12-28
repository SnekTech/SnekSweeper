namespace SnekSweeperCore.Commands;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
    Task UndoAsync();
}