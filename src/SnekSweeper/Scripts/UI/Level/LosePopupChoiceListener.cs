using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

public partial class LosePopupChoiceListener : Node
{
    [Export]
    Button _newGame = null!;

    [Export]
    Button _retry = null!;

    [Export]
    Button _leave = null!;

    readonly TaskCompletionSource<PopupChoiceOnLose> _tcs = new();

    public override void _EnterTree()
    {
        _retry.Pressed += OnRetryButtonPressed;
        _newGame.Pressed += OnNewGameButtonPressed;
        _leave.Pressed += OnLeaveButtonPressed;
    }

    public override void _ExitTree()
    {
        _retry.Pressed -= OnRetryButtonPressed;
        _newGame.Pressed -= OnNewGameButtonPressed;
        _leave.Pressed -= OnLeaveButtonPressed;
    }

    public Task<PopupChoiceOnLose> GetChoiceAsync() => _tcs.Task;

    void OnNewGameButtonPressed() => _tcs.SetResult(PopupChoiceOnLose.NewGame);
    void OnLeaveButtonPressed() => _tcs.SetResult(PopupChoiceOnLose.Leave);
    void OnRetryButtonPressed() => _tcs.SetResult(PopupChoiceOnLose.Retry);
}