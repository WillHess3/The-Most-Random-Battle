using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShowerHider : MonoBehaviour {

    [SerializeField] private GameObject _toggleableUIElements;

    private void Start() {
        GameManager.StartAndEndControllablePlayerTurn += OnToggleUI;

        OnToggleUI();
    }

    private void OnToggleUI() {
        if (_toggleableUIElements.activeSelf) {
            _toggleableUIElements.SetActive(false);
        } else {
            _toggleableUIElements.SetActive(true);
        }
    }

    private void OnDestroy() {
        GameManager.StartAndEndControllablePlayerTurn -= OnToggleUI;
    }

}
