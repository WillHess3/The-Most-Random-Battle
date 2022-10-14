using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class WellnessManager : MonoBehaviour {

    [Header("Sprites")]
    [SerializeField] private Sprite _arm;
    [SerializeField] private Sprite _leg;
    [SerializeField] private Sprite _brokenArm;
    [SerializeField] private Sprite _brokenLeg;

    [Header("UI Elements")]
    [SerializeField] private Image _heart1;
    [SerializeField] private Image _heart2;
    [SerializeField] private Image _heart3;
    [SerializeField] private Image _armElement;
    [SerializeField] private Image _legElement;

    [Header("Misc")]
    [SerializeField] private GameManager _gameManager;
    private Player _player;

    private void Update() {
        _player = _gameManager.CurrentPlayer;

        if (_player.IsControllablePlayer) {
            _heart3.gameObject.SetActive(_player.Health >= 3);
            _heart2.gameObject.SetActive(_player.Health >= 2);
            _heart1.gameObject.SetActive(_player.Health >= 1);

            _armElement.sprite = _player.IsArmsCrippled ? _brokenArm : _arm;
            _legElement.sprite = _player.IsLegsCrippled ? _brokenLeg : _leg;
        }
    }
}
