using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.SaveLoad.SerializationService;

namespace SnekSweeperCore.SaveLoad;

public record PlayerSaveData(
    MainSetting MainSetting,
    ActivatedCheatCodeSet ActivatedCheatCodeSet,
    History History);

public static class PlayerSaveDataExtensions
{
    static readonly SerializationStrategy DefaultStrategy = SerializationStrategy.MemoryPack;

    extension(PlayerSaveData playerSaveData)
    {
        public static PlayerSaveData CreateEmpty() =>
            new(new MainSetting(), new ActivatedCheatCodeSet([]), new History([]));

        public void Save(string userDataDir) =>
            DefaultStrategy.Save(playerSaveData,userDataDir.ToDir());

        public static PlayerSaveData? Load(string userDataDir) =>
            DefaultStrategy.Load(userDataDir.ToDir());
    }

    extension(string saveDir)
    {
        SaveDir ToDir() => new(saveDir);
    }
}