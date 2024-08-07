﻿using System.Collections.Generic;
using Godot;
using SnekSweeper.CellSystem;
using SnekSweeper.GridSystem;

namespace SnekSweeper.GameMode;

public class Referee
{
    private readonly Grid _grid;

    public Referee(Grid grid)
    {
        _grid = grid;
    }

    public void OnBombRevealed(List<Cell> bombsRevealed)
    {
        GD.Print("Game over! Bomb revealed!");
    }

    public void OnBatchRevealed()
    {
        if (_grid.IsResolved)
        {
            GD.Print("You win!");
        }
    }
}