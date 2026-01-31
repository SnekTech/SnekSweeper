using GodotGadgets.ShaderStuff;
using GodotGadgets.Tasks;
using GTweens.Extensions;
using GTweensGodot.Extensions;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem.Components;

namespace SnekSweeper.CellSystem.Components;

[SceneTree]
public partial class Cover : Node2D, ICover, ISceneScript
{
    const float AnimationDuration = .2f;

    Uniform<float> dissolveProgress = null!;
    Uniform<float> coverAlpha = null!;
    Uniform<float> noiseSeed = null!;

    public override void _Ready()
    {
        var shaderMaterial = (ShaderMaterial)_.Sprite.Material;
        dissolveProgress = new Uniform<float>(shaderMaterial, "progress");
        coverAlpha = new Uniform<float>(shaderMaterial, "coverAlpha");
        noiseSeed = new Uniform<float>(shaderMaterial, "noiseSeed");

        SetDissolveProgress(0);
    }

    public async Task RevealAsync(CancellationToken cancellationToken = default)
    {
        var tokenLinkedWithTreeExit = cancellationToken.LinkWithNodeDestroy(this);

        RandomizeNoise();
        var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 1, AnimationDuration);
        await tween.PlayAsync(tokenLinkedWithTreeExit.Token);
        Hide();
    }

    public async Task PutOnAsync(CancellationToken cancellationToken = default)
    {
        var tokenLinkedWithTreeExit = cancellationToken.LinkWithNodeDestroy(this);

        RandomizeNoise();
        Show();
        var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 0, AnimationDuration);
        await tween.PlayAsync(tokenLinkedWithTreeExit.Token);
    }

    public void SetAlpha(float normalizedAlpha) => coverAlpha.Value = normalizedAlpha;

    float GetDissolveProgress() => dissolveProgress.Value;
    void SetDissolveProgress(float progress) => dissolveProgress.Value = progress;

    void RandomizeNoise() => noiseSeed.Value = GD.Randf();
}