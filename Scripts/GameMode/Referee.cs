using System;
using System.Collections.Generic;
using Godot;
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
        GD.Print("Game over! Bomb revealed!");
        SaveNewRecord(false);

        SceneManager.Instance.GotoScene(ScenePaths.LosingScene);
    }

    private void OnBatchRevealed()
    {
        if (_grid.IsResolved)
        {
            GD.Print("You win!");
            SaveNewRecord(true);

            SceneManager.Instance.GotoScene(ScenePaths.WinningScene);
        }
    }

    private void SaveNewRecord(bool winning)
    {
        var (startAt, endAt) = (HistoryManager.CurrentRecordStartAt, DateTime.Now);
        var record = new Record(startAt, endAt, winning);
        _history.AddRecord(record);
    }
}