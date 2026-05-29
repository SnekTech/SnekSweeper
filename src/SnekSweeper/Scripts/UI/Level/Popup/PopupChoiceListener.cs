using GodotTask;

namespace SnekSweeper.UI.Level.Popup;

public class PopupChoiceListener<T>
{
    readonly GDTaskCompletionSource<T> _tcs = new();
    readonly Dictionary<Button, Action> _handlerCache = [];

    public PopupChoiceListener(List<ButtonAndValue<T>> buttonConfigs)
    {
        foreach (var (button, value) in buttonConfigs)
        {
            _handlerCache.Add(button, () => _tcs.TrySetResult(value));
        }
    }

    public GDTask<T> GetChoiceAsync(CancellationToken cancellationToken = default)
    {
        CancellationTokenRegistration ctr = default;
        if (cancellationToken.CanBeCanceled)
        {
            ctr = cancellationToken.Register(() =>
            {
                _tcs.TrySetCanceled(cancellationToken);
            });
        }

        _tcs.Task.ContinueWith(() => ctr.Dispose());
        
        return _tcs.Task;
    }

    public void RegisterButtonListeners()
    {
        foreach (var (button, handler) in _handlerCache)
        {
            button.Pressed += handler;
        }
    }

    public void UnregisterButtonListeners()
    {
        foreach (var (button, handler) in _handlerCache)
        {
            button.Pressed -= handler;
        }
    }
}

public readonly record struct ButtonAndValue<T>(Button ChoiceButton, T Value);

public static class ButtonAndValueExtensions
{
    extension(Button button)
    {
        public ButtonAndValue<T> CreateChoiceButton<T>(T value)
            => new(button, value);
    }
}