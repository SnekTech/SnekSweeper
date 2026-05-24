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
            event Action? FirstInputHandled;
            bool _hasInitializedGrid;
            bool _isProcessingInput;
            readonly GridStateContext _context;

            public InstantiatedFromSnapshot()
            {
                OnAttach(() => FirstInputHandled += OnFirstInputHandled);
                OnDetach(() => FirstInputHandled -= OnFirstInputHandled);

                _context = Get<GridStateContext>();
                var fromGridSnapshot = (FromGridSnapshot)Get<Data>().LoadLevelSource;
                var (cellSnapshotStates, bombs) = fromGridSnapshot.Snapshot;
                var cellStatesMatrix = MatrixExtensions.FromJagged(cellSnapshotStates);

                this.OnEnter(delegate { TriggerInitGridAsync().Forget(); });

                return;
                
                async GDTask RestoreCellStatesAsync(CancellationToken ct = default)
                {
                    var tasks = _context.Grid.Cells
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
                    await _context.Grid.InitCellsAsync(bombs, ct);
                    await RestoreCellStatesAsync(ct);
                    _context.RunRecorder.MarkRunStartInfo(fromGridSnapshot.StartInfo);
                    _hasInitializedGrid = true;
                }
            }

            void OnFirstInputHandled() => Input(new Input.StartLevel());

            public Transition On(in Input.PlayerInput input)
            {
                if (!_hasInitializedGrid || _isProcessingInput)
                    return ToSelf();
                
                var gridInput = input.GridInput;

                TriggerHandleInputAsync().Forget();
                return ToSelf();

                async GDTaskVoid TriggerHandleInputAsync(CancellationToken ct = default)
                {
                    _isProcessingInput = true;
                    await _context.Grid.HandleInputAsync(gridInput, ct);
                    FirstInputHandled?.Invoke();
                    _isProcessingInput = false;
                }
            }

            public Transition On(in Input.StartLevel input) => To<GameRunning>();
        }
    }
}