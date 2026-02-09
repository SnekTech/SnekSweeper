using GTweens.Easings;
using GTweensGodot.Extensions;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class WinPopup : PanelContainer
{
    readonly TaskCompletionSource<PopupChoiceOnWin> _tcs = new();

    public override void _EnterTree()
    {
        NewGameButton.Pressed += OnNewGameButtonPressed;
        LeaveButton.Pressed += OnLeaveButtonPressed;
    }

    public override void _ExitTree()
    {
        NewGameButton.Pressed -= OnNewGameButtonPressed;
        LeaveButton.Pressed -= OnLeaveButtonPressed;
    }

    public override void _Ready()
    {
        Hide();
    }

    public async Task<PopupChoiceOnWin> ShowAndGetChoiceAsync(Vector2 targetGlobalPosition,
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
        _tcs.SetResult(new NewGame());
    }

    void OnLeaveButtonPressed()
    {
        _tcs.SetResult(new Leave());
    }
}