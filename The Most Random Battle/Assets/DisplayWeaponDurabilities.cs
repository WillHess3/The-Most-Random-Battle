using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayWeaponDurabilities : MonoBehaviour {

    [Header("TMP objects")]
    [SerializeField] private TMP_Text _durabilityWeapon1;
    [SerializeField] private TMP_Text _durabilityWeapon2;

    [Header("Misc")]
    [SerializeField] private GameManager _gameManager;

    private void ShowDurability(int weaponIndex) {
        //show the counter & display proper count in the counter
        if (weaponIndex == 0) {
            _durabilityWeapon1.text = _gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[0].Durability.ToString();
        } else {
            _durabilityWeapon2.text = _gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[1].Durability.ToString();
        }

        //if knife, hide counter
        if (_gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[0]?.WeaponScriptableObject.name == "Knife") {
            _durabilityWeapon1.text = "";
        }

        if (_gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[1]?.WeaponScriptableObject.name == "Knife") {
            _durabilityWeapon2.text = "";
        }
    }

    private void Update() {
        if (_gameManager.CurrentPlayer.IsControllablePlayer) {
            if (_gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[0] != null) {
                ShowDurability(0);
            } else {
                _durabilityWeapon1.text = "";
            }

            if (_gameManager.CurrentPlayer.PlayerWeaponManager.Inventory[1] != null) {
                ShowDurability(1);
            } else {
                _durabilityWeapon2.text = "";
            }
        }
    }

}
