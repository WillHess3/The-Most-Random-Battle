using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponEquip : MonoBehaviour {

    private WeaopnPowerUpDisplayer _weaopnPowerUpDisplayer;
    
    [SerializeField] private GameManager _gameManager;

    private void Awake() {
        _weaopnPowerUpDisplayer = GetComponent<WeaopnPowerUpDisplayer>();
    }


    public void OnButtonPressed(int weaponIndex) {
        HighlightEquipedWeapon(weaponIndex);
    }

    public void HighlightEquipedWeapon(int weaponIndex) {
        //check if player is controllable
        if (!_gameManager.CurrentPlayer.IsControllablePlayer) {
            return;
        }

        //check if theres something there
        if (_gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[weaponIndex] == null) {
            return;
        }

        //change inventory equip if necessary
        ChangeInventoryEquipedWeapon(weaponIndex);

        //highlight green
        if (weaponIndex == 0) {
            _weaopnPowerUpDisplayer.WeaponSquare1.color = Color.green;
            _weaopnPowerUpDisplayer.WeaponSquare2.color = new Color(1, 1, 1, 1);
        } else {
            _weaopnPowerUpDisplayer.WeaponSquare1.color = new Color(1, 1, 1, 1);
            _weaopnPowerUpDisplayer.WeaponSquare2.color = Color.green;
        }

        //change player sprite
        ChangePlayerSprite(_gameManager.CurrentPlayer);
    }

    public void ChangeInventoryEquipedWeapon(int weaponIndex) {
        if (_gameManager.CurrentPlayer.PlayerWeaponManager.EquipedWeapon != _gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[weaponIndex]) {
            _gameManager.CurrentPlayer.PlayerWeaponManager.Equip(weaponIndex);
        }
    }

    public static void ChangePlayerSprite(Player player) {
        player.GetComponent<SpriteRenderer>().sprite = player.PlayerWeaponManager.EquipedWeapon.WeaponScriptableObject.playerSprite;
    }
}
