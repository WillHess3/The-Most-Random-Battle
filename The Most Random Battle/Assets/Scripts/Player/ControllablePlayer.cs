using System.Collections.Generic;
using UnityEngine;
using System;

public class ControllablePlayer : Player {

    private bool _isGettingDirInput;

    private bool _isWaitingOnDiceRoll;

    private bool _isWaitingOnInteractableTileSelect;

    public static event Action<Player> ReplaceWeaponStart;
    private bool _acceptingReplacementInput;

    private List<Cell> _highlightedCells;

    public static event Action<Vector4> ToggleArrows;

    private void Start() {
        _isControllablePlayer = true;

        Spawn();

        _turn = new Turn(this);

        _interactableCells = new List<Cell>();

        DieRoller.RollDie += OnDieRolled;

        WeaopnPowerUpDisplayer.ReplaceInputRecieved += OnWeaponReplaceButtonPressed;

        _highlightedCells = new List<Cell>();
    }

    public override void ChooseDirection() {
        _isGettingDirInput = true;
        ToggleArrows?.Invoke(new Vector4(1, 1, 1, 1));
    }

    private void SetDirection(Vector2Int direction) {
        Vector4 arrowDir;

        if (direction == Vector2Int.up) {
            arrowDir = new Vector4(1, 0, 0, 0);
        } else if (direction == Vector2Int.right) {
            arrowDir = new Vector4(0, 1, 0, 0);
        } else if (direction == Vector2Int.down) {
            arrowDir = new Vector4(0, 0, 1, 0);
        } else {
            arrowDir = new Vector4(0, 0, 0, 1);
        }

        ToggleArrows?.Invoke(arrowDir);
        _direction = direction;
        _isGettingDirInput = false;
        _turn.Move();
    }

    public override void StartMovingProcess() {
        _isWaitingOnDiceRoll = true;
    }

    public void OnDieRolled(int rolledNumber) {
        if (_isWaitingOnDiceRoll) {
            ToggleArrows?.Invoke(Vector4.zero);
            _isWaitingOnDiceRoll = false;
            Move(rolledNumber, _direction);
        }
    }

    public override void Interact() {
        //check if interacting is possible
        int attackRadius = (int) _playerWeaponManager.EquipedWeapon.WeaponScriptableObject.attackRadius;
        if (IsInteractingPossible(attackRadius)) {
            //Get cell to interact with
            _isWaitingOnInteractableTileSelect = true;

            //highlight cells
            _highlightedCells = HighlightCellsInAttackRadius(attackRadius);
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

    private List<Cell> HighlightCellsInAttackRadius(int radius) {
        List<Cell> highlightedCells = new List<Cell>();

        for (int y = _coord.y - radius; y <= _coord.y + radius; y++) {
            for (int x = _coord.x - radius; x <= _coord.x + radius; x++) {
                if (x < 0 || x > GridCreator.instance.gridInformation.GridLength - 1 || y < 0 || y > GridCreator.instance.gridInformation.GridHeight - 1) {
                    continue;
                }

                Cell cell = GridCreator.instance.Grid.GetCellAtCoord(new Vector2Int(x, y));

                if (cell.player == this) {
                    continue;
                }

                if ((cell.CellCoord - _coord).sqrMagnitude <= radius * radius) {
                    highlightedCells.Add(cell);
                    cell.HighlightCell(true);
                }
            }
        }

        return highlightedCells;
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
                    foreach (Cell cell in _highlightedCells) {
                        cell.HighlightCell(false);
                    }

                    _highlightedCells.Clear();

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
                    foreach (Cell cell in _highlightedCells) {
                        cell.HighlightCell(false);
                    }

                    _highlightedCells.Clear();

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
