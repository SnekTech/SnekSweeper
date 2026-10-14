using AwesomeAssertions;
using BasicTests.Utils;
using SnekSweeperCore.AppFlow;
using SnekSweeperCore.LevelManagement;

namespace BasicTests.AppFlow;

public sealed class AppFlowSpecs
{
    [Test]
    public void splash_goes_to_mainMenu()
    {
        var init = AppSession.Initial;

        var (next, effect) = init.Update(new AppSession.Event.AnyKeyPressed());

        init.Should().Be(new AppSession.SplashScreen());
        next.Should().Be(new AppSession.MainMenu());
        effect.Should().Be(new AppSessionEffect.Enter(new AppScreen.MainMenu()));
    }

    [Test]
    public void mainMenu_goes_to_inGame_on_new_game_event()
    {
        var mainMenuSession = new AppSession.MainMenu();
        var regularStart = LoadLevelSource.CreateDefaultRegularStart();

        var (next, effect) = mainMenuSession.Update(new AppSession.Event.NewGame(regularStart));

        next.Should().Be(new AppSession.InGame());
        effect.Should().Be(new AppSessionEffect.Enter(new AppScreen.Level(regularStart)));
    }

    [Test]
    public void inGame_goes_back_to_mainMenu_on_backToMain()
    {
        var inGame = new AppSession.InGame();

        var (next, effect) = inGame.Update(new AppSession.Event.BackToMainMenu());

        next.Should().Be(new AppSession.MainMenu());
        effect.Should().Be(new AppSessionEffect.Enter(new AppScreen.MainMenu()));
    }

    [Test]
    public void inGame_stays_inGame_on_newGame()
    {
        var inGame = new AppSession.InGame();
        var regularStart = LoadLevelSource.CreateDefaultRegularStart();

        var (next, effect) = inGame.Update(new AppSession.Event.NewGame(regularStart));

        next.Should().Be(new AppSession.InGame());
        effect.Should().Be(new AppSessionEffect.Enter(new AppScreen.Level(regularStart)));
    }

    [Test]
    public void mainMenu_stays_mainMenu_on_backToMain()
    {
        var mainMenu = new AppSession.MainMenu();

        var (next, effect) = mainMenu.Update(new AppSession.Event.BackToMainMenu());

        next.Should().Be(new AppSession.MainMenu());
        effect.Should().Be(new AppSessionEffect.Stay());
    }

    [Test]
    public void settings_stays_settings_on_newGame()
    {
        var settings = new AppSession.Settings();
        var regularStart = LoadLevelSource.CreateDefaultRegularStart();

        var (next, effect) = settings.Update(new AppSession.Event.NewGame(regularStart));

        next.Should().Be(new AppSession.Settings());
        effect.Should().Be(new AppSessionEffect.Stay());
    }

    [Test]
    public void history_goes_to_newGame()
    {
        var history = new AppSession.History();
        var runRecord = RunRecordFactory.GetSampleWinningRecord();
        var fromRun = new FromRunRecord(runRecord);

        var (next, effect) = history.Update(new AppSession.Event.NewGame(fromRun));

        next.Should().Be(new AppSession.InGame());
        effect.Should().BeOfType<AppSessionEffect.Enter>()
            .Which.TargetScreen.Should().BeOfType<AppScreen.Level>();
    }
}