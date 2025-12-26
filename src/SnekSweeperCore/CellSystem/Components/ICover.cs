namespace SnekSweeperCore.CellSystem.Components;

public interface ICover
{
    Task RevealAsync(CancellationToken cancellationToken);
    Task PutOnAsync(CancellationToken cancellationToken);
    void SetAlpha(float normalizedAlpha);
}