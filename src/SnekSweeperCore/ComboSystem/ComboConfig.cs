namespace SnekSweeperCore.ComboSystem;

public sealed record ComboConfig
{
    public int MaxLevel { get; }
    public float DecayInterval { get; }

    public ComboConfig(int maxLevel = 4, float decayInterval = 5f)
    {
        if (maxLevel < 2)
        {
            throw new ArgumentException("maxLevel must be at least 2 to reach the Good/Great/Excellent tiers");
        }

        if (decayInterval <= 0f)
        {
            throw new ArgumentException("decayInterval must be positive");
        }

        MaxLevel = maxLevel;
        DecayInterval = decayInterval;
    }

    public static ComboConfig Default => new();
}