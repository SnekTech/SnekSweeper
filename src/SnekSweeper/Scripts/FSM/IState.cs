using System.Threading.Tasks;

namespace SnekSweeper.FSM;

public interface IState
{
    Task OnEnterAsync();
    Task OnExitAsync();
}