using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllablePlayer : Player {

    private void Start() {
        int xCoord = Mathf.FloorToInt(Random.value * gridInformation.GridLength);
        int yCoord = Mathf.FloorToInt(Random.value * gridInformation.GridHeight);
        _coord = new Vector2Int(xCoord, yCoord);

        Move(0, Vector2Int.zero);
        Debug.Log(_coord);
    }

}
