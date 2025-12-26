namespace SnekSweeperCore.CellSystem.Components;

public interface IFlag
{
    Task RaiseAsync(CancellationToken cancellationToken);
    Task PutDownAsync(CancellationToken cancellationToken);
}