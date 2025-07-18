using GTweensGodot.Extensions;

namespace SnekSweeper;

public static class CustomExtensions
{
    public static async void Fire(this Task task, Action? onComplete = null, Action<Exception>? onError = null)
    {
        try
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException e)
            {
                GD.Print("---------- Under Control -----------");
                GD.Print($"A task was canceled, token {e.CancellationToken}");
                GD.Print("---------- Under Control -----------");
            }
            catch (Exception e)
            {
                GD.PrintErr("something wrong during fire & forget: ");
                GD.PrintErr(e);
                onError?.Invoke(e);
            }

            onComplete?.Invoke();
        }
        catch (Exception e)
        {
            GD.PrintErr("something wrong on fire & forget complete : ");
            GD.PrintErr(e);
            onError?.Invoke(e);
        }
    }

    public static void SetModulateAlpha(this CanvasItem canvasItem, float alpha)
    {
        canvasItem.Modulate = canvasItem.Modulate with { A = alpha };
    }

    public static async Task ShakeAsync(this Node2D node2D, float strength, float duration = 0.2f)
    {
        var originalPosition = node2D.Position;
        const int shakeCount = 10;

        for (var i = 0; i < shakeCount; i++)
        {
            var offset = GetRandomOffset();
            var targetPosition = originalPosition + offset * strength;
            if (i % 2 == 0)
            {
                targetPosition = originalPosition;
            }

            await node2D.TweenPosition(targetPosition, duration / shakeCount).PlayAsync(CancellationToken.None);
            strength *= 0.75f;
        }

        node2D.Position = originalPosition;

        return;

        Vector2 GetRandomOffset()
        {
            return new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1);
        }
    }

    public static void SetStyleBox(this Panel panel, StyleBox styleBox)
    {
        const string panelStylePath = "theme_override_styles/panel";
        panel.Set(panelStylePath, styleBox);
    }

    public static T PickRandom<T>(this List<T> list)
    {
        var randomIndex = GD.RandRange(0, list.Count - 1);
        return list[randomIndex];
    }

    public static void Shuffle<T>(this List<T> list)
    {
        var random = new Random();
        var array = list.ToArray();
        random.Shuffle(array);
        list.Clear();
        list.AddRange(array);
    }

    public static void ClearChildren(this Node node)
    {
        foreach (var child in node.GetChildren())
        {
            child.QueueFree();
        }
    }

    public static IEnumerable<T> GetChildrenOfType<T>(this Node node)
    {
        return node.GetChildren().OfType<T>();
    }
}