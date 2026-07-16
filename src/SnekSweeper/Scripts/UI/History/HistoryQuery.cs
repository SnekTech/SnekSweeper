using System.Threading.Tasks;
using GodotGadgets.UI.Pagination;
using SnekSweeperCore.GameHistory;

namespace SnekSweeper.UI.History;

public class HistoryQuery(IReadOnlyList<GameRunRecord> records) : IPageQuery<GameRunRecord>
{
    public Task<PageResult<GameRunRecord>> FetchPageAsync(PageRequest request, CancellationToken ct = default)
    {
        var (pageIndex, pageSize) = request;
        var items = records.Skip(pageIndex * pageSize).Take(pageSize).ToArray();
        return Task.FromResult(new PageResult<GameRunRecord>(items, records.Count));
    }
}