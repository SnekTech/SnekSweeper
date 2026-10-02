using SnekSweeperCore.CellSystem;

namespace SnekSweeper.CellSystem;

/// <summary>表现层把一次格子的变化呈现出来</summary>
public interface ICellRenderer
{
    void Render(CellOutcome outcome);
}

public static class CellRendererExtensions
{
    extension(ICellRenderer renderer)
    {
        public void RenderSome(IEnumerable<CellOutcome> outcomes)
        {
            foreach (var cellOutcome in outcomes)
            {
                renderer.Render(cellOutcome);
            }
        }
    }
}