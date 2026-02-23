namespace SnekSweeper.UI.Level;

public class PopupChoiceListener<T>
{
    readonly TaskCompletionSource<T> _tcs = new();
    readonly Dictionary<Button, Action> _handlerCache = [];

    public PopupChoiceListener(List<ButtonAndValue<T>> buttonConfigs)
    {
        foreach (var (button, value) in buttonConfigs)
        {
            _handlerCache.Add(button, () => _tcs.SetResult(value));
        }
    }

    public Task<T> GetChoiceAsync() => _tcs.Task;

    public void RegisterButtonListeners()
    {
        foreach (var button in _handlerCache.Keys)
        {
            button.Pressed += _handlerCache[button];
        }
    }

    public void UnregisterButtonListeners()
    {
        foreach (var button in _handlerCache.Keys)
        {
            button.Pressed -= _handlerCache[button];
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