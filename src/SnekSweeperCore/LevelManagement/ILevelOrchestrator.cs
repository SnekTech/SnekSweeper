using SnekSweeperCore.GameHistory;

namespace SnekSweeperCore.LevelManagement;

public interface ILevelOrchestrator
{
    Task<PopupChoiceOnWin> GetPopupChoiceOnWinAsync(CancellationToken ct = default);
    Task<PopupChoiceOnLose> GetPopupChoiceOnLoseAsync(CancellationToken ct = default);
    void NewGame();
    void BackToMainMenu();
    void Retry(GameRunRecord runRecord);
}

public abstract record PopupChoiceOnWin;

public sealed record NewGame : PopupChoiceOnWin;

public sealed record Leave : PopupChoiceOnWin;

public enum PopupChoiceOnLose
{
    Retry,
    NewGame,
    Leave,
}