using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.Autoloads;
using SnekSweeper.Levels;
using SnekSweeper.Widgets;
using SnekSweeperCore.ComboSystem;

namespace SnekSweeper.Combo;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class ComboRankCard : VBoxContainer, ISceneScript
{
    readonly ComboCounter _counter = new(ComboConfig.Default);

    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    LevelData LevelData => this.DependOn<LevelData>();

    public void OnResolved()
    {
        LevelData.GridEventBus.BatchRevealed += OnBatchRevealed;
    }

    public void OnExitTree()
    {
        LevelData.GridEventBus.BatchRevealed -= OnBatchRevealed;
    }

    public override void _Ready()
    {
        if (!HouseKeeper.MainSetting.ComboRankDisplay)
        {
            Hide();
            return;
        }

        RefreshDisplay();
    }

    public override void _Process(double delta)
    {
        _counter.Update((float)delta);
        RefreshDisplay();
    }

    void OnBatchRevealed()
    {
        _counter.Increment();
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        var tier = ComboCounter.GetTier(_counter.Level);
        _.ComboLevelTextLabel.Text = ToDisplayText(tier);
        _.ComboProgressBar.Value = _counter.ProgressRatio * _.ComboProgressBar.MaxValue;
        Visible = tier != ComboTier.None;
    }

    static string ToDisplayText(ComboTier tier) => tier switch
    {
        ComboTier.Good      => "Good",
        ComboTier.Great     => "Great",
        ComboTier.Excellent => "Excellent",
        _                   => "",
    };
}
