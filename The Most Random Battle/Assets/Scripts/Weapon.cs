using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    private Pickupable _pickupable;

    private Player _player;

    private Weapons _weaponScriptableObject;
    public Weapons WeaponScriptableObject => _weaponScriptableObject;

    private int _durability;

    public struct HitChances {
        public float headHitChance;
        public float chestHitChance;
        public float armsHitChance;
        public float legsHitChance;
    }

    private HitChances _hitChances;
    public HitChances WeaponHitChances => _hitChances;

    private void Awake() {
        _pickupable = GetComponent<Pickupable>();

        _weaponScriptableObject = _pickupable.weaponsList.Weapons[_pickupable.Sprites.IndexOf(GetComponent<SpriteRenderer>().sprite)];

        _durability = _weaponScriptableObject.durability;

        Player.PickedUpWeapon += OnWeaponPickedUp;
    }

    public int WeaponScore() {
        int score = 0;

        if (_pickupable.PickupableType == PickupableTypeEnum.Knife) {
            score += 1;
        } else if (_pickupable.PickupableType == PickupableTypeEnum.Spear) {
            score += 5;
        } else if (_pickupable.PickupableType == PickupableTypeEnum.Chainsaw || _pickupable.PickupableType == PickupableTypeEnum.Machette || _pickupable.PickupableType == PickupableTypeEnum.Hammer) {
            score += 10;
        } else if (_pickupable.PickupableType == PickupableTypeEnum.Pistol || _pickupable.PickupableType == PickupableTypeEnum.Shotgun || _pickupable.PickupableType == PickupableTypeEnum.SMG) {
            score += 15;
        } else {
            score += 20;
        }

        score -= (int)((1 - (_durability / (float)_weaponScriptableObject.durability)) * 15);

        return score;
    }

    private void OnWeaponPickedUp(Player player, GameObject weapon, int inventoryIndex) {
        //check if its this weapon
        if (weapon != gameObject) {
            return;
        }

        //get a reference to the player
        _player = player;

        //set place in inventory
        _player.PlayerWeaponManager.PickUpWeapon(this, inventoryIndex);
        
        //change visual
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Drop(Vector2Int coord) {
        void PickDropCoord(int depth) {
            if (depth <= 0) {
                return; 
            }

            float rand = Random.value;

            int xOffset = 0;
            int yOffset = 0;
            if (rand < .5f) {
                xOffset = (int)((Mathf.RoundToInt(Random.value) - .5f) * 2f);
            } else {
                yOffset = (int)((Mathf.RoundToInt(Random.value) - .5f) * 2f);
            }

            Vector2Int offsetVector = new Vector2Int(xOffset, yOffset);
            Debug.Log(offsetVector);
            if (GridCreator.instance.Grid.GetCellAtCoord(coord + offsetVector) != null) {
                if (GridCreator.instance.Grid.GetCellAtCoord(coord + offsetVector).CellState != CellState.Empty) {
                    PickDropCoord(depth - 1);
                } else {
                    coord += offsetVector;
                    return;
                }
            } else {
                PickDropCoord(depth - 1);
            }
        }

        if (_player.Coord == coord) {
            PickDropCoord(500);
        }

        Debug.Log(_player.Coord + ", " + coord);

        GridCreator.instance.Grid.GetCellAtCoord(coord).SetCellState(CellState.Occupied);

        _pickupable.SetCoord(coord);
        transform.position = new Vector3(coord.x * 2, coord.y * 2, transform.position.z);
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void GenerateHitChances(Vector2Int targetCoord) {
        float headMeanHitChance = _weaponScriptableObject.headMeanHitChance;
        float limbMeanHitChance = _weaponScriptableObject.limbMeanHitChance;
        float chestMeanHitChance = _weaponScriptableObject.chestMeanHitChance;

        if (!_weaponScriptableObject.isMelee) {
            float distance = (targetCoord - _player.Coord).magnitude;

            headMeanHitChance = Mathf.Lerp(_weaponScriptableObject.headMeanHitChance, _weaponScriptableObject.headMeanHitChanceMaxDistance, distance / _weaponScriptableObject.attackRadius);
            limbMeanHitChance = Mathf.Lerp(_weaponScriptableObject.limbMeanHitChance, _weaponScriptableObject.limbMeanHitChanceMaxDistance, distance / _weaponScriptableObject.attackRadius);
            chestMeanHitChance = Mathf.Lerp(_weaponScriptableObject.chestMeanHitChance, _weaponScriptableObject.chestMeanHitChanceMaxDistance, distance / _weaponScriptableObject.attackRadius);
        }

        _hitChances.headHitChance = Mathf.Clamp01(NormalDistribution.RandomOverNormalDistribution(headMeanHitChance, _weaponScriptableObject.headHitChanceStandardDeviation));
        _hitChances.chestHitChance = Mathf.Clamp01(NormalDistribution.RandomOverNormalDistribution(chestMeanHitChance, _weaponScriptableObject.chestHitChanceStandardDeviation));
        _hitChances.armsHitChance = Mathf.Clamp01(NormalDistribution.RandomOverNormalDistribution(limbMeanHitChance, _weaponScriptableObject.limbHitChanceStandardDeviation));
        _hitChances.legsHitChance = Mathf.Clamp01(NormalDistribution.RandomOverNormalDistribution(limbMeanHitChance, _weaponScriptableObject.limbHitChanceStandardDeviation));
    }

    public void TakeWeaponDamage() {
        _durability--;
    }

    private void OnDestroy() {
        Player.PickedUpWeapon -= OnWeaponPickedUp;
    }

}
