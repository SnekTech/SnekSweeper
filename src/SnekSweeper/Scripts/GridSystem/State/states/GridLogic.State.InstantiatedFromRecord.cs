using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record InstantiatedFromRecord : State, IGet<Input.PlayerInput>, IGet<Input.StartLevel>
        {
            event Action? FirstInputHandled;
            bool _hasInitializedGrid;
            bool _isProcessingInput;
            readonly GameRunRecord _runRecord;
            readonly GridStateContext _context;

            public InstantiatedFromRecord()
            {
                OnAttach(() => FirstInputHandled += OnFirstInputHandled);
                OnDetach(() => FirstInputHandled -= OnFirstInputHandled);

                _context = Get<GridStateContext>();
                var fromRunRecord = (FromRunRecord)Get<Data>().LoadLevelSource;
                _runRecord = fromRunRecord.RunRecord;

                this.OnEnter(delegate { TriggerInitGridAsync().Forget(); });

                this.OnExit(delegate { _context.HumbleGrid.GridCursor.Unlock(); });

                return;

                async GDTaskVoid TriggerInitGridAsync(CancellationToken ct = default)
                {
                    await _context.Grid.InitCellsAsync(_runRecord.BombMatrix, ct);
                    _context.HumbleGrid.GridCursor.LockTo(_runRecord.StartIndex, _context.Grid.Size);
                    _hasInitializedGrid = true;
                }
            }

            void OnFirstInputHandled() => Input(new Input.StartLevel());

            public Transition On(in Input.PlayerInput input)
            {
                if (!_hasInitializedGrid || _isProcessingInput)
                    return ToSelf();
                var gridInput = input.GridInput;

                if (gridInput.Index != _runRecord.StartIndex)
                    return ToSelf();

                TriggerHandleInputAsync().Forget();
                return ToSelf();

                async GDTaskVoid TriggerHandleInputAsync(CancellationToken ct = default)
                {
                    _context.RunRecorder.MarkRunStartInfo(new RunStartInfo(DateTime.Now, gridInput.Index));
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