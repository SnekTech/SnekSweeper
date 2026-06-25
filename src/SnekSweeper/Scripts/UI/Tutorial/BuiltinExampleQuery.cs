using System.Threading.Tasks;
using GodotGadgets.UI.Pagination;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial;

public class BuiltinExampleQuery : IPageQuery<ExampleData>
{
    readonly IReadOnlyList<ExampleData> _examples = TutorialExampleCollection.BuiltinExamples;

    public Task<PageResult<ExampleData>> FetchPageAsync(PageRequest request, CancellationToken ct = default)
    {
        var pageIndex = Math.Clamp(request.PageIndex, 0, int.MaxValue);
        var pageSize = Math.Clamp(request.PageSize, 1, int.MaxValue);

        var items = _examples
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToList().AsReadOnly();
        return Task.FromResult(new PageResult<ExampleData>(items, _examples.Count));
    }
}