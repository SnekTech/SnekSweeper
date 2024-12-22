﻿using System;

namespace SnekSweeper.Roguelike;

public static class Rand
{
    private static readonly RandomGenerator Generator = new();

    public static void Reset(ulong seed = 0, ulong state = 0) => Generator.Reset(seed, state);

    public static void Randomize()
    {
        var seed = (ulong)new Random().Next();
        Reset(seed, 0);
    }

    public static float Float() => Generator.PickFloat();
}