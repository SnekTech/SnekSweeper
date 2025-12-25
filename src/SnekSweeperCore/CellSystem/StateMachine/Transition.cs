namespace SnekSweeperCore.CellSystem.StateMachine;

public record Transition(CellState To, CellRequest Request);