namespace SnekSweeperCore.FSM;

public interface IState
{
    Task OnEnterAsync(CancellationToken ct);
    Task OnExitAsync(CancellationToken ct);
}