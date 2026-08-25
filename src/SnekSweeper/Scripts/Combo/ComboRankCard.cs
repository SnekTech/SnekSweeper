using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.Autoloads;
using SnekSweeper.Levels;
using SnekSweeper.Widgets;
using CoreFS.ComboDomain;

namespace SnekSweeper.Combo;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class ComboRankCard : VBoxContainer, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    ComboState _state = ComboDefault.initial;
    double _gameTime;

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
        if (!SaveData.MainSetting.ComboRankDisplay)
        {
            Hide();
            return;
        }

        RefreshDisplay();
    }

    public override void _Process(double delta)
    {
        _gameTime += delta;
        RefreshDisplay();
    }

    void OnBatchRevealed()
    {
        _state = ComboDefault.increment(_gameTime, _state);
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        var tier = ComboDefault.getTier(ComboDefault.levelAt(_gameTime, _state));
        _.ComboLevelTextLabel.Text = ToDisplayText(tier);
        _.ComboProgressBar.Value = ComboDefault.progressRatio(_gameTime, _state) * _.ComboProgressBar.MaxValue;
        Visible = !tier.Equals(ComboTier.None);
    }

    static readonly IReadOnlyDictionary<ComboTier, string> TierText = new Dictionary<ComboTier, string>
    {
        [ComboTier.Good] = "Good",
        [ComboTier.Great] = "Great",
        [ComboTier.Excellent] = "Excellent",
    };

    static string ToDisplayText(ComboTier tier) => TierText.GetValueOrDefault(tier) ?? "";
}