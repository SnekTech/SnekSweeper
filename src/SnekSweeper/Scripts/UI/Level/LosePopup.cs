using GTweens.Easings;
using GTweensGodot.Extensions;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class LosePopup : PanelContainer
{
    readonly TaskCompletionSource<PopupChoiceOnLose> _tcs = new();

    public override void _EnterTree()
    {
        RetryButton.Pressed += OnRetryButtonPressed;
        NewGameButton.Pressed += OnNewGameButtonPressed;
        LeaveButton.Pressed += OnLeaveButtonPressed;
    }

    public override void _ExitTree()
    {
        RetryButton.Pressed -= OnRetryButtonPressed;
        NewGameButton.Pressed -= OnNewGameButtonPressed;
        LeaveButton.Pressed -= OnLeaveButtonPressed;
    }

    public override void _Ready()
    {
        Hide();
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

        var choice = await _tcs.Task;

        await this.TweenGlobalPosition(originalGlobalPosition, tweenDuration)
            .SetEasing(Easing.InBack)
            .PlayAsync(ct);
        Hide();

        return choice;
    }

    void OnNewGameButtonPressed()
    {
        _tcs.SetResult(PopupChoiceOnLose.NewGame);
    }

    void OnLeaveButtonPressed()
    {
        _tcs.SetResult(PopupChoiceOnLose.Leave);
    }

    void OnRetryButtonPressed() => _tcs.SetResult(PopupChoiceOnLose.Retry);
}