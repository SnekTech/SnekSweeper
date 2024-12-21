namespace SnekSweeper.Roguelike;

public class RandomGenerator
{
    public RandomGenerator(ulong seed = 0, ulong state = 0)
    {
        _generator = new Pcg32(seed, state);
    }
    
    private Pcg32 _generator;

    public void Reset(ulong seed, ulong state)
    {
        _generator = new Pcg32(seed, state);
    }

    public ulong Seed => _generator.Seed;

    public ulong State
    {
        get => _generator.State;
        set => _generator.State = value;
    }

    public int PickInt()
    {
        return (int)_generator.GenerateNext();
    }

    public bool PickBool()
    {
        return _generator.GenerateNext() > (uint.MaxValue / 2);
    }
}