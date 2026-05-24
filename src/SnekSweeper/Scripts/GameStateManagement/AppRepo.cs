namespace SnekSweeper.GameStateManagement;

public interface IAppRepo : IDisposable
{
    event Action? GameEnded;
    void InvokeGameEnded();
}

public sealed class AppRepo : IAppRepo
{
    public event Action? GameEnded;

    public void InvokeGameEnded() => GameEnded?.Invoke();

    #region Disposable

    bool _disposed;

    void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed objects.
            GameEnded = null;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ~AppRepo() => Dispose(disposing: false);

    #endregion
}