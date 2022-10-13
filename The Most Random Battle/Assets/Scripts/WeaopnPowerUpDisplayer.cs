using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaopnPowerUpDisplayer : MonoBehaviour {

    [Header("UI References")]
    [SerializeField] private Image _weaponSquare1;
    [SerializeField] private Image _weaponSquare2;
    [SerializeField] private Image _powerUpSquare;

    public Image WeaponSquare1 => _weaponSquare1;
    public Image WeaponSquare2 => _weaponSquare2;

    [SerializeField] private Image _displayedWeapon1;
    [SerializeField] private Image _displayedWeapon2;
    [SerializeField] private Image _displayedPowerUp;

    [Header("Sprites")]
    [SerializeField] private Sprite _squareEmpty;
    [SerializeField] private Sprite _weaponSquareSilhouette;
    [SerializeField] private Sprite _powerUpSquareSilhouette;

    private Player _player;
    public Player Player => _player;

    public static event Action<Player, int> ReplaceInputRecieved;

    private bool _isReplaceMode;
    public bool IsReplaceMode => _isReplaceMode;

    private PlayerWeaponEquip _playerWeaponEquip;

    private void Awake() {
        GameManager.NextTurnEvent += OnDisplayInventoryItems;
        Player.PickedUpWeaponVisualEvent += OnDisplayInventoryItems;
        ControllablePlayer.ReplaceWeaponStart += OnStartReplaceWeapon;

        _playerWeaponEquip = GetComponent<PlayerWeaponEquip>();
    }

    private void OnDisplayInventoryItems(Player currentPlayer) {
        if (currentPlayer.IsControllablePlayer) {
            _player = currentPlayer;
            DisplayInventoryItems(currentPlayer.PlayerWeaponManager.Inventory[0], currentPlayer.PlayerWeaponManager.Inventory[1], null);
            _playerWeaponEquip.HighlightEquipedWeapon(currentPlayer.PlayerWeaponManager.Inventory[0] == currentPlayer.PlayerWeaponManager.EquipedWeapon ? 0 : 1);
        } else {
            PlayerWeaponEquip.ChangePlayerSprite(currentPlayer);
        }
    }

    private void DisplayInventoryItems(Weapon weapon1, Weapon weapon2, PowerUp powerUp) {
        if (weapon1 != null) {
            _weaponSquare1.sprite = _squareEmpty;
            _displayedWeapon1.sprite = weapon1.WeaponScriptableObject.weaponSprite;
            _displayedWeapon1.color = new Color(1, 1, 1, 1);

            _displayedWeapon1.SetNativeSize();
        } else {
            _weaponSquare1.sprite = _weaponSquareSilhouette;
            _displayedWeapon1.color = new Color(1, 1, 1, 0);
        }

        if (weapon2 != null) {
            _weaponSquare2.sprite = _squareEmpty;
            _displayedWeapon2.sprite = weapon2.WeaponScriptableObject.weaponSprite;
            _displayedWeapon2.color = new Color(1, 1, 1, 1);

            _displayedWeapon2.SetNativeSize();
        } else {
            _weaponSquare2.sprite = _weaponSquareSilhouette;
            _displayedWeapon2.color = new Color(1, 1, 1, 0);
        }

        //power up
    }

    private void OnStartReplaceWeapon(Player player) {
        _player = player;

        //turn both bgs red
        _weaponSquare1.color = Color.red;
        _weaponSquare2.color = Color.red;

        _isReplaceMode = true;
    }

    public void OnReplaceWeaponButton(int index) {
        if (_isReplaceMode) {

            _weaponSquare1.color = Color.white;
            _weaponSquare2.color = Color.white;

            ReplaceInputRecieved?.Invoke(_player, index);
            _isReplaceMode = false;
        }
    }

    private void OnDestroy() {
        GameManager.NextTurnEvent -= OnDisplayInventoryItems;
        Player.PickedUpWeaponVisualEvent -= OnDisplayInventoryItems;
        ControllablePlayer.ReplaceWeaponStart -= OnStartReplaceWeapon;
    }
}
