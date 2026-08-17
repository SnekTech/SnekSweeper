using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.SaveLoad;

/// <summary>
/// Lifts a sub-state update to the whole <see cref="PlayerSaveData"/>.
/// One method per domain — a fixed set that does not grow per feature.
/// B-stage consumers use these now (via a temporary bridge); the SaveData
/// store's <c>Dispatch</c> will use the same functions in D-stage.
/// </summary>
public static class SaveDataLenses
{
    extension(PlayerSaveData state)
    {
        public PlayerSaveData UpdateMainSetting(Func<MainSetting, MainSetting> reduce) =>
            state with { MainSetting = reduce(state.MainSetting) };

        public PlayerSaveData UpdateActivatedCheatCodeSet(Func<ActivatedCheatCodeSet, ActivatedCheatCodeSet> reduce) =>
            state with { ActivatedCheatCodeSet = reduce(state.ActivatedCheatCodeSet) };

        public PlayerSaveData UpdateCurrentRunInfo(Func<CurrentRunInfo, CurrentRunInfo> reduce) =>
            state with { CurrentRunInfo = reduce(state.CurrentRunInfo) };

        public PlayerSaveData UpdateHistory(Func<History, History> reduce) =>
            state with { History = reduce(state.History) };
    }
}
