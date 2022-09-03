using UnityEngine;

public abstract class Player : MonoBehaviour {

    protected Vector2Int _coord;

    public GridInformation gridInformation;

    protected Turn _turn;

    protected void Spawn() {
        int xCoord = Mathf.FloorToInt(Random.value * gridInformation.GridLength);
        int yCoord = Mathf.FloorToInt(Random.value * gridInformation.GridHeight);
        _coord = new Vector2Int(xCoord, yCoord);

        transform.position = gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0);
    }
    
    public void Move(int moveAmount, Vector2Int direction) {
        _coord += moveAmount * direction;

        if (_coord.x < 0) {
            _coord.x = 0;
        } else if (_coord.x >= gridInformation.GridLength) {
            _coord.x = gridInformation.GridLength - 1;
        }

        if (_coord.y < 0) {
            _coord.y = 0;
        } else if (_coord.y >= gridInformation.GridHeight) {
            _coord.y = gridInformation.GridHeight - 1;
        }

        transform.position = gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0);
    }

    public abstract void ChooseDirection();
    public abstract void StartMovingProcess();
}
