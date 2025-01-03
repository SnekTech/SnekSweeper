namespace Widgets;

public static class TaskExtensions
{
    public static async void Fire(this Task task, Action? onComplete = null, Action<Exception>? onError = null)
    {
        try
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Console.WriteLine("something wrong during fire & forget: ");
                Console.WriteLine(e);
                onError?.Invoke(e);
            }

            onComplete?.Invoke();
        }
        catch (Exception e)
        {
            Console.WriteLine("something wrong on fire & forget complete : ");
            onError?.Invoke(e);
        }
    }
}