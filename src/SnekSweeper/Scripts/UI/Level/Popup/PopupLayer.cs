using GodotTask;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level.Popup;

[SceneTree]
public partial class PopupLayer : CanvasLayer
{
    bool IsInputBlocked
    {
        set => InputMask.Visible = value;
    }

    public override void _Ready()
    {
        // todo: block input the right way, this only block mouse inputs now
        IsInputBlocked = false;
    }

    public async GDTask<PopupChoiceOnWin> ShowAndGetChoiceOnWinAsync(CancellationToken ct = default)
    {
        IsInputBlocked = true;
        var choice = await WinPopup.ShowAndGetChoiceAsync(PopupTargetMarker.GlobalPosition, ct);
        IsInputBlocked = false;
        return choice;
    }

    public async GDTask<PopupChoiceOnLose> ShowAndGetChoiceOnLoseAsync(CancellationToken ct = default)
    {
        IsInputBlocked = true;
        var choice = await LosePopup.ShowAndGetChoiceAsync(PopupTargetMarker.GlobalPosition, ct);
        IsInputBlocked = false;
        return choice;
    }

    public void Reset()
    {
        IsInputBlocked = false;
        WinPopup.Hide();
        LosePopup.Hide();
    }
}