using SnekSweeper.Roguelike;

namespace BasicTests.Roguelike;

public class RandomGeneratorSpecs
{
    [Test]
    public void should_generate_same_first_number_when_created_with_same_seed()
    {
        const int seed = 12345;
        const int state = 1;
        var generatorA = new RandomGenerator();
        var generatorB = new RandomGenerator();
        generatorA.Reset(seed, state);
        generatorB.Reset(seed, state);

        var a1 = generatorA.PickInt();
        var b1 = generatorB.PickInt();

        a1.Should().Be(b1);
    }

    [Test]
    public void should_generate_same_after_state_reload()
    {
        const int seed = 12345;
    }
}