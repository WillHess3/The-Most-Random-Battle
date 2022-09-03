using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllablePlayer : Player {

    private bool _isGettingDirInput;
    private Vector2Int _direction;

    private bool _isWaitingOnDiceRoll;

    private void Start() {
        Spawn();

        _turn = new Turn(this);
        _turn.StartTurn();
    }

    public override void ChooseDirection() {
        _isGettingDirInput = true;
    }

    public override void StartMovingProcess() {
        _isWaitingOnDiceRoll = true;
    }

    public void OnDieRolled() {
        if (_isWaitingOnDiceRoll) {
            _isWaitingOnDiceRoll = false;
            Move(Mathf.CeilToInt(Random.value * 6), _direction);
            _turn.EndTurn();
        }
    }

    private void SetDirection(Vector2Int direction) {
        _direction = direction;
        _isGettingDirInput = false;
        _turn.Move();
    }

    private void Update() {
        if (_isGettingDirInput) {
            if (Input.GetKeyDown(KeyCode.W)) {
                SetDirection(new Vector2Int(0, 1));
            } else if (Input.GetKeyDown(KeyCode.A)) {
                SetDirection(new Vector2Int(-1, 0));
            } else if (Input.GetKeyDown(KeyCode.S)) {
                SetDirection(new Vector2Int(0, -1));
            } else if (Input.GetKeyDown(KeyCode.D)) {
                SetDirection(new Vector2Int(1, 0));
            }
        }
    }

    
}
