using MemoryPack;

namespace SnekSweeperCore.SaveLoad;

/// <summary>
/// Versioned binary save envelope: [version][payload].
/// The payload is a MemoryPack-serialized DTO of the version indicated by <see cref="Version"/>.
/// </summary>
[MemoryPackable]
partial record BinarySaveEnvelope(int Version, byte[] Payload);
