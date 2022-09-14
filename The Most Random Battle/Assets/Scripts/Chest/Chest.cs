using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    private Vector2Int _coord;
    public Vector2Int Coord => _coord;

    public GameManager gameManager;
    public GameObject pickupablePrefab;

    public bool isWeaponChest;

    private List<PickupableTypeEnum> _items;
    private List<float> _distributionProportions;
    
    private Distribution<PickupableTypeEnum> _itemDistribution;


    private void Start() {
        PickCoord();

        _items = new List<PickupableTypeEnum>();
        _distributionProportions = new List<float>();

        if (isWeaponChest) {
            _items.Add(PickupableTypeEnum.Chainsaw);
            _distributionProportions.Add(0.111f);

            _items.Add(PickupableTypeEnum.Hammer);
            _distributionProportions.Add(0.111f);

            _items.Add(PickupableTypeEnum.Machette);
            _distributionProportions.Add(0.111f);

            _items.Add(PickupableTypeEnum.Spear);
            _distributionProportions.Add(0.111f);

            _items.Add(PickupableTypeEnum.Pistol);
            _distributionProportions.Add(0.111f);

            _items.Add(PickupableTypeEnum.Shotgun);
            _distributionProportions.Add(0.111f);

            _items.Add(PickupableTypeEnum.Sniper);
            _distributionProportions.Add(0.111f);

            _items.Add(PickupableTypeEnum.SMG);
            _distributionProportions.Add(0.111f);

            _items.Add(PickupableTypeEnum.HomingPistol);
            _distributionProportions.Add(0.112f);
        } else {
            _items.Add(PickupableTypeEnum.Heart);
            _distributionProportions.Add(0.17f);

            _items.Add(PickupableTypeEnum.BandAid);
            _distributionProportions.Add(0.166f);

            _items.Add(PickupableTypeEnum.WormHole);
            _distributionProportions.Add(0.166f);

            _items.Add(PickupableTypeEnum.WeaponDisarmer);
            _distributionProportions.Add(0.166f);

            _items.Add(PickupableTypeEnum.Mine);
            _distributionProportions.Add(0.166f);

            _items.Add(PickupableTypeEnum.Shield);
            _distributionProportions.Add(0.166f);
        }

        _itemDistribution = new Distribution<PickupableTypeEnum>(_items, _distributionProportions);
    }

    private void PickCoord() {
        Vector2Int randCoord = new Vector2Int(Mathf.FloorToInt(Random.value * GridCreator.instance.gridInformation.GridLength), Mathf.FloorToInt(Random.value * GridCreator.instance.gridInformation.GridHeight));

        if (GridCreator.instance.Grid.GetCellAtCoord(randCoord).CellState == CellState.Blocked) {
            PickCoord();
        } else {
            _coord = randCoord;
            GridCreator.instance.Grid.GetCellAtCoord(randCoord).SetCellState(CellState.Blocked);
            transform.position = GridCreator.instance.gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0);
        }
    }

    public void OpenChest() {
        Debug.Log("chest opened");
        //disable chest
        gameManager.Chests.Remove(this);
        gameObject.SetActive(false);

        //unblock tile
        GridCreator.instance.Grid.GetCellAtCoord(_coord).SetCellState(CellState.Occupied);

        //spawn reward
        GameObject pickupableObject = Instantiate(pickupablePrefab, GridCreator.instance.gridInformation.cellSize * new Vector3(_coord.x, _coord.y, 0), Quaternion.identity);
        pickupableObject.GetComponent<Pickupable>().SetType(_itemDistribution.RandomFromDistribution());
    }

    public void SetCoord(Vector2Int coord) {
        _coord = coord;
    }
}
