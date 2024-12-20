namespace SnekSweeper.FSM;

public interface IState
{
    void OnEnter();
    void OnExit();
}