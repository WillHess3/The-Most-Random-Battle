using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    private Vector2Int _coord;
    public Vector2Int Coord => _coord;

    private void Start() {
        PickCoord();
    }

    private void PickCoord() {
        Vector2Int randCoord = new Vector2Int(Mathf.FloorToInt(Random.value * GridCreator.instance.gridInformation.GridLength), Mathf.FloorToInt(Random.value * GridCreator.instance.gridInformation.GridHeight));

        if (GridCreator.instance.Grid.GetCellAtCoord(randCoord).CellState == CellState.Blocked) {
            PickCoord();
        } else {
            _coord = randCoord;
            GridCreator.instance.Grid.GetCellAtCoord(randCoord).SetCellState(CellState.Blocked);
            transform.position = GridCreator.instance.gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0);
        }
    }

    public void OpenChest() {
        Debug.Log("chest opened");
    }

    public void SetCoord(Vector2Int coord) {
        _coord = coord;
    }
}
