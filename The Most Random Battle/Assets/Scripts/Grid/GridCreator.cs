using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour {

    private Grid _grid;

    [Header("Grid")]
    [SerializeField] private int _gridLength;
    [SerializeField] private int _gridHeight;

    public GridInformation gridInformation;

    [Header("Cell")]
    [SerializeField] private Sprite _cellSprite;

    [SerializeField] private Color _color1;
    [SerializeField] private Color _color2;

    private void Awake() {
        _grid = new Grid(_gridLength, _gridHeight, _cellSprite, _color1, _color2, transform);

        _grid.CreateGrid();

        gridInformation.SetGrid(_gridLength, _gridHeight);
    }

}
