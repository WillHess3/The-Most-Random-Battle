using UnityEngine;

public abstract class Player : MonoBehaviour {

    protected Vector2Int _coord;

    public GridInformation gridInformation;

    protected void Move(int moveAmount, Vector2Int direction) {
        _coord += moveAmount * direction;

        if (_coord.x < 0) {
            _coord.x = 0;
        } else if (_coord.x > gridInformation.GridLength) {
            _coord.x = gridInformation.GridLength;
        }

        if (_coord.y < 0) {
            _coord.y = 0;
        } else if (_coord.y > gridInformation.GridHeight) {
            _coord.y = gridInformation.GridHeight;
        }

        transform.position = gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0);
    }

}
