using SnekSweeper.Autoloads;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.GameStateManagement;

public interface IAppRepo : IDisposable
{
    event Action? GameEnded;
    void InvokeGameEnded();
    GridSkin CurrentSkin { get; }
}

public sealed class AppRepo : IAppRepo
{
    public event Action? GameEnded;

    public void InvokeGameEnded() => GameEnded?.Invoke();

    public GridSkin CurrentSkin => HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();

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