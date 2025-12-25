namespace SnekSweeperCore.CellSystem.Components;

public interface ICover
{
    Task RevealAsync();
    Task PutOnAsync();
    void SetAlpha(float normalizedAlpha);
}