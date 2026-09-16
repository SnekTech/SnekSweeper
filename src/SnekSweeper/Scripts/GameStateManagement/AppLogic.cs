using Chickensoft.LogicBlocks;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GameStateManagement;

public partial class AppLogic : LogicBlock
{
    public record Data
    {
        public LoadLevelSource LoadLevelSource { get; set; } = LoadLevelSource.CreateDefaultRegularStart();
    }

    public AppLogic()
    {
        Set(new Data());

        Set(new AppState.SplashScreen());
        Set(new AppState.MainMenu());
        Set(new AppState.InGame());
        Set(new AppState.AfterGame());
        Set(new AppState.HistoryPage());
        Set(new AppState.SettingsPage());
        Set(new AppState.CheatCodePage());
        Set(new AppState.TutorialPage());
    }

    public void InputNewGame(LoadLevelSource loadLevelSource) => Input(new AppState.Input.NewGame(loadLevelSource));
    public void InputGameEnd() => Input(new AppState.Input.GameEnd());
}

[StateDiagram]
public abstract record AppState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct AnyKeyPressed;
        public readonly record struct SettingsPressed;
        public readonly record struct HistoryPressed;
        public readonly record struct TutorialPressed;
        public readonly record struct CheatCodePressed;
        public readonly record struct NewGame(LoadLevelSource LoadLevelSource);
        public readonly record struct BackToMainMenu;
        public readonly record struct GameEnd;
    }

    /// <summary>
    /// 大脑发出的命令: 只说"去哪", 不说"长什么样"。
    /// 具体是哪个场景、配哪套背景主题, 由组合根(Main)翻译。
    /// </summary>
    public static class Output
    {
        public readonly record struct ShowMainMenu;
        public readonly record struct ShowSettings;
        public readonly record struct ShowHistory;
        public readonly record struct ShowCheatCode;
        public readonly record struct ShowTutorial;
        public readonly record struct ShowLevel(LoadLevelSource Source);
    }

    public record SplashScreen : AppState, IGet<Input.AnyKeyPressed>
    {
        public Type On(in Input.AnyKeyPressed input) => To<MainMenu>();
    }

    public record MainMenu : AppState,
        IGet<Input.NewGame>,
        IGet<Input.HistoryPressed>,
        IGet<Input.SettingsPressed>,
        IGet<Input.CheatCodePressed>,
        IGet<Input.TutorialPressed>
    {
        public MainMenu()
        {
            this.OnEnter(() => Output(new Output.ShowMainMenu()));
        }

        public Type On(in Input.NewGame input)
        {
            Get<AppLogic.Data>().LoadLevelSource = input.LoadLevelSource;
            return To<InGame>();
        }

        public Type On(in Input.HistoryPressed input) => To<HistoryPage>();
        public Type On(in Input.SettingsPressed input) => To<SettingsPage>();
        public Type On(in Input.CheatCodePressed input) => To<CheatCodePage>();
        public Type On(in Input.TutorialPressed input) => To<TutorialPage>();
    }

    public record InGame : AppState, IGet<Input.BackToMainMenu>, IGet<Input.GameEnd>
    {
        public InGame()
        {
            this.OnEnter(() => Get<IAppRepo>().GameEnded += OnGameEnded);
            this.OnExit(() => Get<IAppRepo>().GameEnded -= OnGameEnded);

            this.OnEnter(() => Output(new Output.ShowLevel(Get<AppLogic.Data>().LoadLevelSource)));
        }

        void OnGameEnded() => Input(new Input.GameEnd());


        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();
        public Type On(in Input.GameEnd input) => To<AfterGame>();
    }

    public record AfterGame : AppState, IGet<Input.NewGame>, IGet<Input.BackToMainMenu>
    {
        public Type On(in Input.NewGame input)
        {
            Get<AppLogic.Data>().LoadLevelSource = input.LoadLevelSource;
            return To<InGame>();
        }

        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();
    }

    public record SettingsPage : AppState, IGet<Input.BackToMainMenu>
    {
        public SettingsPage()
        {
            this.OnEnter(() => Output(new Output.ShowSettings()));
        }

        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();
    }

    public record CheatCodePage : AppState, IGet<Input.BackToMainMenu>
    {
        public CheatCodePage()
        {
            this.OnEnter(() => Output(new Output.ShowCheatCode()));
        }

        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();
    }

    public record HistoryPage : AppState, IGet<Input.BackToMainMenu>, IGet<Input.NewGame>
    {
        public HistoryPage()
        {
            this.OnEnter(() => Output(new Output.ShowHistory()));
        }

        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();

        public Type On(in Input.NewGame input)
        {
            Get<AppLogic.Data>().LoadLevelSource = input.LoadLevelSource;
            return To<InGame>();
        }
    }

    public record TutorialPage : AppState, IGet<Input.BackToMainMenu>
    {
        public TutorialPage()
        {
            this.OnEnter(() => Output(new Output.ShowTutorial()));
        }

        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();
    }
}
