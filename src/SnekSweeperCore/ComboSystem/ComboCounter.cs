namespace SnekSweeperCore.ComboSystem;

public sealed class ComboCounter(ComboConfig config)
{
    float _elapsedSinceIncrement;

    public int Level { get; private set; }

    public float ProgressRatio =>
        Level == 0 ? 0f : 1f - Math.Clamp(_elapsedSinceIncrement / config.DecayInterval, 0f, 1f);

    public void Update(float deltaTime)
    {
        if (Level == 0)
            return;

        _elapsedSinceIncrement = Math.Max(0f, _elapsedSinceIncrement + deltaTime);

        while (_elapsedSinceIncrement >= config.DecayInterval && Level > 0)
        {
            _elapsedSinceIncrement -= config.DecayInterval;
            Level--;
        }

        if (Level == 0)
        {
            _elapsedSinceIncrement = 0;
        }
    }

    public void Increment()
    {
        _elapsedSinceIncrement = 0;
        Level = Math.Min(Level + 1, config.MaxLevel);
    }

    public void Reset()
    {
        Level = 0;
        _elapsedSinceIncrement = 0;
    }

    public static ComboTier GetTier(int level) => level switch
    {
        <= 1 => ComboTier.None,
        2 => ComboTier.Good,
        3 => ComboTier.Great,
        _ => ComboTier.Excellent,
    };
}

public enum ComboTier
{
    None,
    Good,
    Great,
    Excellent,
}