namespace SnekSweeper.Roguelike;

public class RandomGenerator
{
    private Pcg32 _generator = new(0, 0);

    public void Reset(ulong seed, ulong state)
    {
        _generator = new Pcg32(seed, state);
    }

    public int PickInt()
    {
        return (int)_generator.GenerateNext();
    }

    public ulong Seed => _generator.Seed;

    public ulong State
    {
        get => _generator.State;
        set => _generator.State = value;
    }
}