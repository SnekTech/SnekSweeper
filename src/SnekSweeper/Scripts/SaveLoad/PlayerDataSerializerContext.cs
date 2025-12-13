using System.Text.Json.Serialization;

namespace SnekSweeper.SaveLoad;

[JsonSerializable(typeof(PlayerDataJson))]
partial class PlayerDataSerializerContext : JsonSerializerContext;