using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.LevelManagement;

public record CurrentRunInfo(GridSnapshot? GridSnapshot = null, RunStartInfo StartInfo = default);
