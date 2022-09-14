using UnityEngine;

public class Cell {

    private Vector2Int _coord;
    public Vector2Int CellCoord => _coord;

    private const float CELL_SIZE = 2;

    private readonly GameObject _cellGameObject;

    private CellState _cellState;
    public CellState CellState => _cellState;

    public Pickupable pickupableObject;
    public Player player;
    public Chest chest;

    public Cell(Vector2Int coord, Grid grid) {
        _cellState = CellState.Empty;

        _coord = coord;

        _cellGameObject = new GameObject("Cell (" + _coord.x + ", " + _coord.y + ")");
        _cellGameObject.transform.parent = grid.Parent;
        _cellGameObject.transform.position = new Vector3(coord.x * CELL_SIZE, coord.y * CELL_SIZE, 0);

        SpriteRenderer spriteRenderer = _cellGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = grid.CellSprite;
        spriteRenderer.sortingLayerName = "Grid";

        if (_coord.y % 2 == 0) {
            spriteRenderer.color = _coord.x % 2 == 0 ? grid.Color1 : grid.Color2;
        } else {
            spriteRenderer.color = _coord.x % 2 == 1 ? grid.Color1 : grid.Color2;
        }

    }

    public void SetCellState(CellState cellState) {
        _cellState = cellState;
    }

}

public enum CellState {
    Empty,
    Occupied,
    Blocked
}
