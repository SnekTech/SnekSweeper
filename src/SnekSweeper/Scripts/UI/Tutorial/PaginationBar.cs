using GodotGadgets.Extensions;
using GodotGadgets.UI;
using GodotGadgets.UI.Pagination;

namespace SnekSweeper.UI.Tutorial;

[SceneTree]
public partial class PaginationBar : HBoxContainer, IPaginationUI
{
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

    public void ClearContent() => _contentContainer?.ClearChildren();
    public void AddContentItem(Control item) => _contentContainer?.AddChild(item);

    Container? _contentContainer;

    ButtonBindings _buttonBindings = null!;
    IDisposable? _paginationBinder;

    public override void _Ready()
    {
        _buttonBindings = new ButtonBindings(
            FirstButton.BindHandler(() => FirstPageRequested?.Invoke()),
            PrevButton.BindHandler(() => PreviousPageRequested?.Invoke()),
            NextButton.BindHandler(() => NextPageRequested?.Invoke()),
            LastButton.BindHandler(() => LastPageRequested?.Invoke())
        );
    }

    public void Bind<TItem>(Container contentContainer,
        Func<TItem, Control> entryFactory,
        Pagination<TItem> pagination)
    {
        _paginationBinder?.Dispose();
        _contentContainer = contentContainer;
        _paginationBinder = new PaginationBinder<TItem>(this, pagination, entryFactory);
    }

    public override void _ExitTree()
    {
        _buttonBindings.Dispose();
        _paginationBinder?.Dispose();
    }
}