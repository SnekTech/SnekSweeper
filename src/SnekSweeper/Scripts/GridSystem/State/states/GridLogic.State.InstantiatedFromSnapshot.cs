using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record InstantiatedFromSnapshot : State, IGet<Input.PlayerInput>, IGet<Input.StartLevel>
        {
            bool _hasInitializedGrid;
            FromGridSnapshot _fromGridSnapshot = null!;

            public InstantiatedFromSnapshot()
            {
                this.OnEnter(delegate
                {
                    _fromGridSnapshot = (FromGridSnapshot)Get<Data>().LoadLevelSource;
                    TriggerInitGridAsync().Forget();
                });

                return;

                async GDTask RestoreCellStatesAsync(CancellationToken ct = default)
                {
                    var cellStatesMatrix = MatrixExtensions.FromJagged(_fromGridSnapshot.Snapshot.SnapshotStates);

                    var tasks = Context.Grid.Cells
                        .Select(cell => (cell, state: cellStatesMatrix.At(cell.GridIndex)))
                        .Select(tuple => tuple.state switch
                        {
                            CellSnapshotState.Revealed => tuple.cell.RevealAsync(ct).AsGDTask(),
                            CellSnapshotState.Flagged => tuple.cell.SwitchFlagAsync(ct).AsGDTask(),
                            _ => GDTask.CompletedTask,
                        });

                    await GDTask.WhenAll(tasks);
                }

                async GDTaskVoid TriggerInitGridAsync(CancellationToken ct = default)
                {
                    await Context.Grid.InitCellsAsync(_fromGridSnapshot.Snapshot.BombMatrix, ct);
                    await RestoreCellStatesAsync(ct);
                    Context.RunRecorder.MarkRunStartInfo(_fromGridSnapshot.StartInfo);
                    _hasInitializedGrid = true;
                }
            }

            void OnReadyToHandleFirstInput(GridInput firstInput)
            {
                Input(new Input.StartLevel());
                Input(new Input.PlayerInput(firstInput));
            }

            public Transition On(in Input.PlayerInput input)
            {
                if (!_hasInitializedGrid)
                    return ToSelf();

                OnReadyToHandleFirstInput(input.GridInput);

                return ToSelf();
            }

            public Transition On(in Input.StartLevel input) => To<GameRunning>();
        }
    }
}