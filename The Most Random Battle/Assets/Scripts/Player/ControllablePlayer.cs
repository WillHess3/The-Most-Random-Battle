using System.Collections.Generic;
using UnityEngine;

public class ControllablePlayer : Player {

    private bool _isGettingDirInput;

    private bool _isWaitingOnDiceRoll;

    private bool _isWaitingOnInteractableTileSelect;

    private void Start() {
        _isControllablePlayer = true;

        Spawn();

        _turn = new Turn(this);

        _interactableCells = new List<Cell>();

        DieRoller.RollDie += OnDieRolled;
    }

    public override void ChooseDirection() {
        _isGettingDirInput = true;
        Debug.Log("accepting Input");
    }

    private void SetDirection(Vector2Int direction) {
        _direction = direction;
        _isGettingDirInput = false;
        _turn.Move();
    }

    public override void StartMovingProcess() {
        _isWaitingOnDiceRoll = true;
    }

    public void OnDieRolled(int rolledNumber) {
        if (_isWaitingOnDiceRoll) {
            _isWaitingOnDiceRoll = false;
            Move(rolledNumber, _direction);
        }
    }

    public override void Interact() {
        //check if interacting is possible
        if (IsInteractingPossible(_playerWeaponManager.EquipedWeapon.attackRadius)) {
            //Get cell to interact with
            _isWaitingOnInteractableTileSelect = true;
        } else {
            _turn.EndTurn();
        }
    }

    public override void Flee() {
        ChooseDirection();
    }

    private void Update() {
        if (_isGettingDirInput) {
            if (Input.GetKeyDown(KeyCode.W)) {
                SetDirection(Vector2Int.up);
            } else if (Input.GetKeyDown(KeyCode.A)) {
                SetDirection(Vector2Int.left);
            } else if (Input.GetKeyDown(KeyCode.S)) {
                SetDirection(Vector2Int.down);
            } else if (Input.GetKeyDown(KeyCode.D)) {
                SetDirection(Vector2Int.right);
            }
        }

        if (_isInMotion) {
            InMotion();
        }

        //get cell to interact with
        if (_isWaitingOnInteractableTileSelect) {
            if (Input.GetMouseButtonDown(0)) {
                Vector2Int selectedCellCoord = GridCreator.instance.Grid.GetCellCoordFromClick(Input.mousePosition);

                if (selectedCellCoord.x < 0 || selectedCellCoord.y < 0 || selectedCellCoord.x >= gridInformation.GridLength || selectedCellCoord.y >= gridInformation.GridHeight) {
                    return;
                }

                Cell selectedCell = GridCreator.instance.Grid.GetCellAtCoord(selectedCellCoord);

                if (selectedCell.CellState == CellState.Blocked && _interactableCells.Contains(selectedCell)) {
                    //find out whats on the cell
                    foreach (Player player in GameManager.Players) {
                        if (player != this && player.Turn.CurrentTurnState != TurnState.Dead) {
                            if (player.Coord == selectedCell.CellCoord) {
                                //interact
                                Debug.Log("attacked player");
                                _isWaitingOnInteractableTileSelect = false;

                                //flee
                                _turn.Flee();
                            }
                        }
                    }
                } else {
                    _turn.EndTurn();
                }
            }
        }
    }

    private void OnDestroy() {
        DieRoller.RollDie -= OnDieRolled;
    }
}
