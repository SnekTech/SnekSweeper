namespace SnekSweeperCore.CellSystem.Components;

public interface ICover
{
    Task RevealAsync(CancellationToken ct = default);
    Task PutOnAsync(CancellationToken ct = default);
    void SetAlpha(float normalizedAlpha);
    void SetStatus(CoverStatus status);
}

public enum CoverStatus
{
    Default,
    Safe,
    Uncertain,
}