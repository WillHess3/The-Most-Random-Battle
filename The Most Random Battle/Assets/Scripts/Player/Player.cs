using UnityEngine;
using System.Collections.Generic;

public abstract class Player : MonoBehaviour {

    protected Vector2Int _coord;

    public GridInformation gridInformation;

    protected Turn _turn;

    public AnimationCurve _movementInterpolationCurve;
    protected bool _isInMotion;
    protected Vector3 _moveFromPosition;
    protected Vector3 _targetPosititon;
    protected float _moveTimer = 0.5f;

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

        _isInMotion = true;
        _moveFromPosition = transform.position;
        _targetPosititon = gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0);
    }

    protected void InMotion() {
        if (_moveTimer > 0) {
            _moveTimer -= Time.deltaTime;
            transform.position = Vector3.Lerp(_moveFromPosition, _targetPosititon, _movementInterpolationCurve.Evaluate(1 - _moveTimer / 0.5f));
        } else {
            _isInMotion = false;
            _moveTimer = 0.5f;
            transform.position = _targetPosititon;
            _turn.EndTurn();
        }
    }

    public abstract void ChooseDirection();
    public abstract void StartMovingProcess();
}
