using AwesomeAssertions;
using SnekSweeperCore.ComboSystem;

namespace BasicTests.ComboTests;

public sealed class ComboCounterTests
{
    const float DefaultTinyTolerance = 1e-6f;

    #region core behavior

    [Test]
    public void Increment_IncreasesLevel()
    {
        var counter = CreateCounter();

        counter.Increment();
        counter.Increment();

        counter.Level.Should().Be(2);
    }

    [Test]
    public void Increment_ClampsAtMaxLevel()
    {
        var maxLevel = 4;
        var counter = CreateCounter(maxLevel: maxLevel);

        counter.IncrementForNTimes(10);

        counter.Level.Should().Be(maxLevel);
    }

    [Test]
    public void Increment_ResetsProgressRatio()
    {
        var counter = CreateCounter();

        counter.Increment();
        counter.Update(3f);
        counter.Increment();

        counter.ProgressRatio.Should().BeApproximately(1f, DefaultTinyTolerance);
    }

    [Test]
    public void Reset_SetsLevelToZero()
    {
        var counter = CreateCounter();

        counter.Increment();
        counter.Increment();
        counter.Reset();

        counter.Level.Should().Be(0);
    }

    [Test]
    public void Reset_ClearsProgressRatio()
    {
        var counter = CreateCounter();

        counter.Increment();
        counter.Increment();
        counter.Update(2f);
        counter.Reset();

        counter.ProgressRatio.Should().BeApproximately(0f, DefaultTinyTolerance);
    }

    #endregion

    #region time elapse

    [Test]
    public void Update_DecaysOneLevel_AfterExactInterval()
    {
        var counter = CreateCounter();

        counter.Increment(); // lv1
        counter.Increment(); // lv2
        counter.Increment(); // lv3

        counter.Update(5f);

        counter.Level.Should().Be(2);
    }

    [Test]
    public void Update_DecaysMultipleLevels()
    {
        var counter = CreateCounter();

        counter.IncrementForNTimes(4); // lv4

        counter.Update(12f); // 5 x 2 + 2 = 12

        counter.Level.Should().Be(2);
    }

    [Test]
    public void Update_StopsAtZero()
    {
        var counter = CreateCounter();

        counter.IncrementForNTimes(4); // lv4

        counter.Update(100f); // deltaTime long enough

        counter.Level.Should().Be(0);
    }

    [Test]
    public void Update_ZeroesProgressWhenLevelZero()
    {
        var counter = CreateCounter();

        counter.IncrementForNTimes(4); // lv4

        counter.Update(100f); // long enough

        counter.ProgressRatio.Should().BeApproximately(0f, DefaultTinyTolerance);
    }

    [Test]
    public void Update_PartialInterval_DoesNotDecay()
    {
        var counter = CreateCounter();

        counter.Increment();
        counter.Increment(); // lv2

        counter.Update(4.9f); // less than 5f

        counter.Level.Should().Be(2);
    }

    [Test]
    public void Increment_PreventsDecay()
    {
        var counter = CreateCounter();

        counter.Increment();
        counter.Increment(); // lv2
        counter.Update(4.5f); // less than 5f, no decay
        counter.Increment(); // reset timer, lv3
        counter.Update(4.5f); // still < 5, won't change

        counter.Level.Should().Be(3);
    }

    #endregion

    #region progressRatio

    [Test]
    public void ProgressRatio_StartsAtOne()
    {
        var counter = CreateCounter();

        counter.Increment();

        counter.ProgressRatio.Should().BeApproximately(1f, DefaultTinyTolerance);
    }

    [Test]
    public void ProgressRatio_LinearWithTime()
    {
        var decayInterval = 5f;
        var counter = CreateCounter(decayInterval: decayInterval);

        counter.Increment();
        counter.Update(decayInterval / 2f);

        counter.ProgressRatio.Should().BeApproximately(0.5f, 0.01f);
    }

    [Test]
    public void ProgressRatio_ApproachesZero()
    {
        var counter = CreateCounter();

        counter.Increment();
        counter.Update(4.99f);

        counter.ProgressRatio.Should().BeApproximately(0f, 0.01f);
    }

    [Test]
    public void ProgressRatio_NeverExceedsOne_AfterNegativeDelta()
    {
        var counter = CreateCounter();

        counter.Increment();
        counter.Update(-1f);

        counter.ProgressRatio.Should().BeApproximately(1f, DefaultTinyTolerance);
    }

    #endregion

    #region combo counter config

    [Test]
    public void Constructor_RejectsNonPositiveDecayInterval()
    {
        var act = () => new ComboConfig(decayInterval: 0f);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Constructor_RejectsMaxLevelBelowTwo()
    {
        var act = () => new ComboConfig(maxLevel: 1);

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    static ComboCounter CreateCounter(int maxLevel = 4, float decayInterval = 5f)
        => new(new ComboConfig(maxLevel, decayInterval));
}

file static class CounterExtensions
{
    extension(ComboCounter counter)
    {
        internal void IncrementForNTimes(int n)
        {
            for (int i = 0; i < n; i++)
            {
                counter.Increment();
            }
        }
    }
}