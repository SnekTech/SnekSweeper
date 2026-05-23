using GodotTask;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record RegularInstantiated : State, IGet<Input.PlayerInput>, IGet<Input.InitComplete>
        {
            event Action? InitCompleted;
            
            public RegularInstantiated()
            {
                OnAttach(() => InitCompleted += OnInitCompleted);
                OnDetach(() => InitCompleted -= OnInitCompleted);
            }

            void OnInitCompleted()
            {
                Input(new Input.InitComplete());
            }

            public Transition On(in Input.PlayerInput input)
            {
                if (input.GridInput is not PrimaryReleased primaryReleased) return ToSelf();

                TriggerGridInitAsync().Forget();

                return ToSelf();
                
                async GDTaskVoid TriggerGridInitAsync(CancellationToken ct = default)
                {
                    var context = Get<GridStateContext>();
                    var firstClickIndex = primaryReleased.Index;
                
                    context.RunRecorder.MarkRunStartInfo(new RunStartInfo(DateTime.Now, firstClickIndex));
                    // todo: handle the cancellation token
                    await context.Grid.InitCellsAsync(Get<Data>().LoadLevelSource.LayMineFn(firstClickIndex), ct);
                    await context.Grid.HandleInputAsync(primaryReleased, ct);
                    InitCompleted?.Invoke();
                }
            }

            public Transition On(in Input.InitComplete input) => To<GameRunning>();
        }
    }
}