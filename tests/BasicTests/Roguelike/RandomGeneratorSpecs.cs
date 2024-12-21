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

        var sequenceA = GenerateSequence(generatorA, 3);
        var sequenceB = GenerateSequence(generatorB, 3);

        sequenceA.Should().Equal(sequenceB);
    }

    [Test]
    public void should_generate_same_before_and_after_reset()
    {
        const int seed = 12345;
        const int state = 0;
        var generator = new RandomGenerator();
        generator.Reset(seed,state);

        var firstSequence = GenerateSequence(generator, 3);
        generator.Reset(seed, state);
        var sequenceAfterReset = GenerateSequence(generator, 3);

        firstSequence.Should().Equal(sequenceAfterReset);
    }

    private static List<int> GenerateSequence(RandomGenerator generator, int length)
    {
        var result = new List<int>();
        for (var i = 0; i < length; i++)
        {
            result.Add(generator.PickInt());
        }

        return result;
    }
}