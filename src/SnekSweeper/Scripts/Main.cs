using System.Runtime.CompilerServices;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Levels;
using SnekSweeper.UI.Common.AmbientBackground;
using SnekSweeper.Widgets;
using SnekSweeperCore.AppFlow;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper;

[Meta(typeof(IAutoNode))]
public partial class Main : Node, IAppSessionHost,
    IProvide<IAppSessionHost>, IProvide<ISaveDataStore>
{
    public override void _Notification(int what) => this.Notify(what);

    /// <summary>背景过渡时长(步骤 7 的编排旋钮)。</summary>
    const float AmbientTransitionDuration = 0.8f;

    [Node]
    public ISceneSwitcher SceneSwitcher { get; set; } = null!;

    [Node]
    public AmbientBackground Background { get; set; } = null!;

    ISaveDataStore IProvide<ISaveDataStore>.Value() => SaveData.Instance;

    IAppSessionHost IProvide<IAppSessionHost>.Value() => this;

    AppSession _session = AppSession.Initial;

    public void Send(AppSession.Event evt)
    {
        var (next, effect) = _session.Update(evt);
        _session = next;
        ApplyAppSessionEffect(effect);
    }

    public void OnReady()
    {
        this.Provide();

        SaveData.Instance.SavedFeedback += () => MessageBox.Print("已保存");
    }

    void Show<T>(AmbientTheme theme, Action<T>? onSceneEntered = null) where T : Node, ISceneScript
    {
        SceneSwitcher.GotoSceneAsync(onSceneEntered).Forget();
        Background.GoTo(theme, AmbientTransitionDuration);
    }

    void ApplyAppSessionEffect(AppSessionEffect effect)
    {
        Action? applyEffect = effect switch
        {
            AppSessionEffect.Stay => null,
            AppSessionEffect.Show { TargetScreen: var targetScreen } => () => ApplyTargetScreen(targetScreen),
            _ => throw new SwitchExpressionException(),
        };
        applyEffect?.Invoke();
        return;

        void ApplyTargetScreen(AppScreen targetScreen)
        {
            switch (targetScreen)
            {
                case AppScreen.MainMenu:
                    Show<UI.MainScreen.MainMenuContainer>(AmbientThemes.Menu);
                    break;

                case AppScreen.Settings:
                    Show<UI.Settings.SettingsPage>(AmbientThemes.Menu);
                    break;

                case AppScreen.History:
                    Show<UI.History.HistoryPage>(AmbientThemes.Menu);
                    break;

                case AppScreen.CheatCode:
                    Show<CheatCodeSystem.UI.CheatCodePage>(AmbientThemes.Menu);
                    break;

                case AppScreen.Tutorial:
                    Show<UI.Tutorial.TutorialPage>(AmbientThemes.Menu);
                    break;

                case AppScreen.Level { Source: var source }:
                    // todo: reuse Level1 scene
                    Show<Level1>(AmbientThemes.Level, level => level.LoadLevel(source));
                    break;

                default: throw new SwitchExpressionException();
            }
        }
    }
}