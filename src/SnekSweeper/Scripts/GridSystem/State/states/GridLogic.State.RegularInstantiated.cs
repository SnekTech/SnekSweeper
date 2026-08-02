using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public record RegularInstantiated : GridState, IGet<Input.PlayerInput>, IGet<Input.StartLevel>
    {
        bool _hasFirstInputBeenHandled;

        void OnReadyToHandleFirstInput(GridInput firstInput)
        {
            Input(new Input.StartLevel());
            Input(new Input.PlayerInput(firstInput));
        }

        public Type On(in Input.PlayerInput input)
        {
            if (_hasFirstInputBeenHandled)
                return ToSelf();
            if (input.GridInput is not PrimaryReleased primaryReleased)
                return ToSelf();
            _hasFirstInputBeenHandled = true;

            TriggerGridInitAsync(LevelExitToken).Forget();

            return ToSelf();

            async GDTaskVoid TriggerGridInitAsync(CancellationToken ct = default)
            {
                var firstClickIndex = primaryReleased.Index;

                Context.RunRecorder.MarkRunStartInfo(new RunStartInfo(DateTime.Now, firstClickIndex));
                await Context.Grid.InitCellsAsync(Get<GridLogic.Data>().LoadLevelSource.LayMineFn(firstClickIndex), ct);

                OnReadyToHandleFirstInput(primaryReleased);
            }
        }

        public Type On(in Input.StartLevel input) => To<GameRunning>();
    }
}