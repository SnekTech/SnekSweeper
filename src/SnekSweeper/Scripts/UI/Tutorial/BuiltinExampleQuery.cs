using System.Threading.Tasks;
using GodotGadgets.UI.Pagination;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial;

public class BuiltinExampleQuery : IPageQuery<ExampleData>
{
    readonly IReadOnlyList<ExampleData> _examples = TutorialExampleCollection.BuiltinExamples;

    public Task<PageResult<ExampleData>> FetchPageAsync(PageRequest request, CancellationToken ct = default) =>
        Task.FromResult(_examples.SlicePage(request));
}