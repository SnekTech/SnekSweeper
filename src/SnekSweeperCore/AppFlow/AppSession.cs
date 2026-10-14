using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.AppFlow;

public abstract record AppSession
{
    public sealed record SplashScreen : AppSession;
    public sealed record MainMenu : AppSession;
    public sealed record InGame : AppSession;
    public sealed record History : AppSession;
    public sealed record Settings : AppSession;
    public sealed record CheatCode : AppSession;
    public sealed record Tutorial : AppSession;

    public abstract record Event
    {
        public sealed record AnyKeyPressed : Event;
        public sealed record NewGame(LoadLevelSource Source) : Event;
        public sealed record HistoryPressed : Event;
        public sealed record SettingsPressed : Event;
        public sealed record CheatCodePressed : Event;
        public sealed record TutorialPressed : Event;
        public sealed record BackToMainMenu : Event;
    }
}

public abstract record AppScreen
{
    public sealed record MainMenu : AppScreen;
    public sealed record Settings : AppScreen;
    public sealed record History : AppScreen;
    public sealed record CheatCode : AppScreen;
    public sealed record Tutorial : AppScreen;
    public sealed record Level(LoadLevelSource Source) : AppScreen;
}

public abstract record AppSessionEffect
{
    public sealed record Stay : AppSessionEffect;

    public sealed record Enter(AppScreen TargetScreen) : AppSessionEffect;
}

public readonly record struct AppSessionWithEffect(AppSession Session, AppSessionEffect Effect);

public static class AppSessionExtensions
{
    extension(AppSession appSession)
    {
        public static AppSession Initial => new AppSession.SplashScreen();

        public AppSessionWithEffect Update(AppSession.Event evt) =>
            (appSession, evt) switch
            {
                (AppSession.SplashScreen, AppSession.Event.AnyKeyPressed)
                    => Go(new AppSession.MainMenu(), new AppScreen.MainMenu()),

                // main menu transitions
                (AppSession.MainMenu, AppSession.Event.NewGame { Source: var source })
                    => Go(new AppSession.InGame(), new AppScreen.Level(source)),
                (AppSession.MainMenu, AppSession.Event.HistoryPressed)
                    => Go(new AppSession.History(), new AppScreen.History()),
                (AppSession.MainMenu, AppSession.Event.SettingsPressed)
                    => Go(new AppSession.Settings(), new AppScreen.Settings()),
                (AppSession.MainMenu, AppSession.Event.CheatCodePressed)
                    => Go(new AppSession.CheatCode(), new AppScreen.CheatCode()),
                (AppSession.MainMenu, AppSession.Event.TutorialPressed)
                    => Go(new AppSession.Tutorial(), new AppScreen.Tutorial()),

                // new game
                (AppSession.InGame, AppSession.Event.NewGame { Source: var source })
                    => Go(new AppSession.InGame(), new AppScreen.Level(source)),
                (AppSession.History, AppSession.Event.NewGame { Source: var source })
                    => Go(new AppSession.InGame(), new AppScreen.Level(source)),

                // back-to-main
                (AppSession.InGame
                    or AppSession.Settings
                    or AppSession.History
                    or AppSession.CheatCode
                    or AppSession.Tutorial, AppSession.Event.BackToMainMenu)
                    => Go(new AppSession.MainMenu(), new AppScreen.MainMenu()),

                // invalid combination, no outcome
                _ => Go(appSession, null),
            };
    }

    static AppSessionWithEffect Go(AppSession next, AppScreen? targetScreen) => new(next, targetScreen switch
    {
        null => new AppSessionEffect.Stay(),
        _ => new AppSessionEffect.Enter(targetScreen),
    });
}