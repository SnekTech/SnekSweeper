﻿using System;
using System.Collections.Generic;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.Constants;
using SnekSweeper.GameHistory;
using SnekSweeper.GridSystem;

namespace SnekSweeper.GameMode;

public class Referee
{
    private readonly Grid _grid;
    private readonly History _history;

    public Referee(Grid grid)
    {
        _grid = grid;
        _grid.BatchRevealed += OnBatchRevealed;
        _grid.BombRevealed += OnBombRevealed;

        _history = HouseKeeper.History;
    }

    public void OnDispose()
    {
        _grid.BatchRevealed -= OnBatchRevealed;
        _grid.BombRevealed -= OnBombRevealed;
    }

    private void OnBombRevealed(List<Cell> bombsRevealed)
    {
        MessageBox.Print("Game over! Bomb revealed!");
        SaveNewRecord(false);

        SceneSwitcher.Instance.GotoScene(SceneName.Losing);
    }

    private void OnBatchRevealed()
    {
        if (_grid.IsResolved)
        {
            MessageBox.Print("You win!");
            SaveNewRecord(true);

            SceneSwitcher.Instance.GotoScene(SceneName.Winning);
        }
    }

    private void SaveNewRecord(bool winning)
    {
        var (startAt, endAt) = (HistoryManager.CurrentRecordStartAt, DateTime.Now);
        var record = new Record(startAt, endAt, winning);
        _history.AddRecord(record);
    }
}