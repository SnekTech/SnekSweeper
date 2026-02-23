using GTweens.Easings;
using GTweensGodot.Extensions;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class LosePopup : PanelContainer
{
    PopupChoiceListener<PopupChoiceOnLose> _popupChoiceListener = null!;

    public override void _EnterTree()
    {
        Hide();
        _popupChoiceListener = CreatePopupChoiceListener();

        _popupChoiceListener.RegisterButtonListeners();
    }

    public override void _ExitTree()
    {
        _popupChoiceListener.UnregisterButtonListeners();
    }

    PopupChoiceListener<PopupChoiceOnLose> CreatePopupChoiceListener()
    {
        var buttons = new List<ButtonAndValue<PopupChoiceOnLose>>();
        buttons.AddRange([
            NewGameButton.CreateChoiceButton(PopupChoiceOnLose.NewGame),
            LeaveButton.CreateChoiceButton(PopupChoiceOnLose.Leave),
            RetryButton.CreateChoiceButton(PopupChoiceOnLose.Retry),
        ]);
        return new PopupChoiceListener<PopupChoiceOnLose>(buttons);
    }

    public async Task<PopupChoiceOnLose> ShowAndGetChoiceAsync(Vector2 targetGlobalPosition,
        CancellationToken ct = default)
    {
        Show();
        var originalGlobalPosition = GlobalPosition;
        const float tweenDuration = 0.6f;
        await this.TweenGlobalPosition(targetGlobalPosition, tweenDuration)
            .SetEasing(Easing.OutBack)
            .PlayAsync(ct);

        var choice = await _popupChoiceListener.GetChoiceAsync();

        await this.TweenGlobalPosition(originalGlobalPosition, tweenDuration)
            .SetEasing(Easing.InBack)
            .PlayAsync(ct);
        Hide();

        return choice;
    }
}