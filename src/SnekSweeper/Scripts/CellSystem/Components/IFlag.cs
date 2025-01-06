using System.Threading.Tasks;

namespace SnekSweeper.CellSystem.Components;

public interface IFlag
{
    Task RaiseAsync();
    Task PutDownAsync();
}