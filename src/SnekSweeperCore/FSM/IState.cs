namespace SnekSweeperCore.FSM;

public interface IState
{
    Task OnEnterAsync();
    Task OnExitAsync();
}