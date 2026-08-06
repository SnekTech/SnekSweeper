using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.Autoloads;
using SnekSweeper.Levels;

namespace SnekSweeper.Combo;


[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class ComboRankCard : VBoxContainer
{
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
        InitComboDisplay();
    }

    void OnBatchRevealed() => GridComboComponent.IncreaseComboLevel();

    void InitComboDisplay()
    {
        if (HouseKeeper.MainSetting.ComboRankDisplay)
        {
            GridComboComponent.ComboDisplay = new BasicComboDisplay(ComboLevelTextLabel, ComboProgressBar);
        }
        else
        {
            Hide();
        }
    }
}