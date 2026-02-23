namespace SnekSweeperCore.CellSystem.Components;

public interface ICover
{
    Task RevealAsync(CancellationToken ct = default);
    Task PutOnAsync(CancellationToken ct = default);
    void SetAlpha(float normalizedAlpha);
}