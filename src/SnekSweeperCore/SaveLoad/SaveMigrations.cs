namespace SnekSweeperCore.SaveLoad;

public sealed class SaveVersionNotSupportedException(int version)
    : Exception($"Save data version {version} is not supported.");

static class SaveMigrations
{
    /// <summary>
    /// Upgrades a historical DTO to the current version.
    /// Each future schema change adds a new versioned DTO, a pure step function here,
    /// and a new arm in this switch.
    /// </summary>
    internal static PlayerSaveDataDto MigrateToCurrent(PlayerSaveDataDto dto, int version) =>
        version switch
        {
            SaveVersion.Current => dto,
            _ => throw new SaveVersionNotSupportedException(version),
        };
}
