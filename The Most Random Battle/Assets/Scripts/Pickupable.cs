using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour {

    [SerializeField] private List<Sprite> _sprites;
    private Dictionary<PickupableTypeEnum, Sprite> _pickupableSpritesDictionary;

    private SpriteRenderer _spriteRenderer;

    private PickupableTypeEnum _pickupableType;

    public PickupableTypeEnum PickupableType => _pickupableType;

    private Vector2Int _coord;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        //populates dictionary
        _pickupableSpritesDictionary = new Dictionary<PickupableTypeEnum, Sprite>();

        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Knife, _sprites[0]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Chainsaw, _sprites[1]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Hammer, _sprites[2]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Machette, _sprites[3]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Spear, _sprites[4]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Pistol, _sprites[5]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Shotgun, _sprites[6]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Sniper, _sprites[7]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.SMG, _sprites[8]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.HomingPistol, _sprites[9]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Heart, _sprites[10]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.BandAid, _sprites[11]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.WormHole, _sprites[12]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.WeaponDisarmer, _sprites[13]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Mine, _sprites[14]);
        _pickupableSpritesDictionary.Add(PickupableTypeEnum.Shield, _sprites[15]);
    }

    public void SetType(PickupableTypeEnum pickupableType) {
        _pickupableType = pickupableType;

        _spriteRenderer.sprite = _pickupableSpritesDictionary[_pickupableType];
    }

    public void SetCoord(Vector2Int coord) {
        _coord = coord;
        GridCreator.instance.Grid.GetCellAtCoord(_coord).pickupableObject = this;
    }
}

public enum PickupableTypeEnum {
    Knife,
    Chainsaw,
    Hammer,
    Machette,
    Spear,
    Pistol,
    Shotgun,
    Sniper,
    SMG,
    HomingPistol,
    Heart,
    BandAid,
    WormHole,
    WeaponDisarmer,
    Mine,
    Shield
}
