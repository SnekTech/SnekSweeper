using Dumpify;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.CellSystem.Components;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;

namespace BasicTests.CellTests;

class MockHumbleCell : IHumbleCell
{
    public ICover Cover { get; } = new MockCover();
    public IFlag Flag { get; } = new MockFlag();
    public void OnInstantiate(GridIndex gridIndex, GridSkin skin)
    {
        $"{nameof(OnInstantiate)}, grid index: {gridIndex}, skin: {skin}".Dump();
    }

    public void OnInit(CellInitData initData)
    {
        $"{nameof(OnInit)}, init data: {initData}".Dump();
    }

    public void MarkAsWrongFlagged()
    {
        "this humble cell is marked as wrong-flagged".Dump();
    }

    public void MarkAsBombRevealed()
    {
        "this humble cell is marked as bomb-revealed".Dump();
    }
}

class MockCover :ICover
{
    public Task RevealAsync(CancellationToken cancellationToken)
    {
        "revealing the cover".Dump();
        return Task.CompletedTask;
    }

    public Task PutOnAsync(CancellationToken cancellationToken)
    {
        "putting on the cover".Dump();
        return Task.CompletedTask;
    }

    public void SetAlpha(float normalizedAlpha)
    {
        $"setting cover alpha to {normalizedAlpha}".Dump();
    }
}

class MockFlag : IFlag
{
    public Task RaiseAsync(CancellationToken cancellationToken)
    {
        "raising flag".Dump();
        return Task.CompletedTask;
    }

    public Task PutDownAsync(CancellationToken cancellationToken)
    {
        "putting down flag".Dump();
        return Task.CompletedTask;
    }
}