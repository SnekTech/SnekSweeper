using GTweens.Easings;
using GTweensGodot.Extensions;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class LosePopup : PanelContainer
{
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

        var choice = await _.ChoiceListener.GetChoiceAsync();

        await this.TweenGlobalPosition(originalGlobalPosition, tweenDuration)
            .SetEasing(Easing.InBack)
            .PlayAsync(ct);
        Hide();

        return choice;
    }
}