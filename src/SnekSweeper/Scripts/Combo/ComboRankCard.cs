using SnekSweeper.Autoloads;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.Combo;

[SceneTree]
public partial class ComboRankCard : VBoxContainer
{
    private readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;

    public override void _EnterTree()
    {
        _gridEventBus.BatchRevealed += OnBatchRevealed;
    }

    public override void _ExitTree()
    {
        _gridEventBus.BatchRevealed -= OnBatchRevealed;
    }

    public override void _Ready()
    {
        InitComboDisplay();
    }

    private void OnBatchRevealed() => GridComboComponent.IncreaseComboLevel();

    private void InitComboDisplay()
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