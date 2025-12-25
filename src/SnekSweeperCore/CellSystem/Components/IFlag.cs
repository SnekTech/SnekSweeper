namespace SnekSweeperCore.CellSystem.Components;

public interface IFlag
{
    Task RaiseAsync();
    Task PutDownAsync();
}