using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager {

    private readonly WeaponsList _weaponList;
    private Weapons[] _inventory;
    private Weapons _equipedWeapon;

    public Weapons EquipedWeapon => _equipedWeapon;

    private Player _player;

    private float _headHitChance;
    private float _chestHitChance;
    private float _armsHitChance;
    private float _legsHitChance;

    public PlayerWeaponManager(Player player, WeaponsList weaponsList) {
        _weaponList = weaponsList;
        _player = player;

        _inventory = new Weapons[2];

        _equipedWeapon = weaponsList.Weapons[0];
        _inventory[0] = _equipedWeapon;
    }

    public List<Vector2Int> AttackableCellCoords() {
        List<Vector2Int> attackableCellCoords = new List<Vector2Int>();

        for (int y = (int)(_player.Coord.y - _equipedWeapon.attackRadius); y < (int)(_player.Coord.y + _equipedWeapon.attackRadius); y++) {
            for (int x = (int)(_player.Coord.x - _equipedWeapon.attackRadius); x < (int)(_player.Coord.x + _equipedWeapon.attackRadius); x++) {
                if (x * x + y * y <= _equipedWeapon.attackRadius * _equipedWeapon.attackRadius) {
                    if (x == _player.Coord.x && y == _player.Coord.y) {
                        continue;
                    }

                    attackableCellCoords.Add(new Vector2Int(x, y));
                }
            }
        }

        return attackableCellCoords;
    }

    public void GenerateHitChances(Vector2Int targetCoord) {
        float headMeanHitChance = _equipedWeapon.headMeanHitChance;
        float limbMeanHitChance = _equipedWeapon.limbMeanHitChance;
        float chestMeanHitChance = _equipedWeapon.chestMeanHitChance;

        if (!_equipedWeapon.isMelee) {
            float distance = (targetCoord - _player.Coord).magnitude;

            headMeanHitChance = Mathf.Lerp(_equipedWeapon.headMeanHitChance, _equipedWeapon.headMeanHitChanceMaxDistance, distance / _equipedWeapon.attackRadius);
            limbMeanHitChance = Mathf.Lerp(_equipedWeapon.limbMeanHitChance, _equipedWeapon.limbMeanHitChanceMaxDistance, distance / _equipedWeapon.attackRadius);
            chestMeanHitChance = Mathf.Lerp(_equipedWeapon.chestMeanHitChance, _equipedWeapon.chestMeanHitChanceMaxDistance, distance / _equipedWeapon.attackRadius);
        }

        _headHitChance = Mathf.Clamp01(NormalDistribution.RandomOverNormalDistribution(headMeanHitChance, _equipedWeapon.headHitChanceStandardDeviation));
        _chestHitChance = Mathf.Clamp01(NormalDistribution.RandomOverNormalDistribution(chestMeanHitChance, _equipedWeapon.chestHitChanceStandardDeviation));
        _armsHitChance = Mathf.Clamp01(NormalDistribution.RandomOverNormalDistribution(limbMeanHitChance, _equipedWeapon.limbHitChanceStandardDeviation));
        _legsHitChance = Mathf.Clamp01(NormalDistribution.RandomOverNormalDistribution(limbMeanHitChance, _equipedWeapon.limbHitChanceStandardDeviation));
    }

    //Attack
    public void Attack(Player target, Limbs targetedLimb) {
        float randomHitChance = Random.value;
        bool isHit = false;

        switch (targetedLimb) {
            case Limbs.Chest:
                if (randomHitChance >= _chestHitChance) {
                    target.TakeDamage(_equipedWeapon.damage);
                    isHit = true;
                }

                break;

            case Limbs.Head:
                if (randomHitChance >= _headHitChance) {
                    target.TakeDamage(5);
                    isHit = true;
                }

                break;

            case Limbs.Arms:
                if (randomHitChance >= _armsHitChance) {
                    target.CrippleLimb(true);

                    if (Random.value < .5f) {
                        target.TakeDamage(_equipedWeapon.damage);
                    }

                    isHit = true;
                }

                break;

            case Limbs.Legs:
                if (randomHitChance >= _legsHitChance) {
                    target.CrippleLimb(false);

                    if (Random.value < .5f) {
                        target.TakeDamage(_equipedWeapon.damage);
                    }

                    isHit = true;
                }

                break;
        }

        if (!_equipedWeapon.isMelee || (_equipedWeapon.isMelee && isHit)) {
            _equipedWeapon.durability--;
        }
    }
}

public enum Limbs {
    Chest,
    Head,
    Legs,
    Arms
}
