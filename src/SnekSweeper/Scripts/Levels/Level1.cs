using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using GodotGadgets.Tasks;
using SnekSweeper.GameStateManagement;
using SnekSweeper.GridSystem.State;
using SnekSweeper.Widgets;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;
using GridState = SnekSweeper.GridSystem.State.GridState;

namespace SnekSweeper.Levels;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class Level1 : Node2D,
    IProvide<LevelData>,
    ISceneScript, ILevelOrchestrator
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();

    [Dependency]
    IAppRepo AppRepo => this.DependOn<IAppRepo>();

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    GridLogic GridLogic { get; set; } = null!;
    LogicBlock.Binding GridBinding { get; set; } = null!;

    LevelData _levelData = null!;
    LevelData IProvide<LevelData>.Value() => _levelData;

    public override void _EnterTree()
    {
        _levelData = new LevelData(new CommandInvoker());
        this.Provide();

        TheGrid.GridInputListener.GridInputEmitted += OnGridInputEmitted;
    }

    public override void _ExitTree()
    {
        SaveData.NotifySaved();

        TheGrid.GridInputListener.GridInputEmitted -= OnGridInputEmitted;

        HUD.UndoRequested -= OnUndoRequested;

        GridLogic.Stop();
        GridBinding.Dispose();
    }

    public void LoadLevel(LoadLevelSource loadLevelSource)
    {
        var grid = CreateGrid();
        TheGrid.Init(grid.Size);

        SetupGridLogic();
        SetupGridBinding();

        HUD.UndoRequested += OnUndoRequested;

        GridLogic.Start<GridState.PreInstantiated>();
        GridLogic.Input(new GridState.Input.Init(loadLevelSource));

        return;

        Grid CreateGrid()
        {
            // todo: commandInvoker 相关设计是不是需要重构
            var newGrid = loadLevelSource.CreateGrid(_levelData.GridCommandInvoker);
            TheGrid.InstantiateCells(newGrid.Size, SaveData.CurrentSkin);
            return newGrid;
        }

        void SetupGridLogic()
        {
            GridLogic = new GridLogic();

            GridLogic.Set(new GridLogic.Data
            {
                AppRepo = AppRepo,
                CancellationTokenOnLevelExit = this.GetCancellationTokenOnTreeExit(),
            });

            GridLogic.Set(new GridStateContext(
                grid,
                TheGrid,
                new GameRunRecorder(SaveData),
                this,
                _levelData.GridCommandInvoker
            ));
        }

        void SetupGridBinding()
        {
            GridBinding = GridLogic.Bind()
                .OnOutput((in GridState.Output.RestoreGrid output) => { RestoreGrid(output.Snapshot); })
                .OnOutput((in GridState.Output.LayMinesAt output) => { LayMines(output.Source, output.FirstInput); })
                .OnOutput((in GridState.Output.ProcessInput output) => { HandleInput(output.GridInput); })
                .OnOutput((in GridState.Output.EndGameChoiceOnWin output) =>
                {
                    Action handleChoiceAction = output.Choice switch
                    {
                        PopupChoiceOnWin.NewGame => NewGame,
                        PopupChoiceOnWin.Leave => BackToMainMenu,
                        _ => delegate { },
                    };
                    handleChoiceAction();
                })
                .OnOutput((in GridState.Output.EndGameChoiceOnLose output) =>
                {
                    var recentRecord = output.RecentRecord;
                    Action handleChoiceAction = output.Choice switch
                    {
                        PopupChoiceOnLose.Retry => () => Retry(recentRecord),
                        PopupChoiceOnLose.NewGame => NewGame,
                        PopupChoiceOnLose.Leave => BackToMainMenu,
                        _ => delegate { },
                    };
                    handleChoiceAction();
                });

            return;

            // 初始化放在绑定层：续局恢复完整棋盘状态，新局/重试按首次点击布雷（两者都是同步的）
            void RestoreGrid(GridSnapshot snapshot)
            {
                grid.RestoreCellStates(snapshot);
                CompleteInit();
            }

            void LayMines(LoadLevelSource source, GridInput firstInput)
            {
                grid.InitCells(source.LayMineFn(firstInput.Index));
                CompleteInit();
            }

            void CompleteInit()
            {
                TheGrid.Paint(grid);
                HUD.UpdateBombCount(grid.BombCount);
                HUD.UpdateFlagCount(grid.FlagCount);
                GridLogic.Input(new GridState.Input.InitCompleted());
            }

            // 输入处理放在绑定层：FSM 只发效果；处理本身是纯同步的，结果直接打回 FSM
            // todo: 都是同步，是不是没必要传出来再传进去？
            void HandleInput(GridInput gridInput)
            {
                var outcome = grid.HandleInput(gridInput);
                TheGrid.ApplyGridOutcome(outcome);
                
                HUD.UpdateFlagCount(grid.FlagCount);

                if (outcome is GridOutcome.BatchRevealed)
                {
                    HUD.IncreaseCombo();
                }

                GridLogic.Input(new GridState.Input.InputProcessed(outcome));
            }
        }
    }

    public async Task<PopupChoiceOnWin> GetPopupChoiceOnWinAsync(CancellationToken ct = default)
    {
        return await HUD.ShowAndGetChoiceOnWinAsync(ct.LinkWithNodeDestroy(this).Token);
    }

    public async Task<PopupChoiceOnLose> GetPopupChoiceOnLoseAsync(CancellationToken ct = default)
    {
        return await HUD.ShowAndGetChoiceOnLoseAsync(ct.LinkWithNodeDestroy(this).Token);
    }

    void OnGridInputEmitted(GridInput input)
    {
        GridLogic.Input(new GridState.Input.PlayerInput(input));
    }

    void OnUndoRequested() => GridLogic.Input(new GridState.Input.Undo());

    public void NewGame()
    {
        AppLogic.InputNewGame(LoadLevelSource.CreateRegularStart(SaveData.State.MainSetting));
    }

    public void BackToMainMenu()
    {
        AppLogic.Input(new AppState.Input.BackToMainMenu());
    }

    public void Retry(GameRunRecord runRecord)
    {
        AppLogic.InputNewGame(new FromRunRecord(runRecord));
    }
}