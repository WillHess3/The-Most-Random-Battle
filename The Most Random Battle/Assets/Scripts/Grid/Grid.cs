using UnityEngine;

public class Grid {

    private readonly int _gridLength;
    private readonly int _gridHeight;

    private Cell[,] _cells;

    private readonly Sprite _cellSprite;
    private readonly Color _color1;
    private readonly Color _color2;
    
    public Sprite CellSprite => _cellSprite;
    public Color Color1 => _color1;
    public Color Color2 => _color2;

    private readonly Transform _parent;
    public Transform Parent => _parent;

    public Grid(int gridLength, int gridHeight, Sprite cellSprite, Color color1, Color color2, Transform parent) {
        _gridLength = gridLength;
        _gridHeight = gridHeight;

        _cellSprite = cellSprite;
        _color1 = color1;
        _color2 = color2;

        _parent = parent;
    }

    public void CreateGrid() {
        _cells = new Cell[_gridLength, _gridHeight];

        for (int i = 0; i < _gridLength; i++) {
            for (int j = 0; j < _gridHeight; j++) {
                _cells[i, j] = new Cell(new Vector2Int(i, j), this);
            }
        }
    }

    public Cell GetCellAtCoord(Vector2Int coord) => _cells[coord.x, coord.y];

    public Vector2Int GetCellCoordFromClick(Vector3 clickPosition) {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(clickPosition);

        Vector2Int coord = new Vector2Int(Mathf.FloorToInt((worldPoint.x + 1) / 2f), Mathf.FloorToInt((worldPoint.y + 1) / 2f));
        return coord;
    }

}