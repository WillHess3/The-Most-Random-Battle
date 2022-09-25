using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour {

    [SerializeField] private List<GameObject> _arrows;

    private void Start() {
        ControllablePlayer.ToggleArrows += DisplayArrows;
    }

    private void DisplayArrows(Vector4 arrowInfo) {
        _arrows[0].SetActive(arrowInfo.x > 0);
        _arrows[1].SetActive(arrowInfo.y > 0);
        _arrows[2].SetActive(arrowInfo.z > 0);
        _arrows[3].SetActive(arrowInfo.w > 0);
    }

    private void OnDestroy() {
        ControllablePlayer.ToggleArrows -= DisplayArrows;
    }

}
