﻿using Widgets.Roguelike;

namespace BasicTests.Roguelike;

public class RandomGeneratorSpecs
{
    private const int TestSequenceLength = 3;

    [Test]
    public void should_generate_same_first_number_when_created_with_same_seed()
    {
        var rngData = new RngData(12345, 1);
        var generatorA = new RandomGenerator(rngData);
        var generatorB = new RandomGenerator(rngData);

        var sequenceA = GenerateSequence(generatorA);
        var sequenceB = GenerateSequence(generatorB);

        sequenceA.Should().Equal(sequenceB);
    }

    [Test]
    public void should_generate_same_before_and_after_reset()
    {
        var rngData = new RngData(12345, 0);
        var generator = new RandomGenerator(rngData);

        var firstSequence = GenerateSequence(generator);
        generator.Reset(rngData);
        var sequenceAfterReset = GenerateSequence(generator);

        firstSequence.Should().Equal(sequenceAfterReset);
    }

    [Test]
    public void should_generate_same_before_and_after_setting_state_property()
    {
        var rngData = new RngData(12345, 10);
        var generator = new RandomGenerator(rngData);

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
        var generator = new RandomGenerator(RngData.Empty);

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

    private static List<int> GenerateSequence(RandomGenerator generator, int length = TestSequenceLength)
    {
        var result = new List<int>();
        for (var i = 0; i < length; i++)
        {
            result.Add(generator.PickInt());
        }

        return result;
    }
}