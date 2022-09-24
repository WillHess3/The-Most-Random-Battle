using System.Collections.Generic;
using UnityEngine;
using System;

public class ControllablePlayer : Player {

    private bool _isGettingDirInput;

    private bool _isWaitingOnDiceRoll;

    private bool _isWaitingOnInteractableTileSelect;

    public static event Action<Player> ReplaceWeaponStart;
    private bool _acceptingReplacementInput;

    private void Start() {
        _isControllablePlayer = true;

        Spawn();

        _turn = new Turn(this);

        _interactableCells = new List<Cell>();

        DieRoller.RollDie += OnDieRolled;

        WeaopnPowerUpDisplayer.ReplaceInputRecieved += OnWeaponReplaceButtonPressed;
    }

    public override void ChooseDirection() {
        _isGettingDirInput = true;
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
        if (IsInteractingPossible((int)_playerWeaponManager.EquipedWeapon.WeaponScriptableObject.attackRadius)) {
            //Get cell to interact with
            _isWaitingOnInteractableTileSelect = true;
        } else {
            _turn.EndTurn();
        }
    }

    public override void Flee() {
        ChooseDirection();
    }

    public override void ReplaceWeapon() {
        _acceptingReplacementInput = true;
        ReplaceWeaponStart?.Invoke(this);
    }

    private void OnWeaponReplaceButtonPressed(Player player, int replaceIndex) {
        if (!_acceptingReplacementInput || player != this) {
            return;
        }

        _playerWeaponManager.Inventory[replaceIndex].Drop(_pickupableObject.Coord);
        PickedUpWeapon?.Invoke(this, _pickupableObject.gameObject, replaceIndex);
        PickedUpWeaponVisualEvent?.Invoke(this);

        _isStopAtWeapon = false;
        _isCurrentlyStoppedAtWeapon = false;
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
                    /*foreach (Player player in gameManager.Players) {
                        if (player != this && player.Turn.CurrentTurnState != TurnState.Dead) {
                            if (player.Coord == selectedCell.CellCoord) {
                                //interact
                                Debug.Log("attacked player");
                                _isWaitingOnInteractableTileSelect = false;

                                //flee
                                _turn.Flee();
                            }
                        }
                    }*/

                    //Get chest to open
                    /*foreach (Chest chest in gameManager.Chests) {
                        if (chest.Coord == _interactableCells[0].CellCoord) {
                            //open chest
                            chest.OpenChest();
                            _turn.Flee();
                            return;
                        }
                    }*/

                    if (selectedCell.player != null && selectedCell.player != this) {
                        //interact
                        Debug.Log("attacked player");
                        _isWaitingOnInteractableTileSelect = false;

                        //flee
                        _turn.Flee();
                    } else if (selectedCell.chest != null) {
                        //open chest
                        selectedCell.chest.OpenChest();
                        _isWaitingOnInteractableTileSelect = false;

                        //flee
                        _turn.Flee();
                    }
                } else {
                    _turn.EndTurn();
                }
            }
        }
    }

    private void OnDestroy() {
        DieRoller.RollDie -= OnDieRolled;
        WeaopnPowerUpDisplayer.ReplaceInputRecieved -= OnWeaponReplaceButtonPressed;
    }
}
