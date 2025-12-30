namespace SnekSweeperCore.SaveLoad.SerializationService;

public delegate void SaveDataFn(PlayerSaveData playerSaveData, string filePath);

public delegate PlayerSaveData? LoadDataFn(string filePath);

record SerializationStrategy(SaveDataFn SaveDataFn, LoadDataFn LoadDataFn, string SaveFileName);