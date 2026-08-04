using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeper.Levels;
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
            this.OnEnter(() => Get<ISceneSwitcher>().GotoScene<UI.MainScreen.MainMenuContainer>());
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

            this.OnEnter(delegate
            {
                var loadLevelSource = Get<AppLogic.Data>().LoadLevelSource;
                Get<ISceneSwitcher>().GotoSceneAsync<Level1>(level => level.LoadLevel(loadLevelSource),
                    CancellationToken.None).Forget();
            });
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
            this.OnEnter(() => Get<ISceneSwitcher>().GotoScene<UI.Settings.SettingsPage>());
        }

        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();
    }

    public record CheatCodePage : AppState, IGet<Input.BackToMainMenu>
    {
        public CheatCodePage()
        {
            this.OnEnter(() => Get<ISceneSwitcher>().GotoScene<CheatCodeSystem.UI.CheatCodePage>());
        }

        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();
    }

    public record HistoryPage : AppState, IGet<Input.BackToMainMenu>, IGet<Input.NewGame>
    {
        public HistoryPage()
        {
            this.OnEnter(() => Get<ISceneSwitcher>().GotoScene<UI.History.HistoryPage>());
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
            this.OnEnter(() => Get<ISceneSwitcher>().GotoScene<UI.Tutorial.TutorialPage>());
        }

        public Type On(in Input.BackToMainMenu input) => To<MainMenu>();
    }
}