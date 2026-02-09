namespace SnekSweeperCore.LevelManagement;

public interface ILevelOrchestrator
{
    Task<PopupChoiceOnWin> GetPopupChoiceOnWinAsync(CancellationToken ct = default);
    void NewGame();
    void BackToMainMenu();
}

public abstract record PopupChoiceOnWin;

public sealed record NewGame : PopupChoiceOnWin;
public sealed record Leave : PopupChoiceOnWin;