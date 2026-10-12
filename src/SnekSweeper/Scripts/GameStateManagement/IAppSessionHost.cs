using SnekSweeperCore.AppFlow;

namespace SnekSweeper.GameStateManagement;

public interface IAppSessionHost
{
    void Send(AppSession.Event evt);
}