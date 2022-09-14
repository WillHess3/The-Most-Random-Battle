using UnityEngine;
using System.Collections.Generic;

public abstract class Player : MonoBehaviour {

    public GameManager gameManager;
    
    protected Vector2Int _coord;
    public Vector2Int Coord => _coord;

    public GridInformation gridInformation;

    protected Turn _turn;
    public Turn Turn => _turn;
    
    protected bool _isControllablePlayer;
    public bool IsControllablePlayer => _isControllablePlayer;

    protected Vector2Int _direction;

    public AnimationCurve movementInterpolationCurve;
    protected bool _isInMotion;
    protected Vector3 _moveFromPosition;
    protected Vector3 _targetPosititon;
    protected float _moveTimer = 0.5f;

    protected List<Cell> _interactableCells;

    [SerializeField] private PlayerColors _playerColors;
    protected int _colorIndex;

    [SerializeField] protected WeaponsList _weaponsList;
    protected PlayerWeaponManager _playerWeaponManager;

    protected int _health;
    protected bool _isArmsCrippled;
    protected bool _isLegsCrippled;

    protected SpriteRenderer _spriteRenderer;

    protected void Spawn() {
        _playerWeaponManager = new PlayerWeaponManager(this, _weaponsList);
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.sprite = _playerWeaponManager.EquipedWeapon.playerSprite;

        void PickColor() {
            int randIndex = Mathf.FloorToInt(Random.value * _playerColors.Colors.Count);

            if (gameManager.UnavailableColorIndecies.Contains(randIndex)) {
                PickColor();
            } else {
                _colorIndex = randIndex;
                gameManager.SetUnavailableColor(_colorIndex);
            }
        }

        PickColor();
        GetComponent<SpriteRenderer>().material.color = _playerColors.Colors[_colorIndex];

        void PickCoords() {
            int xCoord = Mathf.FloorToInt(Random.value * gridInformation.GridLength);
            int yCoord = Mathf.FloorToInt(Random.value * gridInformation.GridHeight);
            _coord = new Vector2Int(xCoord, yCoord);

            if (GridCreator.instance.Grid.GetCellAtCoord(_coord).CellState != CellState.Empty) {
                PickCoords();
            }
        }

        PickCoords();

        transform.position = gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0);

        GridCreator.instance.Grid.GetCellAtCoord(_coord).SetCellState(CellState.Blocked);
        GridCreator.instance.Grid.GetCellAtCoord(_coord).player = this;

        gameManager.PlayerReady();
    }
    
    public void Move(int moveAmount, Vector2Int direction) {
        //makes cell path
        List<Vector2Int> coordinatePath = new List<Vector2Int>();

        for (int i = 1; i <= moveAmount; i++) {
            Vector2Int newCoordInCoordPath = _coord + i * direction;
            coordinatePath.Add(newCoordInCoordPath);
        }

        //adjusts path if a cell is blocked and or on the grid
        int startRemovingIndex = coordinatePath.Count;
        bool isRemoveFollowingTilesFromPath = false;
        foreach (Vector2Int coord in coordinatePath) {
            if (coord.x < 0 || coord.x >= gridInformation.GridLength || coord.y < 0 || coord.y >= gridInformation.GridHeight) {
                isRemoveFollowingTilesFromPath = true;
            } else {
                if (GridCreator.instance.Grid.GetCellAtCoord(coord).CellState == CellState.Blocked) {
                    isRemoveFollowingTilesFromPath = true;
                }
            }

            if (isRemoveFollowingTilesFromPath) {
                startRemovingIndex = coordinatePath.IndexOf(coord);
                break;
            }
        }

        if (startRemovingIndex < coordinatePath.Count) {
            for (int i = coordinatePath.Count - 1; i >= startRemovingIndex; i--) {
                coordinatePath.RemoveAt(i);
            }
        }

        //checks path for a pickupable objcet
        foreach (Vector2Int coord in coordinatePath) {
            Cell cell = GridCreator.instance.Grid.GetCellAtCoord(coord);
            if (cell.CellState == CellState.Occupied) {
                Debug.Log(cell.pickupableObject);
            }
        }

        //sets new coord
        GridCreator.instance.Grid.GetCellAtCoord(_coord).SetCellState(CellState.Empty);
        GridCreator.instance.Grid.GetCellAtCoord(_coord).player = null;

        if (coordinatePath.Count > 0) {
            _coord = coordinatePath[coordinatePath.Count - 1];
        }

        GridCreator.instance.Grid.GetCellAtCoord(_coord).SetCellState(CellState.Blocked);
        GridCreator.instance.Grid.GetCellAtCoord(_coord).player = this;

        //moves player
        _isInMotion = true;
        _moveFromPosition = transform.position;
        _targetPosititon = gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0);
    }

    protected void InMotion() {
        if (_moveTimer > 0) {
            _moveTimer -= Time.deltaTime;
            transform.position = Vector3.Lerp(_moveFromPosition, _targetPosititon, movementInterpolationCurve.Evaluate(1 - _moveTimer / 0.5f));
        } else {
            _isInMotion = false;
            _moveTimer = 0.5f;
            transform.position = _targetPosititon;
            _turn.MovingComplete();
        }
    }

    protected bool IsInteractingPossible(int radius) {
        _interactableCells.Clear();

        /*foreach (Player player in gameManager.Players) {
            if (player == this) {
                continue;
            }

            if (Mathf.Pow(player.Coord.x - _coord.x, 2) + Mathf.Pow(player.Coord.y - _coord.y, 2) <= range * range) {
                _interactableCells.Add(GridCreator.instance.Grid.GetCellAtCoord(player.Coord));
            }
        }

        foreach (Chest chest in gameManager.Chests) {
            if ((chest.Coord - _coord).sqrMagnitude == 1) {
                _interactableCells.Add(GridCreator.instance.Grid.GetCellAtCoord(chest.Coord));
            }
        }*/

        for (int y = _coord.y - radius; y <= _coord.y + radius; y++) {
            for (int x = _coord.x - radius; x <= _coord.x + radius; x++) {
                if (x < 0 || x > GridCreator.instance.gridInformation.GridLength - 1 || y < 0 || y > GridCreator.instance.gridInformation.GridHeight - 1) {
                    continue;
                }

                Cell cell = GridCreator.instance.Grid.GetCellAtCoord(new Vector2Int(x, y));

                if (cell.player == this) {
                    continue;
                }

                if ((cell.CellCoord - _coord).sqrMagnitude <= radius * radius && cell.CellState == CellState.Blocked) {
                    Debug.Log("cum");
                    //something on the cell
                    if (cell.chest != null) {
                        if ((cell.CellCoord - _coord).sqrMagnitude == 1) {
                            _interactableCells.Add(cell);
                        }
                    } else if (cell.player != null) {
                        _interactableCells.Add(cell);
                    }
                }
            }
        }

        return _interactableCells.Count > 0;
    }

    public abstract void ChooseDirection();
    public abstract void StartMovingProcess();
    public abstract void Interact();
    public abstract void Flee();

    public void TakeDamage(int damage) {
        _health -= damage;
    }

    public void CrippleLimb(bool isCripplingArms) {
        if (isCripplingArms) {
            _isArmsCrippled = true;
        } else {
            _isLegsCrippled = true;
        }
    }
}
