﻿using System.Threading.Tasks;

namespace SnekSweeper.CellSystem.StateMachine.States;

public class RevealedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override Task OnEnterAsync()
    {
        Cell.Cover.Reveal();
        return Task.CompletedTask;
    }

    public override Task OnExitAsync()
    {
        Cell.Cover.PutOn();
        return Task.CompletedTask;
    }

    public override Task RevealAsync()
    {
        return Task.CompletedTask;
    }

    public override Task SwitchFlagAsync()
    {
        return Task.CompletedTask;
    }

    public override Task PutOnCoverAsync()
    {
        return ChangeStateAsync<CoveredState>();
    }
}