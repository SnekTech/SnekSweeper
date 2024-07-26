using System;
using Godot;
using SnekSweeper.Constants;

namespace SnekSweeper.Autoloads;

public partial class InputListener : Node
{
    public event Action LevelRestarted;

    private const float SecondsRestartThreshold = 1;
    private const string RestartActionName = "restart";

    private bool _isRestartPressed;
    private bool _countingRestartHold;
    private float _restartHoldTime;


    public override void _Ready()
    {
        LevelRestarted += OnLevelRestarted;
    }

    public override void _ExitTree()
    {
        LevelRestarted -= OnLevelRestarted;
    }

    private void OnLevelRestarted()
    {
        GD.Print("restart");
        SceneManager.Instance.GotoScene(ScenePaths.MainScene);
    }

    public override void _Process(double delta)
    {
        ProcessRestartInput(delta);
    }

    private void ProcessRestartInput(double delta)
    {
        if (Input.IsActionJustPressed(RestartActionName))
        {
            _countingRestartHold = true;
            _restartHoldTime = 0;
        }
        
        if (_countingRestartHold && Input.IsActionPressed(RestartActionName))
        {
            _restartHoldTime += (float)delta;

            if (_restartHoldTime > SecondsRestartThreshold)
            {
                LevelRestarted?.Invoke();
                _countingRestartHold = false;
            }
        }

        if (Input.IsActionJustReleased(RestartActionName))
        {
            _countingRestartHold = false;
        }
    }
}