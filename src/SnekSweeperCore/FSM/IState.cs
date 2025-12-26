namespace SnekSweeperCore.FSM;

public interface IState
{
    Task OnEnterAsync(CancellationToken cancellationToken);
    Task OnExitAsync(CancellationToken cancellationToken);
}