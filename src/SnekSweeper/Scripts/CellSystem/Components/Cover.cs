using GTweens.Extensions;
using GTweensGodot.Extensions;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem.Components;

namespace SnekSweeper.CellSystem.Components;

[SceneTree]
public partial class Cover : Node2D, ICover, ISceneScript
{
    private const float AnimationDuration = .4f;

    private ShaderMaterial _shaderMaterial = null!;

    public override void _Ready()
    {
        _shaderMaterial = (ShaderMaterial)_.Sprite.Material;
        SetDissolveProgress(0);
    }

    public async Task RevealAsync()
    {
        RandomizeNoise();
        var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 1, AnimationDuration);
        await tween.PlayAsync(CancellationToken.None);
        Hide();
    }

    public async Task PutOnAsync()
    {
        RandomizeNoise();
        Show();
        var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 0, AnimationDuration);
        await tween.PlayAsync(CancellationToken.None);
    }

    public void SetAlpha(float normalizedAlpha) =>
        _shaderMaterial.SetShaderParameter(Uniforms.Alpha, normalizedAlpha);

    private float GetDissolveProgress() =>
        (float)_shaderMaterial.GetShaderParameter(Uniforms.DissolveProgress).AsDouble();

    private void SetDissolveProgress(float progress) =>
        _shaderMaterial.SetShaderParameter(Uniforms.DissolveProgress, progress);

    private void RandomizeNoise()
    {
        var noiseTexture = (NoiseTexture2D)_shaderMaterial.GetShaderParameter(Uniforms.Mask);
        var noiseLite = (FastNoiseLite)noiseTexture.Noise;
        noiseLite.Seed = (int)GD.Randi();
    }

    private static class Uniforms
    {
        public static readonly StringName DissolveProgress = "progress";
        public static readonly StringName Alpha = "coverAlpha";
        public static readonly StringName Mask = "mask";
    }
}