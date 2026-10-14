using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.Widgets;
using SnekSweeperCore.SaveLoad;
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
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    bool ShouldShowComboInSetting => SaveData.State.MainSetting.ComboRankDisplay;

    public void OnResolved()
    {
        RefreshDisplay();
    }

    public override void _Process(double delta)
    {
        _gameTime += delta;
        RefreshDisplay();
    }

    public void IncreaseCombo()
    {
        _state = ComboDefault.increment(_gameTime, _state);
        RefreshDisplay();
    }

    public void Reset()
    {
        _state = ComboDefault.initial;
        _gameTime = 0;
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        var tier = ComboDefault.getTier(ComboDefault.levelAt(_gameTime, _state));
        _.ComboLevelTextLabel.Text = ToDisplayText(tier);
        _.ComboProgressBar.Value = ComboDefault.progressRatio(_gameTime, _state) * _.ComboProgressBar.MaxValue;
        Visible = ShouldShowComboInSetting && !tier.Equals(ComboTier.None);
    }

    static readonly IReadOnlyDictionary<ComboTier, string> TierText = new Dictionary<ComboTier, string>
    {
        [ComboTier.Good] = "Good",
        [ComboTier.Great] = "Great",
        [ComboTier.Excellent] = "Excellent",
    };

    static string ToDisplayText(ComboTier tier) => TierText.GetValueOrDefault(tier) ?? "";
}