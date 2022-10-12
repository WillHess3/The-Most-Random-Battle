using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager {

    private Weapon[] _inventory;
    private Weapon _equipedWeapon;

    public Weapon EquipedWeapon => _equipedWeapon;
    public Weapon[] Inventory => _inventory;

    private Player _player;

    public PlayerWeaponManager(Player player) {
        _player = player;

        _inventory = new Weapon[2];

        _equipedWeapon = _player.StartingKnifeWeapon;
        _inventory[0] = _equipedWeapon;
    }

    public void Equip(int weaponIndex) {
        _equipedWeapon = _inventory[weaponIndex];
    }

    public void PickUpWeapon(Weapon weapon, int inventorySpot) {
        _inventory[inventorySpot] = weapon;
    }

    public void DropWeapon(int inventorySpot) {
        _inventory[inventorySpot] = null;
    }

    public void Attack(Player target, Limbs targetedLimb) {
        float randomHitChance = Random.value;
        bool isHit = false;

        switch (targetedLimb) {
            case Limbs.Chest:
                if (randomHitChance <= _equipedWeapon.WeaponHitChances.chestHitChance) {
                    target.TakeDamage(_equipedWeapon.WeaponScriptableObject.damage);
                    isHit = true;
                }

                break;

            case Limbs.Head:
                if (randomHitChance <= _equipedWeapon.WeaponHitChances.headHitChance) {
                    target.TakeDamage(3);
                    isHit = true;
                }

                break;

            case Limbs.Arms:
                if (randomHitChance <= _equipedWeapon.WeaponHitChances.armsHitChance) {
                    target.CrippleLimb(true);

                    if (Random.value < .5f) {
                        target.TakeDamage(_equipedWeapon.WeaponScriptableObject.damage);
                    }

                    isHit = true;
                }

                break;

            case Limbs.Legs:
                if (randomHitChance <= _equipedWeapon.WeaponHitChances.legsHitChance) {
                    target.CrippleLimb(false);

                    if (Random.value < .5f) {
                        target.TakeDamage(_equipedWeapon.WeaponScriptableObject.damage);
                    }

                    isHit = true;
                }

                break;
        }

        if (!_equipedWeapon.WeaponScriptableObject.isMelee || (_equipedWeapon.WeaponScriptableObject.isMelee && isHit)) {
            _equipedWeapon.TakeWeaponDamage();
        }
    }
}

public enum Limbs {
    Chest,
    Head,
    Legs,
    Arms
}
