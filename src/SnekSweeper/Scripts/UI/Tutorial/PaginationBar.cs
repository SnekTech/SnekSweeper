using GodotGadgets.Extensions;
using GodotGadgets.UI;
using GodotGadgets.UI.Pagination;

namespace SnekSweeper.UI.Tutorial;

[SceneTree]
public partial class PaginationBar : HBoxContainer, IPaginationView
{
    public event Action<PageNav>? NavigationRequested;

    /// <summary>页码文字与导航可用性：同一个快照，一次给全。</summary>
    public void ShowPage(PaginationViewData page)
    {
        PageTextLabel.Text = $"{page.PageNumber} / {page.TotalPages}";

        // 首页/末页 与 上一页/下一页 共用同一条事实（在首页就不存在"上一页"）
        FirstButton.Disabled = !page.HasPreviousPage;
        PrevButton.Disabled = !page.HasPreviousPage;
        NextButton.Disabled = !page.HasNextPage;
        LastButton.Disabled = !page.HasNextPage;
    }

    /// <summary>整屏替换内容；空列表就是清空。</summary>
    public void ShowItems(IReadOnlyList<Control> items)
    {
        _contentContainer?.ClearChildren();
        foreach (var item in items) _contentContainer?.AddChild(item);
    }

    Container? _contentContainer;

    ButtonBindings _buttonBindings = null!;
    IDisposable? _paginationBinder;

    public override void _Ready()
    {
        _buttonBindings = new ButtonBindings(
            FirstButton.BindHandler(() => NavigationRequested?.Invoke(new PageNav.First())),
            PrevButton.BindHandler(() => NavigationRequested?.Invoke(new PageNav.Previous())),
            NextButton.BindHandler(() => NavigationRequested?.Invoke(new PageNav.Next())),
            LastButton.BindHandler(() => NavigationRequested?.Invoke(new PageNav.Last()))
        );
    }

    public void Bind<TItem>(Container contentContainer,
        int pageSize,
        Func<PageRequest, PageResult<TItem>> fetchPage,
        Func<TItem, Control> entryFactory)
    {
        _paginationBinder?.Dispose();
        _contentContainer = contentContainer;
        _paginationBinder = new PaginationBinder<TItem>(this, pageSize, fetchPage, entryFactory);
    }

    public override void _ExitTree()
    {
        _buttonBindings.Dispose();
        _paginationBinder?.Dispose();
    }
}
