using UnityEngine;
using Cinemachine;

public class CameraTargeting : MonoBehaviour {

    private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private void Awake() {
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        GameManager.SwitchCameraTarget += OnTurnSwitched;
    }

    private void OnTurnSwitched(Player player) {
        _cinemachineVirtualCamera.Follow = player.transform;
    }

    private void OnDestroy() {
        GameManager.SwitchCameraTarget -= OnTurnSwitched;
    }

}
