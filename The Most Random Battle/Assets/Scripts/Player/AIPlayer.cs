using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : Player {

    private bool _isDecidingRandomly;

    private void Start() {
        _isControllablePlayer = false;

        Spawn();

        _interactableCells = new List<Cell>();

        _turn = new Turn(this);
    }

    public override void ChooseDirection() {
        if (_isDecidingRandomly) {
            float rand1 = Random.value;
            float rand2 = Random.value;
            _direction = new Vector2Int(rand1 > .5f ? 0 : rand2 < .5f ? 1 : -1, rand1 < .5f ? 0 : rand2 < .5f ? 1 : -1);
        } else {
            //get targeted player
            float closestSquaredDist = float.MaxValue;
            Player closestPlayer = null;
            foreach (Player player in gameManager.Players) {
                if (player == this || player.Turn.CurrentTurnState == TurnState.Dead) {
                    continue;
                }

                int squaredDist = (player.Coord.x - _coord.x) * (player.Coord.x - _coord.x) + (player.Coord.y - _coord.y) * (player.Coord.y - _coord.y);
                if (squaredDist < closestSquaredDist) {
                    closestSquaredDist = squaredDist;
                    closestPlayer = player;
                }
            }

            //calculate the direction
            if (closestPlayer != null) {
                int deltaX = closestPlayer.Coord.x - _coord.x;
                int deltaY = closestPlayer.Coord.y - _coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    _direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                } else {
                    _direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                }
            } else {
                Debug.Log("closest player is null");
                _direction = Vector2Int.zero;
            }
        }

        if (_turn.CurrentTurnState == TurnState.Fleeing) {
            void ChooseAnyOtherDirection() {
                float rand1 = Random.value;
                float rand2 = Random.value;
                Vector2Int newDirection = new Vector2Int(rand1 > .5f ? 0 : rand2 < .5f ? 1 : -1, rand1 < .5f ? 0 : rand2 < .5f ? 1 : -1);

                if (newDirection == _direction) {
                    ChooseAnyOtherDirection();
                } else {
                    _direction = newDirection;
                }
            }

            ChooseAnyOtherDirection();
        }
        
        _turn.Move();
    }

    public override void StartMovingProcess() {
        Move(Mathf.CeilToInt(Random.value * 6), _direction);
    }

    public override void Interact() {
        //check if interacting is possible
        if (IsInteractingPossible((int)_playerWeaponManager.EquipedWeapon.attackRadius)) {
            /*//Get the player to attack
            foreach (Player player in gameManager.Players) {
                if (player != this && player.Turn.CurrentTurnState != TurnState.Dead) {
                    if (player.Coord == _interactableCells[0].CellCoord) {
                        //attack player
                        _turn.Flee();
                        return;
                    }
                }
            }

            //Get chest to open
            foreach (Chest chest in gameManager.Chests) {
                if (chest.Coord == _interactableCells[0].CellCoord) {
                    //open chest
                    chest.OpenChest();
                    _turn.Flee();
                    return;
                }
            }*/

            //pick which cell to interact with
            Cell selectedCell = _interactableCells[0];

            if (selectedCell.player != null && selectedCell.player != this) {
                //interact
                Debug.Log("attacked player");

                //flee
                _turn.Flee();
            } else if (selectedCell.chest != null) {
                //open chest
                selectedCell.chest.OpenChest();

                //flee
                _turn.Flee();
            }
        } else {
            _turn.EndTurn();
        }
    }

    public override void Flee() {
        ChooseDirection();
    }

    private void Update() {
        if (_isInMotion) {
            InMotion();
        }
    }
}
