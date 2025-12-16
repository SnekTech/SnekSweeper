using System.Text.Json.Serialization;

namespace SnekSweeper.SaveLoad;

[JsonSerializable(typeof(PlayerDataJson))]
[JsonSerializable(typeof(List<string>))]
partial class PlayerDataSerializerContext : JsonSerializerContext;