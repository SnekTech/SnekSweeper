using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class LosePopup : PanelContainer
{
    PopupChoiceListener<PopupChoiceOnLose> _popupChoiceListener = null!;
    PopupAnimator _animator = null!;

    public override void _EnterTree()
    {
        Hide();
        _animator = new PopupAnimator(this);
        _popupChoiceListener = CreatePopupChoiceListener();
        _popupChoiceListener.RegisterButtonListeners();
    }

    public override void _ExitTree()
    {
        _popupChoiceListener.UnregisterButtonListeners();
    }

    public async Task<PopupChoiceOnLose> ShowAndGetChoiceAsync(Vector2 targetGlobalPosition,
        CancellationToken ct = default)
    {
        await _animator.ShowAsync(targetGlobalPosition, ct);
        var choice = await _popupChoiceListener.GetChoiceAsync();
        await _animator.HideAsync(ct);

        return choice;
    }

    PopupChoiceListener<PopupChoiceOnLose> CreatePopupChoiceListener() =>
        new([
            NewGameButton.CreateChoiceButton(PopupChoiceOnLose.NewGame),
            LeaveButton.CreateChoiceButton(PopupChoiceOnLose.Leave),
            RetryButton.CreateChoiceButton(PopupChoiceOnLose.Retry),
        ]);
}