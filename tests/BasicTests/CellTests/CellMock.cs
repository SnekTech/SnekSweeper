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
    public void SetPosition(GridIndex gridIndex)
    {
        $"setting cell position with index {gridIndex}".Dump();
    }

    public void SetSkin(GridSkin skin)
    {
        $"setting cell skin to {skin}".Dump();
    }

    public void SetContent(bool hasBomb, int neighborBombCount)
    {
        var initInfo = $"hasBomb = {hasBomb}, neighbor bomb count = {neighborBombCount}";
        $"setting cell content with {initInfo}".Dump();
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