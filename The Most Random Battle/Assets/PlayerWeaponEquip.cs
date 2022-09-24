using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponEquip : MonoBehaviour {

    private WeaopnPowerUpDisplayer _weaopnPowerUpDisplayer;
    
    [SerializeField] private GameManager _gameManager;

    private void Start() {
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
        if (_gameManager.CurrentPlayer.PlayerWeaponManager.EquipedWeapon != _gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[weaponIndex]) {
            _gameManager.CurrentPlayer.PlayerWeaponManager.Equip(weaponIndex);
        }

        //highlight green
        if (weaponIndex == 0) {
            _weaopnPowerUpDisplayer.WeaponSquare1.color = Color.green;
            _weaopnPowerUpDisplayer.WeaponSquare2.color = new Color(1, 1, 1, 1);
        } else {
            _weaopnPowerUpDisplayer.WeaponSquare1.color = new Color(1, 1, 1, 1);
            _weaopnPowerUpDisplayer.WeaponSquare2.color = Color.green;
        }

        //change player sprite
        _gameManager.CurrentPlayer.GetComponent<SpriteRenderer>().sprite = _gameManager.CurrentPlayer.PlayerWeaponManager.EquipedWeapon.WeaponScriptableObject.playerSprite;

    }
}
