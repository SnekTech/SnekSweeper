using MemoryPack;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.LevelManagement;

/// <summary>进行中且可续玩的局：局面快照 + 开局信息，两者必同时存在。</summary>
[MemoryPackable]
public partial record OngoingGame(GridSnapshot GridSnapshot, RunStartInfo StartInfo);

public record CurrentRunInfo(OngoingGame? OngoingGame = null);
