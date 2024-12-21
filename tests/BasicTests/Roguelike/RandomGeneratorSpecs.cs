using SnekSweeper.Roguelike;

namespace BasicTests.Roguelike;

public class RandomGeneratorSpecs
{
    private const int TestSequenceLength = 3;

    private static List<int> GenerateSequence(RandomGenerator generator, int length = TestSequenceLength)
    {
        var result = new List<int>();
        for (var i = 0; i < length; i++)
        {
            result.Add(generator.PickInt());
        }

        return result;
    }

    [Test]
    public void should_generate_same_first_number_when_created_with_same_seed()
    {
        const int seed = 12345;
        const int state = 1;
        var generatorA = new RandomGenerator();
        var generatorB = new RandomGenerator();
        generatorA.Reset(seed, state);
        generatorB.Reset(seed, state);

        var sequenceA = GenerateSequence(generatorA);
        var sequenceB = GenerateSequence(generatorB);

        sequenceA.Should().Equal(sequenceB);
    }

    [Test]
    public void should_generate_same_before_and_after_reset()
    {
        const int seed = 12345;
        const int state = 0;
        var generator = new RandomGenerator();
        generator.Reset(seed, state);

        var firstSequence = GenerateSequence(generator);
        generator.Reset(seed, state);
        var sequenceAfterReset = GenerateSequence(generator);

        firstSequence.Should().Equal(sequenceAfterReset);
    }

    [Test]
    public void should_generate_same_before_and_after_setting_state_property()
    {
        const int seed = 12345;
        const int state = 10;
        var generator = new RandomGenerator();
        generator.Reset(seed, state);

        // generate some first
        GenerateSequence(generator, 5);
        var oldState = generator.State;
        var sequenceBefore = GenerateSequence(generator);
        generator.State = oldState;
        var sequenceAfter = GenerateSequence(generator);

        sequenceBefore.Should().Equal(sequenceAfter);
    }
    
    [Test]
    public void should_evenly_generate_true_or_false_when_picking_bool()
    {
        var generator = new RandomGenerator();
    
        var trueCount = 0;
        var falseCount = 0;
        for (var i = 0; i < 100; i++)
        {
            if (generator.PickBool())
                trueCount++;
            else
                falseCount++;
        }
    
        trueCount.Should().BeCloseTo(falseCount, 20);
    }
}