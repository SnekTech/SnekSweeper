using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Levels;
using SnekSweeper.UI.Common.AmbientBackground;
using SnekSweeper.Widgets;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper;

[Meta(typeof(IAutoNode))]
public partial class Main : Node, IProvide<AppLogic>, IProvide<IAppRepo>, IProvide<ISaveDataStore>
{
    /// <summary>背景过渡时长(步骤 7 的编排旋钮)。</summary>
    const float AmbientTransitionDuration = 0.8f;

    [Node]
    public ISceneSwitcher SceneSwitcher { get; set; } = null!;

    [Node]
    public AmbientBackground Background { get; set; } = null!;

    IAppRepo AppRepo { get; set; } = null!;
    AppLogic _appLogic = null!;
    LogicBlock.Binding _binding = null!;

    AppLogic IProvide<AppLogic>.Value() => _appLogic;
    IAppRepo IProvide<IAppRepo>.Value() => AppRepo;
    ISaveDataStore IProvide<ISaveDataStore>.Value() => SaveData.Instance;

    public void OnReady()
    {
        AppRepo = new AppRepo();
        _appLogic = new AppLogic();
        _appLogic.Set(AppRepo);
        this.Provide();

        // 组合根: 把大脑的命令翻译成两个平等、独立的副作用 —— 切换场景 + 背景过渡。
        // 绑定必须在 Start 之前建立, 否则会漏掉初始状态的 OnEnter 输出。
        _binding = _appLogic.Bind()
            .OnOutput((in AppState.Output.ShowMainMenu output) =>
                Show<UI.MainScreen.MainMenuContainer>(AmbientThemes.Menu))
            .OnOutput((in AppState.Output.ShowSettings output) =>
                Show<UI.Settings.SettingsPage>(AmbientThemes.Menu))
            .OnOutput((in AppState.Output.ShowHistory output) =>
                Show<UI.History.HistoryPage>(AmbientThemes.Menu))
            .OnOutput((in AppState.Output.ShowCheatCode output) =>
                Show<CheatCodeSystem.UI.CheatCodePage>(AmbientThemes.Menu))
            .OnOutput((in AppState.Output.ShowTutorial output) =>
                Show<UI.Tutorial.TutorialPage>(AmbientThemes.Menu))
            // 将来要按关卡区分主题时, 在这里用 output.Source 决定
            .OnOutput((in AppState.Output.ShowLevel output) =>
            {
                var source = output.Source;
                Show<Level1>(AmbientThemes.Level, level => level.LoadLevel(source));
            });

        SaveData.Instance.SavedFeedback += () => MessageBox.Print("已保存");

        _appLogic.Start<AppState.SplashScreen>();
    }

    public override void _Notification(int what) => this.Notify(what);

    public override void _ExitTree() => _binding.Dispose();

    void Show<T>(AmbientTheme theme, Action<T>? onSceneEntered = null) where T : Node, ISceneScript
    {
        SceneSwitcher.GotoSceneAsync(onSceneEntered).Forget();
        Background.GoTo(theme, AmbientTransitionDuration);
    }
}
