using GodotTask;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record RegularInstantiated : State, IGet<Input.PlayerInput>, IGet<Input.StartLevel>
        {
            event Action? FirstInputHandled;
            bool _isProcessingInput;
            
            public RegularInstantiated()
            {
                OnAttach(() => FirstInputHandled += OnFirstInputHandled);
                OnDetach(() => FirstInputHandled -= OnFirstInputHandled);
            }

            void OnFirstInputHandled() => Input(new Input.StartLevel());

            public Transition On(in Input.PlayerInput input)
            {
                if (_isProcessingInput)
                    return ToSelf();
                if (input.GridInput is not PrimaryReleased primaryReleased) 
                    return ToSelf();

                // todo: handle the cancellation token
                TriggerGridInitAsync().Forget();

                return ToSelf();
                
                async GDTaskVoid TriggerGridInitAsync(CancellationToken ct = default)
                {
                    _isProcessingInput = true;
                    
                    var context = Get<GridStateContext>();
                    var firstClickIndex = primaryReleased.Index;
                    
                    context.RunRecorder.MarkRunStartInfo(new RunStartInfo(DateTime.Now, firstClickIndex));
                    await context.Grid.InitCellsAsync(Get<Data>().LoadLevelSource.LayMineFn(firstClickIndex), ct);
                    await context.Grid.HandleInputAsync(primaryReleased, ct);
                    FirstInputHandled?.Invoke();
                    
                    _isProcessingInput = false;
                }
            }

            public Transition On(in Input.StartLevel input) => To<GameRunning>();
        }
    }
}