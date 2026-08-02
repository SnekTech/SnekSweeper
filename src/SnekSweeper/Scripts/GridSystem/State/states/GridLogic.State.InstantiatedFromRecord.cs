using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public record InstantiatedFromRecord : GridState, IGet<Input.PlayerInput>, IGet<Input.StartLevel>
    {
        bool _hasInitializedGrid;
        GameRunRecord _runRecord = null!;

        public InstantiatedFromRecord()
        {
            this.OnEnter(delegate
            {
                var fromRunRecord = (FromRunRecord)Get<GridLogic.Data>().LoadLevelSource;
                _runRecord = fromRunRecord.RunRecord;
                Context.HumbleGrid.GridCursor.LockTo(_runRecord.StartIndex, Context.Grid.Size);

                TriggerInitGridAsync(LevelExitToken).Forget();
            });

            this.OnExit(delegate { Context.HumbleGrid.GridCursor.Unlock(); });

            return;

            async GDTaskVoid TriggerInitGridAsync(CancellationToken ct = default)
            {
                await Context.Grid.InitCellsAsync(_runRecord.BombMatrix, ct);
                _hasInitializedGrid = true;
            }
        }

        void OnReadyToHandleFirstInput(GridInput firstInput)
        {
            Input(new Input.StartLevel());
            Input(new Input.PlayerInput(firstInput));
        }

        public Type On(in Input.PlayerInput input)
        {
            if (!_hasInitializedGrid)
                return ToSelf();
            var gridInput = input.GridInput;

            if (gridInput.Index != _runRecord.StartIndex)
                return ToSelf();

            Context.RunRecorder.MarkRunStartInfo(new RunStartInfo(DateTime.Now, gridInput.Index));
            OnReadyToHandleFirstInput(gridInput);
            return ToSelf();
        }

        public Type On(in Input.StartLevel input) => To<GameRunning>();
    }
}