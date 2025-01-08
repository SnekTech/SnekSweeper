using System.Threading;
using System.Threading.Tasks;
using Godot;
using GodotUtilities;
using GTweens.Extensions;
using GTweensGodot.Extensions;

namespace SnekSweeper.CellSystem.Components;

[Scene]
public partial class Cover : Node2D, ICover
{
    [Node] private Sprite2D sprite = null!;
    private const float AnimationDuration = .4f;

    private ShaderMaterial _shaderMaterial = null!;
    private static readonly StringName DissolveProgressName = "progress";
    private static readonly StringName MaskName = "mask";

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        _shaderMaterial = (ShaderMaterial)sprite.Material;
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

    private float GetDissolveProgress() =>
        (float)_shaderMaterial.GetShaderParameter(DissolveProgressName).AsDouble();

    private void SetDissolveProgress(float progress) =>
        _shaderMaterial.SetShaderParameter(DissolveProgressName, progress);

    private void RandomizeNoise()
    {
        var noiseTexture = (NoiseTexture2D)_shaderMaterial.GetShaderParameter(MaskName);
        var noiseLite = (FastNoiseLite)noiseTexture.Noise;
        noiseLite.Seed = (int)GD.Randi();
    }
}