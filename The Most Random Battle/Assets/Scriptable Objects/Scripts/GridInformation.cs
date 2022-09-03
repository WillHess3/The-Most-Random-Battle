using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Grid Information")]
public class GridInformation : ScriptableObject {

    public int GridLength { get; private set; }
    public int GridHeight { get; private set; }

    [HideInInspector] public readonly int cellSize = 2;

    public void SetGrid(int length, int height) {
        GridLength = length;
        GridHeight = height;
    }

}
