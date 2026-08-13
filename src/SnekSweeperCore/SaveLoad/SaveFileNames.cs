namespace SnekSweeperCore.SaveLoad;

/// <summary>
/// File names of the on-disk save formats. Part of the storage contract,
/// shared by the serialization strategies and the format-pinning tests.
/// </summary>
public static class SaveFileNames
{
    public const string Json = "playerSaveData.json";
    public const string Binary = "playerSaveData.bin";
}
