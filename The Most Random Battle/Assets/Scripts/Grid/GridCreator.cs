using UnityEngine;

public class GridCreator : MonoBehaviour {

    public static GridCreator instance;

    private Grid _grid;

    public Grid Grid => _grid;

    [Header("Grid")]
    [SerializeField] private int _gridLength;
    [SerializeField] private int _gridHeight;

    public GridInformation gridInformation;

    [Header("Cell")]
    [SerializeField] private bool _isBorders;

    [SerializeField] private Sprite _cellSpriteWithoutBorder;
    [SerializeField] private Sprite _cellSpriteWithBorder;

    [SerializeField] private Color _color1;
    [SerializeField] private Color _color2;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        _grid = new Grid(_gridLength, _gridHeight, _isBorders ? _cellSpriteWithBorder : _cellSpriteWithoutBorder, _color1, _color2, transform);

        _grid.CreateGrid();

        gridInformation.SetGrid(_gridLength, _gridHeight);
    }

}
