using System.Text.Json.Serialization;
using SnekSweeper.SaveLoad;
using Widgets.Roguelike;

namespace SnekSweeper.GameHistory;

public readonly record struct RunDuration(DateTime StartAt, DateTime EndAt);

public record GameRunRecord(RunDuration Duration, bool Winning, RngData RngData)
{
    [JsonConverter(typeof(Mat2DConverter2))]
    public required bool[,] BombMatrix { get; init; }
}

static class GameRunRecordExtensions
{
    extension(RunDuration)
    {
        internal static RunDuration Create(DateTime startAt, DateTime endAt) => new(startAt, endAt);
    }

    extension(GameRunRecord)
    {
        internal static GameRunRecord Create(RunDuration duration, bool winning, RngData rngData, bool[,] bombMatrix) =>
            new(duration, winning, rngData) { BombMatrix = bombMatrix };
    }
}