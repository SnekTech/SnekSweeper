namespace SnekSweeperCore.CellSystem.Components;

public interface IFlag
{
    Task RaiseAsync(CancellationToken ct = default);
    Task PutDownAsync(CancellationToken ct = default);
}