﻿using System;
using System.Collections.Generic;
using System.Linq;
using SnekSweeper.CellSystem;
using SnekSweeper.GameHistory;

namespace SnekSweeper.GridSystem;

public class Grid
{
    public event Action<List<Cell>>? BombRevealed;
    public event Action? BatchRevealed;

    private static readonly (int offsetI, int offsetJ)[] NeighborOffsets =
    {
        (-1, -1),
        (0, -1),
        (1, -1),
        (-1, 0),
        (1, 0),
        (-1, 1),
        (0, 1),
        (1, 1),
    };

    private readonly Cell[,] _cells;
    private readonly IHumbleGrid _humbleGrid;
    private readonly IGridDifficulty _gridDifficulty;
    private bool _hasCellInitialized;

    public Grid(IHumbleGrid humbleGrid, IGridDifficulty gridDifficulty)
    {
        _humbleGrid = humbleGrid;
        _gridDifficulty = gridDifficulty;

        var (rows, columns) = gridDifficulty.Size;
        _cells = new Cell[rows, columns];

        InstantiateHumbleCells();

        _humbleGrid.PrimaryReleased += OnPrimaryReleasedAt;
        _humbleGrid.PrimaryDoubleClicked += OnPrimaryDoubleClickedAt;
        _humbleGrid.SecondaryReleased += OnSecondaryReleasedAt;
    }

    private void InstantiateHumbleCells()
    {
        var humbleCells = _humbleGrid.InstantiateHumbleCells(_cells.Length);
        foreach (var (i, j) in _cells.Indices())
        {
            var humbleCell = humbleCells[i * _cells.Size().columns + j];
            var cell = new Cell(humbleCell, (i, j));
            _cells[i, j] = cell;
        }
    }

    private void InitCells((int i, int j) firstClickGridIndex)
    {
        var bombMatrix = new BombMatrix(_gridDifficulty);
        bombMatrix.ClearBombAt(firstClickGridIndex);
        foreach (var (i, j) in _cells.Indices())
        {
            _cells[i, j].HasBomb = bombMatrix[i, j];
        }

        // must init individual cells after bombs planted
        foreach (var cell in _cells)
        {
            var neighborBombCount = GetNeighborsOf(cell).Count(neighbor => neighbor.HasBomb);
            cell.Init(neighborBombCount);
        }
        _hasCellInitialized = true;

        HistoryManager.CurrentRecordStartAt = DateTime.Now;
    }

    public void OnDispose()
    {
        _humbleGrid.PrimaryReleased -= OnPrimaryReleasedAt;
        _humbleGrid.PrimaryDoubleClicked -= OnPrimaryDoubleClickedAt;
        _humbleGrid.SecondaryReleased -= OnSecondaryReleasedAt;
    }

    private void OnPrimaryReleasedAt((int i, int j) gridIndex)
    {
        if (!_hasCellInitialized)
        {
            InitCells(gridIndex);
        }

        RevealAt(gridIndex);
    }

    private void OnPrimaryDoubleClickedAt((int i, int j) gridIndex)
    {
        RevealAround(gridIndex);
    }

    private void OnSecondaryReleasedAt((int i, int j) gridIndex)
    {
        GetCellAt(gridIndex).SwitchFlag();
    }

    private Cell GetCellAt((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        return _cells[i, j];
    }

    public bool IsValidIndex((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        var (rows, columns) = _cells.Size();
        return i >= 0 && i < rows && j >= 0 && j < columns;
    }

    private IEnumerable<Cell> GetNeighborsOf(Cell cell)
    {
        var (i, j) = cell.GridIndex;
        foreach (var (offsetI, offsetJ) in NeighborOffsets)
        {
            var neighborIndex = (i + offsetI, j + offsetJ);
            if (IsValidIndex(neighborIndex))
            {
                yield return GetCellAt(neighborIndex);
            }
        }
    }

    private void RevealAt((int i, int j) gridIndex)
    {
        var cellsToReveal = new HashSet<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);

        var bombCellsRevealed = new List<Cell>();
        foreach (var cell in cellsToReveal)
        {
            cell.Reveal();
            if (cell.HasBomb)
            {
                bombCellsRevealed.Add(cell);
            }
        }

        if (bombCellsRevealed.Count > 0)
        {
            BombRevealed?.Invoke(bombCellsRevealed);
        }

        BatchRevealed?.Invoke();
    }

    private void RevealAround((int i, int j) gridIndex)
    {
        var cell = GetCellAt(gridIndex);
        var canRevealAround = cell is { IsRevealed: true, HasBomb: false };
        if (!canRevealAround)
            return;

        var neighbors = GetNeighborsOf(cell).ToList();
        var flaggedNeighborCount = neighbors.Count(neighbor => neighbor.IsFlagged);
        if (flaggedNeighborCount != cell.NeighborBombCount)
            return;

        var cellsToReveal = new HashSet<Cell>();
        foreach (var neighbor in neighbors)
        {
            FindCellsToReveal(neighbor.GridIndex, cellsToReveal);
        }

        foreach (var c in cellsToReveal)
        {
            c.Reveal();
        }

        BatchRevealed?.Invoke();
    }

    private void FindCellsToReveal((int i, int j) gridIndex, ICollection<Cell> cellsToReveal)
    {
        if (!IsValidIndex(gridIndex))
            return;

        var cell = GetCellAt(gridIndex);
        var visited = cellsToReveal.Contains(cell);
        if (visited || !cell.IsCovered)
            return;

        cellsToReveal.Add(cell);

        if (cell.HasBomb || cell.NeighborBombCount > 0)
            return;

        foreach (var neighbor in GetNeighborsOf(cell))
        {
            FindCellsToReveal(neighbor.GridIndex, cellsToReveal);
        }
    }

    public bool IsResolved
    {
        get
        {
            foreach (var cell in _cells)
            {
                if (cell is { HasBomb: false, IsRevealed: false })
                {
                    return false;
                }
            }

            return true;
        }
    }
}