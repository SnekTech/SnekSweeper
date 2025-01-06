using System.Threading.Tasks;

namespace SnekSweeper.Commands;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync();
    Task UndoAsync();
}