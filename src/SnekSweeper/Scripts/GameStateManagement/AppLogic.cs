using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeper.Levels;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GameStateManagement;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class AppLogic : LogicBlock<AppLogic.State>
{
    public static class Input
    {
        public readonly record struct AnyKeyPressed;
        public readonly record struct SettingsPressed;
        public readonly record struct HistoryPressed;
        public readonly record struct CheatCodePressed;
        public readonly record struct NewGame(LoadLevelSource LoadLevelSource);
        public readonly record struct BackToMainMenu;
        public readonly record struct GameEnd;
    }

    public record Data
    {
        public LoadLevelSource LoadLevelSource { get; set; } = LoadLevelSource.CreateDefaultRegularStart();
    }

    public abstract record State : StateLogic<State>
    {
        public record SplashScreen : State, IGet<Input.AnyKeyPressed>
        {
            public Transition On(in Input.AnyKeyPressed input) => To<MainMenu>();
        }

        public record MainMenu : State,
            IGet<Input.NewGame>,
            IGet<Input.HistoryPressed>,
            IGet<Input.SettingsPressed>,
            IGet<Input.CheatCodePressed>
        {
            public MainMenu()
            {
                OnAttach(() => Get<ISceneSwitcher>().GotoScene<UI.MainScreen.MainMenuContainer>());
            }

            public Transition On(in Input.NewGame input)
            {
                Get<Data>().LoadLevelSource = input.LoadLevelSource;
                return To<InGame>();
            }

            public Transition On(in Input.HistoryPressed input) => To<HistoryPage>();

            public Transition On(in Input.SettingsPressed input) => To<SettingsPage>();

            public Transition On(in Input.CheatCodePressed input) => To<CheatCodePage>();
        }

        public record InGame : State, IGet<Input.BackToMainMenu>, IGet<Input.GameEnd>
        {
            public InGame()
            {
                OnAttach(() => Get<IAppRepo>().GameEnded += OnGameEnded);
                OnDetach(() => Get<IAppRepo>().GameEnded -= OnGameEnded);

                this.OnEnter(delegate
                {
                    var loadLevelSource = Get<Data>().LoadLevelSource;
                    Get<ISceneSwitcher>().GotoSceneAsync<Level1>(level => level.LoadLevel(loadLevelSource),
                        CancellationToken.None).Forget();
                });
            }

            void OnGameEnded() => Input(new Input.GameEnd());


            public Transition On(in Input.BackToMainMenu input) => To<MainMenu>();
            public Transition On(in Input.GameEnd input) => To<AfterGame>();
        }

        public record AfterGame : State, IGet<Input.NewGame>, IGet<Input.BackToMainMenu>
        {
            public Transition On(in Input.NewGame input)
            {
                Get<Data>().LoadLevelSource = input.LoadLevelSource;
                return To<InGame>();
            }

            public Transition On(in Input.BackToMainMenu input) => To<MainMenu>();
        }

        public record SettingsPage : State, IGet<Input.BackToMainMenu>
        {
            public SettingsPage()
            {
                OnAttach(() => Get<ISceneSwitcher>().GotoScene<UI.Settings.SettingsPage>());
            }

            public Transition On(in Input.BackToMainMenu input) => To<MainMenu>();
        }

        public record CheatCodePage : State, IGet<Input.BackToMainMenu>
        {
            public CheatCodePage()
            {
                OnAttach(() => Get<ISceneSwitcher>().GotoScene<CheatCodeSystem.UI.CheatCodePage>());
            }

            public Transition On(in Input.BackToMainMenu input) => To<MainMenu>();
        }

        public record HistoryPage : State, IGet<Input.BackToMainMenu>, IGet<Input.NewGame>
        {
            public HistoryPage()
            {
                OnAttach(() => Get<ISceneSwitcher>().GotoScene<UI.History.HistoryPage>());
            }

            public Transition On(in Input.BackToMainMenu input) => To<MainMenu>();

            public Transition On(in Input.NewGame input)
            {
                Get<Data>().LoadLevelSource = input.LoadLevelSource;
                return To<InGame>();
            }
        }
    }

    public override Transition GetInitialState() => To<State.SplashScreen>();

    public void InputNewGame(LoadLevelSource loadLevelSource) => Input(new Input.NewGame(loadLevelSource));
    public void InputGameEnd() => Input(new Input.GameEnd());
}