using System.Collections.Generic;
using UnityEngine;

public class Turn {

    private TurnState _currentTurnState;

    private Player _player;

    public Turn(Player player) {
        _player = player;
    }

    public void StartTurn() {
        if (_currentTurnState != TurnState.WaitingForTurn) {
            Debug.Log("Turn started when in turn");
            return;
        }

        _currentTurnState = TurnState.ChoosingDirection;

        _player.ChooseDirection();
    }

    public void Move() {
        _currentTurnState = TurnState.Moving;
        _player.StartMovingProcess();
    }

    public void EndTurn() {
        _currentTurnState = TurnState.WaitingForTurn;
        StartTurn();
    }

}

public enum TurnState {
    WaitingForTurn,
    ChoosingDirection,
    Moving,
    Interacting,
    Fleeing
}
