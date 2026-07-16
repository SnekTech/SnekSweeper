using System.Threading.Tasks;
using GodotGadgets.UI.Pagination;
using SnekSweeperCore.GameHistory;

namespace SnekSweeper.UI.History;

public class HistoryQuery(IReadOnlyList<GameRunRecord> records) : IPageQuery<GameRunRecord>
{
    public Task<PageResult<GameRunRecord>> FetchPageAsync(PageRequest request, CancellationToken ct = default) =>
        Task.FromResult(records.SlicePage(request));
}