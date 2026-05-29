using GodotTask;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record RegularInstantiated : State, IGet<Input.PlayerInput>, IGet<Input.StartLevel>
        {
            bool _hasFirstInputBeenHandled;

            void OnReadyToHandleFirstInput(GridInput firstInput)
            {
                Input(new Input.StartLevel());
                Input(new Input.PlayerInput(firstInput));
            }

            public Transition On(in Input.PlayerInput input)
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
                    await Context.Grid.InitCellsAsync(Get<Data>().LoadLevelSource.LayMineFn(firstClickIndex), ct);
                    
                    OnReadyToHandleFirstInput(primaryReleased);
                }
            }

            public Transition On(in Input.StartLevel input) => To<GameRunning>();
        }
    }
}