using GodotGadgets.ShaderStuff;
using GTweens.Extensions;
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
        dissolveProgress = shaderMaterial.GetUniform<float>("progress");
        coverAlpha = shaderMaterial.GetUniform<float>("coverAlpha");
        noiseSeed = shaderMaterial.GetUniform<float>("noiseSeed");

        SetDissolveProgress(0);
    }

    public async Task RevealAsync(CancellationToken ct = default)
    {
        RandomizeNoise();
        var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 1, AnimationDuration);
        await tween.PlayAsyncUntilNodeDestroy(this, ct);
        Hide();
    }

    public async Task PutOnAsync(CancellationToken ct = default)
    {
        RandomizeNoise();
        Show();
        var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 0, AnimationDuration);
        await tween.PlayAsyncUntilNodeDestroy(this, ct);
    }

    public void SetAlpha(float normalizedAlpha) => coverAlpha.Value = normalizedAlpha;

    float GetDissolveProgress() => dissolveProgress.Value;
    void SetDissolveProgress(float progress) => dissolveProgress.Value = progress;

    void RandomizeNoise() => noiseSeed.Value = GD.Randf();
}