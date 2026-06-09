using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Tutorial;

public record ExampleDescription(string Title, IReadOnlyList<string> Sentences);
public record ExampleData(GridSnapshot Snapshot, ExampleDescription Description);

public static class TutorialExampleCollection
{
    public static IReadOnlyList<ExampleData> BuiltinExamples => ExamplesDataRaw;
    
    static readonly ExampleData[] ExamplesDataRaw =
    [
        new(
            new GridSnapshot(
                [
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Covered, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Flagged, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Flagged, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Flagged, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Covered,
                        CellSnapshotState.Covered, CellSnapshotState.Covered
                    ],
                ],
                new[,]
                {
                    { false, false, false, false, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { true, false, false, false, false },
                }
            ),
            new ExampleDescription("数数-1", [
                "如果一个数字周围只剩下对应数量的格子",
                "那么这些格子必须全是雷",
            ])
        ),
        new(
            new GridSnapshot(
                [
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Flagged, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Flagged, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Flagged, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Covered,
                        CellSnapshotState.Covered, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Covered,
                        CellSnapshotState.Covered, CellSnapshotState.Covered
                    ],
                ],
                new[,]
                {
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { true, false, false, false, false },
                    { false, false, false, false, false },
                }
            ),
            new ExampleDescription("数数-2", [
                "如果一个数字周围已经有了对应数量的雷",
                "那么剩下的格子全部安全",
            ])
        ),
        new(
            new GridSnapshot(
                [
                    [
                        CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Covered,
                        CellSnapshotState.Covered, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Covered,
                        CellSnapshotState.Covered, CellSnapshotState.Covered
                    ],
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed
                    ],
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed
                    ],
                    [
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed,
                        CellSnapshotState.Revealed, CellSnapshotState.Revealed
                    ],
                ],
                new[,]
                {
                    { false, false, false, false, false },
                    { true, false, false, true, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                }
            ),
            new ExampleDescription("11定式-1", [
                "看最左边的 1",
                "它周围有两个黄格，所以这两个黄格有 1 个雷",
                "再看到左边第二个 1",
                "它周围的两个黄格已经有了 1 个雷，所以剩下一格必定安全",
            ])
        ),
    ];
}