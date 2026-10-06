using System.Runtime.CompilerServices;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.Session;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.Levels;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class Level1 : Node2D, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Chickensoft.AutoInject.Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();

    [Chickensoft.AutoInject.Dependency]
    IAppRepo AppRepo => this.DependOn<IAppRepo>();

    [Chickensoft.AutoInject.Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    Grid _grid = null!;
    GridSession _gridSession = null!;

    GameRunRecorder _runRecorder = null!;

    public override void _EnterTree()
    {
        TheGrid.GridInputListener.GridInputEmitted += OnGridInputEmitted;
        HUD.UndoRequested += OnUndoRequested;
    }

    public override void _ExitTree()
    {
        SaveData.NotifySaved();

        TheGrid.GridInputListener.GridInputEmitted -= OnGridInputEmitted;
        HUD.UndoRequested -= OnUndoRequested;
    }

    public void LoadLevel(LoadLevelSource loadLevelSource)
    {
        _runRecorder = new GameRunRecorder(SaveData);
        StartGridSession();

        return;

        void StartGridSession()
        {
            var setup = loadLevelSource.ToSetup();
            var size = setup.Size;
            _grid = new Grid(size);

            TheGrid.InstantiateCells(size, SaveData.CurrentSkin);
            TheGrid.Init(size);

            var (initialSession, effects) = GridSessionMachine.Start(_grid, setup);
            _gridSession = initialSession;
            PresentState();
            ApplySessionEffects(effects);
        }
    }

    void SendGridEvent(GridSession.Event evt)
    {
        var (next, effects) = GridSessionMachine.Update(_gridSession, evt, TimeProvider.System);
        _gridSession = next;

        PresentState();
        ApplySessionEffects(effects);
    }

    void PresentState()
    {
        TheGrid.Cursor.SetCursorPolicy(_gridSession.CursorPolicy);
        HUD.UpdateBombCount(_grid.BombCount);
        HUD.UpdateFlagCount(_grid.FlagCount);
    }

    void ApplySessionEffects(IReadOnlyList<GridSession.Effect> effects)
    {
        foreach (var effect in effects)
        {
            ApplyGridSessionEffect(effect);
        }

        return;

        void ApplyGridSessionEffect(GridSession.Effect effect)
        {
            switch (effect)
            {
                case GridSession.Effect.PaintBoard:
                    TheGrid.Paint(_grid);
                    break;

                case GridSession.Effect.Render render:
                    TheGrid.ApplyGridOutcome(render.Outcome);
                    break;
                
                case GridSession.Effect.TriggerInitEffects:
                    TheGrid.TriggerInitEffects();
                    break;

                case GridSession.Effect.StartOngoingRun startOngoingRun:
                    // todo: fromRunRecord 开始的游戏，startInfo 能用现有的吗？
                    _runRecorder.StartOngoingGame(startOngoingRun.Snapshot, startOngoingRun.StartInfo);
                    break;

                case GridSession.Effect.UpdateOngoingRun updateOngoingRun:
                    _runRecorder.UpdateOngoingGame(updateOngoingRun.Snapshot);
                    break;

                case GridSession.Effect.IncreaseCombo:
                    HUD.IncreaseCombo();
                    break;

                case GridSession.Effect.FinishRun finishRun:
                    _runRecorder.FinishRun(finishRun.Winning, finishRun.Bombs);
                    break;

                case GridSession.Effect.AskForWinChoice:
                    HandleAskForWinChoice().Forget();
                    break;

                case GridSession.Effect.AskForLoseChoice:
                    HandleAskForLoseChoice(_runRecorder.LatestRecord).Forget();
                    break;

                case GridSession.Effect.PlayCongratulationEffects:
                    TheGrid.PlayCongratulationEffects();
                    break;

                default:
                    throw new SwitchExpressionException();
            }
        }
    }

    async GDTaskVoid HandleAskForWinChoice()
    {
        var choice = await HUD.ShowAndGetChoiceOnWinAsync(this.GetCancellationTokenOnTreeExit());

        Action handleChoiceAction = choice switch
        {
            PopupChoiceOnWin.NewGame => NewGame,
            PopupChoiceOnWin.Leave => BackToMainMenu,
            _ => delegate { },
        };
        handleChoiceAction();
    }

    async GDTaskVoid HandleAskForLoseChoice(GameRunRecord recordJustFinished)
    {
        var choice = await HUD.ShowAndGetChoiceOnLoseAsync(this.GetCancellationTokenOnTreeExit());
        Action handleChoiceAction = choice switch
        {
            PopupChoiceOnLose.Retry => () => Retry(recordJustFinished),
            PopupChoiceOnLose.NewGame => NewGame,
            PopupChoiceOnLose.Leave => BackToMainMenu,
            _ => delegate { },
        };
        handleChoiceAction();
    }

    void OnGridInputEmitted(GridInput input) => SendGridEvent(new GridSession.Event.PlayerInput(input));

    void OnUndoRequested() => SendGridEvent(new GridSession.Event.UndoRequested());

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