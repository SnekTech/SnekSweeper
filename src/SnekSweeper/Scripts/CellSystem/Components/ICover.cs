using System.Threading.Tasks;

namespace SnekSweeper.CellSystem.Components;

public interface ICover
{
    Task RevealAsync();
    Task PutOnAsync();
}