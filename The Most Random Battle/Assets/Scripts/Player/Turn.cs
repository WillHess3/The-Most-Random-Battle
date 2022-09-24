using System.Collections.Generic;
using UnityEngine;

public class Turn {

    private TurnState _currentTurnState;
    public TurnState CurrentTurnState => _currentTurnState;

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
        _currentTurnState = _currentTurnState == TurnState.Fleeing ? TurnState.Fleeing : TurnState.Moving;
        _player.StartMovingProcess();
    }

    public void MovingComplete() {
        Debug.Log("moving complete. State: " + _currentTurnState);
        if (_currentTurnState != TurnState.Fleeing) {
            _currentTurnState = TurnState.Interacting;
            _player.Interact();
        } else {
            EndTurn();
        }
    }

    public void Flee() {
        _currentTurnState = TurnState.Fleeing;
        _player.Flee();
    }

    public void EndTurn() {
        _currentTurnState = TurnState.Done;
    }

    public void ConfirmTurnFinished() {
        Debug.Log("done with turn");
        _currentTurnState = TurnState.WaitingForTurn;
    }

}

public enum TurnState {
    WaitingForTurn,
    ChoosingDirection,
    Moving,
    Interacting,
    Fleeing,
    Done,
    Dead
}
