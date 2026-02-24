using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class PopupLayer : CanvasLayer
{
    bool IsInputBlocked
    {
        set => InputMask.Visible = value;
    }

    public override void _Ready()
    {
        IsInputBlocked = false;
    }

    public async Task<PopupChoiceOnWin> ShowAndGetChoiceOnWinAsync(CancellationToken ct = default)
    {
        IsInputBlocked = true;
        var choice = await WinPopup.ShowAndGetChoiceAsync(PopupTargetMarker.GlobalPosition, ct);
        IsInputBlocked = false;
        return choice;
    }

    public async Task<PopupChoiceOnLose> ShowAndGetChoiceOnLoseAsync(CancellationToken ct = default)
    {
        IsInputBlocked = true;
        var choice = await LosePopup.ShowAndGetChoiceAsync(PopupTargetMarker.GlobalPosition, ct);
        IsInputBlocked = false;
        return choice;
    }
}