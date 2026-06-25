using GodotGadgets.Extensions;
using GodotGadgets.UI;
using GodotGadgets.UI.Pagination;
using SnekSweeper.UI.Tutorial.Example;
using SnekSweeper.Widgets;
using SnekSweeperCore.SkinSystem;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial;

[SceneTree(root: "ROOT")]
public partial class ExamplePagination : Control, IPaginationUI
{
    const int ExamplePageSize = 1;

    public event Action? FirstPageRequested;
    public event Action? PreviousPageRequested;
    public event Action? NextPageRequested;
    public event Action? LastPageRequested;

    public void SetPageText(int currentPage, int totalPages)
    {
        PageTextLabel.Text = $"{currentPage} / {totalPages}";
    }

    public void SetNavigationEnabled(bool canGoFirst, bool canGoPrevious, bool canGoNext, bool canGoLast)
    {
        FirstButton.Disabled = !canGoFirst;
        PrevButton.Disabled = !canGoPrevious;
        NextButton.Disabled = !canGoNext;
        LastButton.Disabled = !canGoLast;
    }

    public void ClearContent() => _examplesContainer.ClearChildren();
    public void AddContentItem(Control item) => _examplesContainer.AddChild(item);

    Container _examplesContainer = null!;

    ButtonBindings _buttonBindings = null!;
    PaginationBinder<ExampleData> _paginationBinder = null!;

    public override void _Ready()
    {
        _buttonBindings = new ButtonBindings(
            FirstButton.BindHandler(() => FirstPageRequested?.Invoke()),
            PrevButton.BindHandler(() => PreviousPageRequested?.Invoke()),
            NextButton.BindHandler(() => NextPageRequested?.Invoke()),
            LastButton.BindHandler(() => LastPageRequested?.Invoke())
        );
    }

    public void Init(GridSkin skin, Container examplesContainer)
    {
        _examplesContainer = examplesContainer;

        var pagination = new Pagination<ExampleData>(new BuiltinExampleQuery(), ExamplePageSize);
        _paginationBinder = new PaginationBinder<ExampleData>(
            this,
            pagination,
            _ =>
            {
                var card = ExampleCard.Instantiate();
                card.Skin = skin;
                return card;
            },
            error => GD.PrintErr($"[Pagination Error] {error}"));
    }

    public override void _ExitTree()
    {
        _buttonBindings.Dispose();
        _paginationBinder.Dispose();
    }
}