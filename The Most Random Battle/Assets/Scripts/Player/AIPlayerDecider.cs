using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerDecider {

    private readonly AIDifficultyLevel _aiDifficultyLevel;

    private readonly Player _player;

    public AIPlayerDecider(AIDifficultyLevel aiDifficultyLevel, Player player) {
        _aiDifficultyLevel = aiDifficultyLevel;
        _player = player;
    }

    public Vector2Int ChooseDirection(bool isFleeing, bool isFleeingBecausePlayer) {
        Vector2Int direction = Vector2Int.zero;

        switch (_aiDifficultyLevel) {
            case AIDifficultyLevel.Random:
                direction = ChooseRandomDirection();
                break;

            case AIDifficultyLevel.Easy:
                direction = ChooseEasyDirection(isFleeing, isFleeingBecausePlayer);
                break;

            case AIDifficultyLevel.Challenge:
                direction = ChooseChallengeDirection(isFleeing, isFleeingBecausePlayer);
                break;
        }

        return direction;
    }

    private Vector2Int ChooseRandomDirection() {
        float rand1 = Random.value;
        float rand2 = Random.value;
        
        return new Vector2Int(rand1 > .5f ? 0 : rand2 < .5f ? 1 : -1, rand1 < .5f ? 0 : rand2 < .5f ? 1 : -1);
    }

    private Vector2Int ChooseEasyDirection(bool isFleeing, bool isFleeingBecausePlayer) {
        Vector2Int direction; 

        bool isGoingForPlayer = Random.value > .5f;

        //makes sure there are chests left
        if (!isGoingForPlayer) {
            isGoingForPlayer = _player.gameManager.Chests.Count == 0;
        }

        //updates isGoingForPlayer based off fleeing status
        if (isFleeing) {
            isGoingForPlayer = isFleeingBecausePlayer;
        }

        if (isGoingForPlayer) {
            //finds closest player
            float closestSquaredDist = float.MaxValue;
            Player closestPlayer = null;
            foreach (Player player in _player.gameManager.Players) {
                if (player == _player || player.Turn.CurrentTurnState == TurnState.Dead) {
                    continue;
                }

                int squaredDist = (player.Coord.x - _player.Coord.x) * (player.Coord.x - _player.Coord.x) + (player.Coord.y - _player.Coord.y) * (player.Coord.y - _player.Coord.y);
                if (squaredDist < closestSquaredDist) {
                    closestSquaredDist = squaredDist;
                    closestPlayer = player;
                }
            }

            //calculate the direction
            if (closestPlayer != null) {
                int deltaX = closestPlayer.Coord.x - _player.Coord.x;
                int deltaY = closestPlayer.Coord.y - _player.Coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                } else {
                    direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                }
            } else {
                Debug.Log("closest player is null");
                direction = Vector2Int.zero;
            }

            //random new direction if it's fleeing
            if (isFleeing) {
                void ChooseAnyOtherDirection() {
                    float rand1 = Random.value;
                    float rand2 = Random.value;
                    Vector2Int newDirection = new Vector2Int(rand1 > .5f ? 0 : rand2 < .5f ? 1 : -1, rand1 < .5f ? 0 : rand2 < .5f ? 1 : -1);

                    if (newDirection == direction) {
                        ChooseAnyOtherDirection();
                    } else {
                        direction = newDirection;
                    }
                }

                ChooseAnyOtherDirection();
            }
        } else {
            if (!isFleeing) {
                //gets closest chest
                float closestSquaredDist = float.MaxValue;
                Chest closestChest = null;

                foreach (Chest chest in _player.gameManager.Chests) {
                    int squaredDist = (chest.Coord.x - _player.Coord.x) * (chest.Coord.x - _player.Coord.x) + (chest.Coord.y - _player.Coord.y) * (chest.Coord.y - _player.Coord.y);
                    if (squaredDist < closestSquaredDist) {
                        closestSquaredDist = squaredDist;
                        closestChest = chest;
                    }
                }

                //calculate the direction
                if (closestChest != null) {
                    int deltaX = closestChest.Coord.x - _player.Coord.x;
                    int deltaY = closestChest.Coord.y - _player.Coord.y;

                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                        direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                    } else {
                        direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                    }
                } else {
                    Debug.Log("closest chest is null");
                    direction = Vector2Int.zero;
                }
            } else {
                //checks neighbors for a pickupable to flee into
                bool isLeftNeighborOccupied = false;
                bool isRightNeighborOccupied = false;
                bool isUpNeighborOccupied = false;
                bool isDownNeighborOccupied = false;

                if (_player.Coord.x > 0) {
                    //left check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord - new Vector2Int(1, 0)).CellState == CellState.Occupied) {
                        isLeftNeighborOccupied = true;
                    }
                }

                if (_player.Coord.x < GridCreator.instance.gridInformation.GridLength - 1) {
                    //right check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord + new Vector2Int(1, 0)).CellState == CellState.Occupied) {
                        isRightNeighborOccupied = true;
                    }
                }

                if (_player.Coord.y > 0) {
                    //down check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord - new Vector2Int(0, 1)).CellState == CellState.Occupied) {
                        isDownNeighborOccupied = true;
                    }
                }

                if (_player.Coord.y < GridCreator.instance.gridInformation.GridHeight - 1) {
                    //up check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord + new Vector2Int(0, 1)).CellState == CellState.Occupied) {
                        isUpNeighborOccupied = true;
                    }
                }

                //sets the direction to the direction of the pickupable
                if (isLeftNeighborOccupied) {
                    direction = Vector2Int.left;
                } else if (isRightNeighborOccupied) {
                    direction = Vector2Int.right;
                } else if (isUpNeighborOccupied) {
                    direction = Vector2Int.up;
                } else if (isDownNeighborOccupied) {
                    direction = Vector2Int.down;
                } else {
                    direction = Vector2Int.zero;
                    Debug.Log("no neighboring pickupable up to flee into (easy player ai)");
                }
            }

        }

        return direction;
    }

    private Vector2Int ChooseChallengeDirection(bool isFleeing, bool isFleeingBecausePlayer) {
        Vector2Int direction;

        //figure out how we're doing
        bool isGoodOnWellness = !(_player.IsArmsCrippled || _player.IsLegsCrippled || _player.Health == 1);
        bool isGoodOnWeapons = !(_player.PlayerWeaponManager.Inventory[1] == null || (_player.PlayerWeaponManager.Inventory[0].WeaponScriptableObject.isMelee && _player.PlayerWeaponManager.Inventory[1].WeaponScriptableObject.isMelee));
        bool isGoodOnPowerUps = true;

        if (isFleeing && isFleeingBecausePlayer) {
            isGoodOnWellness = true;
            isGoodOnWeapons = true;
            isGoodOnPowerUps = true;
        }

    MakeMovementDecision:

        if (!isGoodOnWellness) {
            if (!isFleeing) {
                //find closest power up chest
                float closestChestSquaredDist = float.MaxValue;
                Chest closestChest = null;
                foreach (Chest chest in _player.gameManager.Chests) {
                    if (chest.isWeaponChest) {
                        continue;
                    }

                    int squaredDist = (chest.Coord.x - _player.Coord.x) * (chest.Coord.x - _player.Coord.x) + (chest.Coord.y - _player.Coord.y) * (chest.Coord.y - _player.Coord.y);
                    if (squaredDist < closestChestSquaredDist) {
                        closestChestSquaredDist = squaredDist;
                        closestChest = chest;
                    }
                }

                //find the closest health related power up
                float closestHealthPowerUpSquaredDist = float.MaxValue;
                Pickupable closestHealthPowerUp = null;
                foreach (Pickupable pickupable in _player.gameManager.Pickupables) {
                    if (pickupable.IsWeapon || pickupable.PickupableType == PickupableTypeEnum.Mine || pickupable.PickupableType == PickupableTypeEnum.WeaponDisarmer || pickupable.PickupableType == PickupableTypeEnum.WormHole) {
                        //not interested in that type of pickupable
                        continue;
                    }

                    if ((_player.IsArmsCrippled || _player.IsLegsCrippled) && _player.Health >= 2 && pickupable.PickupableType != PickupableTypeEnum.BandAid) {
                        //intrested in a bandaid not whatever is there
                        continue;
                    }

                    int squaredDist = (pickupable.Coord.x - _player.Coord.x) * (pickupable.Coord.x - _player.Coord.x) + (pickupable.Coord.y - _player.Coord.y) * (pickupable.Coord.y - _player.Coord.y);
                    if (squaredDist < closestHealthPowerUpSquaredDist) {
                        closestHealthPowerUpSquaredDist = squaredDist;
                        closestHealthPowerUp = pickupable;
                    }
                }

                //decide what to go after
                bool isGoingAfterChest;

                if (closestHealthPowerUp == null && closestChest != null) {
                    isGoingAfterChest = true;
                } else if (closestHealthPowerUp != null && closestChest == null) {
                    isGoingAfterChest = false;
                } else if (closestHealthPowerUp != null && closestChest != null) {
                    isGoingAfterChest = Mathf.Sqrt(closestChestSquaredDist) < 3 + Mathf.Sqrt(closestHealthPowerUpSquaredDist);

                    if (_player.IsArmsCrippled || _player.IsLegsCrippled) {
                        isGoingAfterChest = false;
                    }
                } else {
                    //no power up chests left so were as good on wellness as we'll ever be
                    isGoodOnWellness = true;
                    goto MakeMovementDecision;
                }

                //calculate the direction
                if (isGoingAfterChest) {
                    int deltaX = closestChest.Coord.x - _player.Coord.x;
                    int deltaY = closestChest.Coord.y - _player.Coord.y;

                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                        direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                    } else {
                        direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                    }
                } else {
                    int deltaX = closestHealthPowerUp.Coord.x - _player.Coord.x;
                    int deltaY = closestHealthPowerUp.Coord.y - _player.Coord.y;

                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                        direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                    } else {
                        direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                    }
                }
            } else {
                //checks neighbors for a powerup to flee into
                bool isLeftNeighborOccupied = false;
                bool isRightNeighborOccupied = false;
                bool isUpNeighborOccupied = false;
                bool isDownNeighborOccupied = false;

                if (_player.Coord.x > 0) {
                    //left check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord - new Vector2Int(1, 0)).CellState == CellState.Occupied) {
                        isLeftNeighborOccupied = true;
                    }
                }

                if (_player.Coord.x < GridCreator.instance.gridInformation.GridLength - 1) {
                    //right check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord + new Vector2Int(1, 0)).CellState == CellState.Occupied) {
                        isRightNeighborOccupied = true;
                    }
                }

                if (_player.Coord.y > 0) {
                    //down check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord - new Vector2Int(0, 1)).CellState == CellState.Occupied) {
                        isDownNeighborOccupied = true;
                    }
                }

                if (_player.Coord.y < GridCreator.instance.gridInformation.GridHeight - 1) {
                    //up check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord + new Vector2Int(0, 1)).CellState == CellState.Occupied) {
                        isUpNeighborOccupied = true;
                    }
                }

                //sets the direction to the direction of the pickupable
                if (isLeftNeighborOccupied) {
                    direction = Vector2Int.left;
                } else if (isRightNeighborOccupied) {
                    direction = Vector2Int.right;
                } else if (isUpNeighborOccupied) {
                    direction = Vector2Int.up;
                } else if (isDownNeighborOccupied) {
                    direction = Vector2Int.down;
                } else {
                    direction = Vector2Int.zero;
                    Debug.Log("no neighboring pickupable up to flee into (medium player ai)");
                }
            }
        } else if (!isGoodOnWeapons) {
            if (!isFleeing) {
                //find closest ranged weapon
                float closestSquaredDistRangedWeapon = float.MaxValue;
                Pickupable closestRangedWeapon = null;
                foreach (Pickupable rangedWeapon in _player.gameManager.Pickupables) {
                    if (!rangedWeapon.IsWeapon) {
                        continue;
                    } else if (rangedWeapon.GetComponent<Weapon>().WeaponScriptableObject.isMelee) {
                        continue;
                    }

                    int squaredDist = (rangedWeapon.Coord.x - _player.Coord.x) * (rangedWeapon.Coord.x - _player.Coord.x) + (rangedWeapon.Coord.y - _player.Coord.y) * (rangedWeapon.Coord.y - _player.Coord.y);
                    if (squaredDist < closestSquaredDistRangedWeapon) {
                        closestSquaredDistRangedWeapon = squaredDist;
                        closestRangedWeapon = rangedWeapon;
                    }
                }

                //find closest weapon chest
                float closestSquaredDistWeaponChest = float.MaxValue;
                Chest closestWeaponChest = null;
                foreach (Chest weaponChest in _player.gameManager.Chests) {
                    if (!weaponChest.isWeaponChest) {
                        continue;
                    }

                    int squaredDist = (weaponChest.Coord.x - _player.Coord.x) * (weaponChest.Coord.x - _player.Coord.x) + (weaponChest.Coord.y - _player.Coord.y) * (weaponChest.Coord.y - _player.Coord.y);
                    if (squaredDist < closestSquaredDistWeaponChest) {
                        closestSquaredDistWeaponChest = squaredDist;
                        closestWeaponChest = weaponChest;
                    }
                }

                //go for best one
                bool isGoingForRangedWeapon = true;

                if (closestRangedWeapon != null && closestWeaponChest == null) {
                    //check if the weapon is better than the current weapons
                    if (_player.PlayerWeaponManager.Inventory[0] != null) {
                        if (_player.PlayerWeaponManager.Inventory[0].WeaponScore() < closestRangedWeapon.GetComponent<Weapon>().WeaponScore()) {
                            isGoingForRangedWeapon = true;
                        }
                    } else {
                        isGoingForRangedWeapon = true;
                    }

                    if (_player.PlayerWeaponManager.Inventory[1] != null) {
                        if (_player.PlayerWeaponManager.Inventory[1].WeaponScore() < closestRangedWeapon.GetComponent<Weapon>().WeaponScore()) {
                            isGoingForRangedWeapon = true;
                        }
                    } else {
                        isGoingForRangedWeapon = true;
                    }
                } else if (closestRangedWeapon == null && closestWeaponChest != null) {
                    isGoingForRangedWeapon = false;
                } else if (closestRangedWeapon != null && closestWeaponChest != null) {
                    if (closestSquaredDistRangedWeapon < closestSquaredDistWeaponChest && closestRangedWeapon.GetComponent<Weapon>().WeaponScore() > closestWeaponChest.AverageWeaponScore) {
                        //ranged weapon is closer and better
                        isGoingForRangedWeapon = true;
                    } else if (closestSquaredDistRangedWeapon >= closestSquaredDistWeaponChest && closestRangedWeapon.GetComponent<Weapon>().WeaponScore() <= closestWeaponChest.AverageWeaponScore) {
                        //chest is closer and better
                        isGoingForRangedWeapon = false;
                    } else {
                        //could go either way so just leave it up to a 50-50
                        isGoingForRangedWeapon = Random.value > .5f;
                    }
                } else {
                    //no weapons or weapon chests so we're as good on weapons as we'll ever be
                    isGoodOnWeapons = true;
                    goto MakeMovementDecision;
                }

                if (isGoingForRangedWeapon) {
                    int deltaX = closestRangedWeapon.Coord.x - _player.Coord.x;
                    int deltaY = closestRangedWeapon.Coord.y - _player.Coord.y;

                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                        direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                    } else {
                        direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                    }
                } else {
                    int deltaX = closestWeaponChest.Coord.x - _player.Coord.x;
                    int deltaY = closestWeaponChest.Coord.y - _player.Coord.y;

                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                        direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                    } else {
                        direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                    }
                }
            } else {
                //checks neighbors for a weapon to flee into
                bool isLeftNeighborOccupied = false;
                bool isRightNeighborOccupied = false;
                bool isUpNeighborOccupied = false;
                bool isDownNeighborOccupied = false;

                if (_player.Coord.x > 0) {
                    //left check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord - new Vector2Int(1, 0)).CellState == CellState.Occupied) {
                        isLeftNeighborOccupied = true;
                    }
                }

                if (_player.Coord.x < GridCreator.instance.gridInformation.GridLength - 1) {
                    //right check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord + new Vector2Int(1, 0)).CellState == CellState.Occupied) {
                        isRightNeighborOccupied = true;
                    }
                }

                if (_player.Coord.y > 0) {
                    //down check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord - new Vector2Int(0, 1)).CellState == CellState.Occupied) {
                        isDownNeighborOccupied = true;
                    }
                }

                if (_player.Coord.y < GridCreator.instance.gridInformation.GridHeight - 1) {
                    //up check
                    if (GridCreator.instance.Grid.GetCellAtCoord(_player.Coord + new Vector2Int(0, 1)).CellState == CellState.Occupied) {
                        isUpNeighborOccupied = true;
                    }
                }

                //sets the direction to the direction of the weapon
                if (isLeftNeighborOccupied) {
                    direction = Vector2Int.left;
                } else if (isRightNeighborOccupied) {
                    direction = Vector2Int.right;
                } else if (isUpNeighborOccupied) {
                    direction = Vector2Int.up;
                } else if (isDownNeighborOccupied) {
                    direction = Vector2Int.down;
                } else {
                    direction = Vector2Int.zero;
                    Debug.Log("no neighboring weapon up to flee into (medium player ai)");
                }
            }
        } else if (!isGoodOnPowerUps) {
            //finds closest power up chest
            float closestSquaredDistPowerUpChest = float.MaxValue;
            Chest closestChest = null;
            foreach (Chest chest in _player.gameManager.Chests) {
                if (chest.isWeaponChest) {
                    continue;
                }

                int squaredDist = (chest.Coord.x - _player.Coord.x) * (chest.Coord.x - _player.Coord.x) + (chest.Coord.y - _player.Coord.y) * (chest.Coord.y - _player.Coord.y);
                if (squaredDist < closestSquaredDistPowerUpChest) {
                    closestSquaredDistPowerUpChest = squaredDist;
                    closestChest = chest;
                }
            }

            //finds the closest pickupable power up
            float closestSquaredDistPowerUp = float.MaxValue;
            Pickupable closestPowerUp = null;
            foreach (Pickupable pickupable in _player.gameManager.Pickupables) {
                if (pickupable.IsWeapon) {
                    continue;
                }

                int squaredDist = (pickupable.Coord.x - _player.Coord.x) * (pickupable.Coord.x - _player.Coord.x) + (pickupable.Coord.y - _player.Coord.y) * (pickupable.Coord.y - _player.Coord.y);
                if (squaredDist < closestSquaredDistPowerUp) {
                    closestSquaredDistPowerUp = squaredDist;
                    closestPowerUp = pickupable;
                }
            }

            //decides what to go for
            bool isGoingForChest;
            if (closestChest != null && closestPowerUp == null) {
                isGoingForChest = true;
            } else if (closestChest == null && closestPowerUp != null) {
                isGoingForChest = false;
            } else if (closestChest != null && closestPowerUp != null) {
                isGoingForChest = closestSquaredDistPowerUpChest <= closestSquaredDistPowerUp;
            } else {
                Debug.Log("no power ups / power up chests to go for (challenge ai)");
                isGoodOnPowerUps = true;
                goto MakeMovementDecision;
            }

            //calculate the direction
            if (isGoingForChest) {
                int deltaX = closestChest.Coord.x - _player.Coord.x;
                int deltaY = closestChest.Coord.y - _player.Coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                } else {
                    direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                }
            } else {
                int deltaX = closestPowerUp.Coord.x - _player.Coord.x;
                int deltaY = closestPowerUp.Coord.y - _player.Coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                } else {
                    direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                }
            }
        } else {
            //finds closest player
            float closestSquaredDist = float.MaxValue;
            Player closestPlayer = null;
            foreach (Player player in _player.gameManager.Players) {
                if (player == _player || player.Turn.CurrentTurnState == TurnState.Dead) {
                    continue;
                }

                int squaredDist = (player.Coord.x - _player.Coord.x) * (player.Coord.x - _player.Coord.x) + (player.Coord.y - _player.Coord.y) * (player.Coord.y - _player.Coord.y);
                if (squaredDist < closestSquaredDist) {
                    closestSquaredDist = squaredDist;
                    closestPlayer = player;
                }
            }

            //calculate the direction
            if (closestPlayer != null) {
                int deltaX = closestPlayer.Coord.x - _player.Coord.x;
                int deltaY = closestPlayer.Coord.y - _player.Coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                } else {
                    direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                }
            } else {
                Debug.Log("closest player is null");
                direction = Vector2Int.zero;
            }

            //random new direction if it's fleeing
            if (isFleeing) {
                void ChooseAnyOtherDirection() {
                    float rand1 = Random.value;
                    float rand2 = Random.value;
                    Vector2Int newDirection = new Vector2Int(rand1 > .5f ? 0 : rand2 < .5f ? 1 : -1, rand1 < .5f ? 0 : rand2 < .5f ? 1 : -1);

                    if (newDirection == direction || newDirection == -direction) {
                        ChooseAnyOtherDirection();
                    } else {
                        direction = newDirection;
                    }
                }

                ChooseAnyOtherDirection();
            }
        }

        return direction;
    }

    //super buggy hard mode
    /* private struct Thing {
        public GameObject thingsGameObject;
        public Vector2Int coord;
        public float probability;
        public string name;

        public PickupableTypeEnum pickupableType;
       }

     private Vector2Int ChooseHardDirection(bool isFleeing, bool isFleeingBecausePlayer) {
        Vector2Int direction = Vector2Int.zero;

        //check surroundings
        List<Thing> things = new List<Thing>();

        bool checkLeft = true;
        bool checkRight = true;
        bool checkUp = true;
        bool checkDown = true;

        Vector2Int coord;

        bool AddToThingsList(int i) {
            if (coord.x < 0 || coord.x > GridCreator.instance.gridInformation.GridLength - 1 || coord.y < 0 || coord.y > GridCreator.instance.gridInformation.GridHeight - 1) {
                return false;
            }

            if (GridCreator.instance.Grid.GetCellAtCoord(coord).CellState == CellState.Blocked) {

                //something is blocking the player
                if (GridCreator.instance.Grid.GetCellAtCoord(coord).chest != null) {

                    //its a chest
                    if (GridCreator.instance.Grid.GetCellAtCoord(coord).chest.isWeaponChest) {
                        Thing weaponChest = new Thing();

                        weaponChest.coord = coord;
                        weaponChest.thingsGameObject = GridCreator.instance.Grid.GetCellAtCoord(coord).chest.gameObject;
                        weaponChest.probability = Mathf.Clamp01(-(1 / 6f) * i + 1 + (1 / 3f));
                        weaponChest.name = "weapon chest";

                        things.Add(weaponChest);
                        return false;
                    } else {
                        Thing powerUpChest = new Thing();

                        powerUpChest.coord = coord;
                        powerUpChest.thingsGameObject = GridCreator.instance.Grid.GetCellAtCoord(coord).chest.gameObject;
                        powerUpChest.probability = Mathf.Clamp01(-(1 / 6f) * i + 1 + (1 / 3f));
                        powerUpChest.name = "power up chest";

                        things.Add(powerUpChest);
                        return false;
                    }
                } else {

                    //its a player
                    Player player = GridCreator.instance.Grid.GetCellAtCoord(coord).player;

                    bool isPlayerHasThreateningGuns = false;

                    if (player.PlayerWeaponManager.Inventory[0] != null) {
                        if (!player.PlayerWeaponManager.Inventory[0].WeaponScriptableObject.isMelee) {
                            isPlayerHasThreateningGuns = true;
                        }
                    }

                    if (player.PlayerWeaponManager.Inventory[1] != null) {
                        if (!player.PlayerWeaponManager.Inventory[1].WeaponScriptableObject.isMelee) {
                            isPlayerHasThreateningGuns = true;
                        }
                    }

                    if (player.Health > 2 || isPlayerHasThreateningGuns) {
                        Thing threateningPlayer = new Thing();

                        threateningPlayer.coord = coord;
                        threateningPlayer.thingsGameObject = player.gameObject;
                        threateningPlayer.probability = Mathf.Clamp01(-(1 / 6f) * i + 1 + (1 / 3f));
                        threateningPlayer.name = "threatening player";

                        things.Add(threateningPlayer);
                        return false;
                    } else {
                        Thing nonThreateningPlayer = new Thing();

                        nonThreateningPlayer.coord = coord;
                        nonThreateningPlayer.thingsGameObject = player.gameObject;
                        nonThreateningPlayer.probability = Mathf.Clamp01(-(1 / 6f) * i + 1 + (1 / 3f));
                        nonThreateningPlayer.name = "nonthreatening player";

                        things.Add(nonThreateningPlayer);
                        return false;
                    }
                }
            } else if (GridCreator.instance.Grid.GetCellAtCoord(coord).CellState == CellState.Occupied) {

                //something pickupable is on the cells
                Pickupable pickupable = GridCreator.instance.Grid.GetCellAtCoord(coord).pickupableObject;

                bool isPowerUp = !pickupable.IsWeapon;
                bool isMelee = false;

                if (!isPowerUp) {
                    isMelee = pickupable.GetComponent<Weapon>().WeaponScriptableObject.isMelee;
                }

                if (isPowerUp) {
                    //power up
                    Thing powerUp = new Thing();

                    powerUp.coord = coord;
                    powerUp.thingsGameObject = pickupable.gameObject;
                    powerUp.probability = Mathf.Clamp01(-(1 / 6f) * (i - 1) + 1);
                    powerUp.name = "power up";

                    powerUp.pickupableType = pickupable.PickupableType;

                    things.Add(powerUp);
                    return true;

                } else {
                    //weapon
                    if (isMelee) {
                        //melee
                        Thing meleeWeapon = new Thing();

                        meleeWeapon.coord = coord;
                        meleeWeapon.thingsGameObject = pickupable.gameObject;
                        meleeWeapon.probability = Mathf.Clamp01(-(1 / 6f) * (i - 1) + 1);
                        meleeWeapon.name = "melee weapon";

                        meleeWeapon.pickupableType = pickupable.PickupableType;

                        things.Add(meleeWeapon);
                        return true;
                    } else {
                        //ranged
                        Thing rangedWeapon = new Thing();

                        rangedWeapon.coord = coord;
                        rangedWeapon.thingsGameObject = pickupable.gameObject;
                        rangedWeapon.probability = Mathf.Clamp01(-(1 / 6f) * (i - 1) + 1);
                        rangedWeapon.name = "ranged weapon";

                        rangedWeapon.pickupableType = pickupable.PickupableType;

                        things.Add(rangedWeapon);
                        return true;
                    }
                }
            } else {
                //nothing is on the cell
                return true;
            }
        }

        for (int i = 1; i <= 7; i++) {
            if (checkLeft) {
                coord = _player.Coord + Vector2Int.left * i;
                checkLeft = AddToThingsList(i);
            }

            if (checkRight) {
                coord = _player.Coord + Vector2Int.right * i;
                checkRight = AddToThingsList(i);
            }

            if (checkUp) {
                coord = _player.Coord + Vector2Int.up * i;
                checkUp = AddToThingsList(i);
            }

            if (checkDown) {
                coord = _player.Coord + Vector2Int.down * i;
                checkDown = AddToThingsList(i);
            }
        }

        //consider health weapon and power up needs
        bool isGoodOnWellness = !(_player.IsArmsCrippled || _player.IsLegsCrippled || _player.Health == 1);
        bool isGoodOnWeapons = !(_player.PlayerWeaponManager.Inventory[1] == null || (_player.PlayerWeaponManager.Inventory[0].WeaponScriptableObject.isMelee && _player.PlayerWeaponManager.Inventory[1].WeaponScriptableObject.isMelee));

    MakeMovementDecision:

        if (!isGoodOnWellness) {
            //not good on wellness
            bool isBandaidInSurroundings = false;
            Thing bandaidThing = new Thing();

            bool isHeartInSurroundings = false;
            Thing heartThing = new Thing();

            bool isShieldInSurroundings = false;
            Thing shieldThing = new Thing();

            bool isPowerUpChestInSurroundings = false;
            Thing powerUpChestThing = new Thing();


            foreach (Thing thing in things) {
                if (thing.name == "power up") {
                    if (thing.pickupableType == PickupableTypeEnum.BandAid) {
                        isBandaidInSurroundings = true;
                        bandaidThing = thing;
                    } else if (thing.pickupableType == PickupableTypeEnum.Heart) {
                        isHeartInSurroundings = true;
                        heartThing = thing;
                    } else if (thing.pickupableType == PickupableTypeEnum.Shield) {
                        isShieldInSurroundings = true;
                        shieldThing = thing;
                    }
                } else if (thing.name == "power up chest") {
                    isPowerUpChestInSurroundings = true;
                    powerUpChestThing = thing;
                }
            }

            if (_player.IsArmsCrippled || _player.IsLegsCrippled) {
                //not good on limb cnd
                if (isBandaidInSurroundings) {
                    if (!_player.IsLegsCrippled) {
                        direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(bandaidThing.coord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(bandaidThing.coord.y - _player.Coord.y)));
                        return direction;
                    } else {
                        if (bandaidThing.probability >= 0.83f) {
                            direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(bandaidThing.coord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(bandaidThing.coord.y - _player.Coord.y)));
                            return direction;
                        }
                    }
                } else if (isPowerUpChestInSurroundings) {
                    if (powerUpChestThing.probability >= 0.5f) {
                        direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(powerUpChestThing.coord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(powerUpChestThing.coord.y - _player.Coord.y)));
                        return direction;
                    }
                }
            } else {
                //not good on health
                if (isHeartInSurroundings) {
                    direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(heartThing.coord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(heartThing.coord.y - _player.Coord.y)));
                    return direction;
                } else if (isShieldInSurroundings) {
                    direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(shieldThing.coord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(shieldThing.coord.y - _player.Coord.y)));
                    return direction;
                } else if (isPowerUpChestInSurroundings) {
                    if (powerUpChestThing.probability >= 0.5f) {
                        direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(powerUpChestThing.coord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(powerUpChestThing.coord.y - _player.Coord.y)));
                        return direction;
                    }
                }
            }

            //find a close by chest
            float closestSquaredDist = float.MaxValue;
            Chest closestChest = null;
            foreach (Chest chest in _player.gameManager.Chests) {
                if (chest.isWeaponChest) {
                    continue;
                }

                int squaredDist = (chest.Coord.x - _player.Coord.x) * (chest.Coord.x - _player.Coord.x) + (chest.Coord.y - _player.Coord.y) * (chest.Coord.y - _player.Coord.y);
                if (squaredDist < closestSquaredDist) {
                    closestSquaredDist = squaredDist;
                    closestChest = chest;
                }
            }

            //calculate the direction
            if (closestChest != null) {
                int deltaX = closestChest.Coord.x - _player.Coord.x;
                int deltaY = closestChest.Coord.y - _player.Coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                } else {
                    direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                }
            } else {
                //no power up chests left so were as good on wellness as we'll ever be
                isGoodOnWellness = true;
                goto MakeMovementDecision;
            }

            foreach (Thing thing in things) {
                if (thing.name == "threatening player" || thing.name == "nonthreatening player") {
                    Vector2 badDirection = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(thing.coord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(thing.coord.y - _player.Coord.y)));

                    if (badDirection == direction) {
                        //go the slightly more ineficient rout to the chest to avoid the other player

                        int deltaX = closestChest.Coord.x - _player.Coord.x;
                        int deltaY = closestChest.Coord.y - _player.Coord.y;

                        if (Mathf.Abs(deltaX) < Mathf.Abs(deltaY)) {
                            direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                        } else {
                            direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                        }
                    }
                }
            }
        } else if (!isGoodOnWeapons) {
            //check surroundings for weapons
            List<Weapon> surroundingWeapons = new List<Weapon>();
            List<float> surroundingWeaponProbabilities = new List<float>();

            foreach (Thing thing in things) {
                if (thing.name == "melee weapon" || thing.name == "ranged weapon") {
                    surroundingWeapons.Add(thing.thingsGameObject.GetComponent<Weapon>());
                    surroundingWeaponProbabilities.Add(thing.probability);
                }
            }

            //find best weapon in surrounding weapons
            Weapon bestWeaponToGoFor = null;
            float bestWeaponRelativeScore = float.MaxValue;
            for (int i = 0; i < surroundingWeapons.Count; i++) {
                float weaponReletiveScore = surroundingWeaponProbabilities[i] * surroundingWeapons[i].WeaponScore();

                if (weaponReletiveScore < bestWeaponRelativeScore) {
                    bestWeaponToGoFor = surroundingWeapons[i];
                    bestWeaponRelativeScore = weaponReletiveScore;
                }
            }

            //check surroundings for weapon chest
            List<Chest> surroundingWeaponChests = new List<Chest>();
            List<float> surroundingWeaponChestProbabilities = new List<float>();

            foreach (Thing thing in things) {
                if (thing.name == "weapon chest") {
                    surroundingWeaponChests.Add(thing.thingsGameObject.GetComponent<Chest>());
                    surroundingWeaponChestProbabilities.Add(thing.probability);
                }
            }

            //find best weapon chest in surrounding weapons
            Chest bestWeaponChestToGoFor = null;
            float bestWeaponChestRelativeScore = float.MaxValue;
            for (int i = 0; i < surroundingWeaponChests.Count; i++) {
                float weaponChestReletiveScore = surroundingWeaponChestProbabilities[i] * 12.1f;//12.1f is the average weapon score from a fresh weapon droped

                if (weaponChestReletiveScore < bestWeaponChestRelativeScore) {
                    bestWeaponChestToGoFor = surroundingWeaponChests[i];
                    bestWeaponRelativeScore = weaponChestReletiveScore;
                }
            }

            //see whats better, weapon chest or weapon on the ground
            if (bestWeaponRelativeScore > bestWeaponChestRelativeScore) {
                //go for weapon if worth it
                bool isInventorySlot1Empty = _player.PlayerWeaponManager.Inventory[0] == null;
                bool isInventorySlot2Empty = _player.PlayerWeaponManager.Inventory[1] == null;

                if (bestWeaponToGoFor != null) {
                    if (isInventorySlot1Empty || isInventorySlot2Empty) {
                        Vector2Int weaponCoord = bestWeaponToGoFor.GetComponent<Pickupable>().Coord;

                        if (bestWeaponRelativeScore > 4) {
                            direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(weaponCoord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(weaponCoord.y - _player.Coord.y)));
                            return direction;
                        }
                    } else {
                        if (bestWeaponRelativeScore > _player.PlayerWeaponManager.Inventory[0].WeaponScore() || bestWeaponRelativeScore > _player.PlayerWeaponManager.Inventory[1].WeaponScore()) {
                            Vector2Int weaponCoord = bestWeaponToGoFor.GetComponent<Pickupable>().Coord;

                            direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(weaponCoord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(weaponCoord.y - _player.Coord.y)));
                            return direction;
                        }
                    }
                }
            } else {
                //go for chest
                bool isInventorySlot1Empty = _player.PlayerWeaponManager.Inventory[0] == null;
                bool isInventorySlot2Empty = _player.PlayerWeaponManager.Inventory[1] == null;

                if (bestWeaponChestToGoFor != null) {
                    if (isInventorySlot1Empty || isInventorySlot2Empty) {
                        Vector2Int chestCoord = bestWeaponChestToGoFor.Coord;

                        if (bestWeaponChestRelativeScore > 4) {
                            direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(chestCoord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(chestCoord.y - _player.Coord.y)));
                            return direction;
                        }
                    } else {
                        if (bestWeaponChestRelativeScore > _player.PlayerWeaponManager.Inventory[0].WeaponScore() || bestWeaponChestRelativeScore > _player.PlayerWeaponManager.Inventory[1].WeaponScore()) {
                            Vector2Int chestCoord = bestWeaponChestToGoFor.Coord;

                            direction = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp01(chestCoord.x - _player.Coord.x)), Mathf.RoundToInt(Mathf.Clamp01(chestCoord.y - _player.Coord.y)));
                            return direction;
                        }
                    }
                }
            }

            //look for nearby weapon chests and weapons on the ground
            float closestWeaponChestSquaredDist = float.MaxValue;
            Chest closestWeaponChest = null;
            foreach (Chest chest in _player.gameManager.Chests) {
                if (!chest.isWeaponChest) {
                    continue;
                }

                int squaredDist = (chest.Coord.x - _player.Coord.x) * (chest.Coord.x - _player.Coord.x) + (chest.Coord.y - _player.Coord.y) * (chest.Coord.y - _player.Coord.y);
                if (squaredDist < closestWeaponChestSquaredDist) {
                    closestWeaponChestSquaredDist = squaredDist;
                    closestWeaponChest = chest;
                }
            }

            float closestWeaponSquaredDist = float.MaxValue;
            Pickupable closestWeapon = null;
            foreach (Pickupable weapon in _player.gameManager.Pickupables) {
                if (!weapon.IsWeapon) {
                    continue;
                }

                int squaredDist = (weapon.Coord.x - _player.Coord.x) * (weapon.Coord.x - _player.Coord.x) + (weapon.Coord.y - _player.Coord.y) * (weapon.Coord.y - _player.Coord.y);
                if (squaredDist < closestWeaponSquaredDist) {
                    closestWeaponSquaredDist = squaredDist;
                    closestWeapon = weapon;
                }
            }

            //checks if the weapon and chests are worth going for
            bool isWeaponWorthIt = false;
            bool isChestWorthIt = false;

            if (_player.PlayerWeaponManager.Inventory[0] != null) {
                if (closestWeapon != null) {
                    if (_player.PlayerWeaponManager.Inventory[0].WeaponScore() < closestWeapon.GetComponent<Weapon>().WeaponScore()) {
                        isWeaponWorthIt = true;
                    }
                }

                if (closestWeaponChest != null) {
                    if (_player.PlayerWeaponManager.Inventory[0].WeaponScore() < 12.1f) {
                        isChestWorthIt = true;
                    }
                }
            } else {
                isWeaponWorthIt = true;
                isChestWorthIt = true;
            }

            if (_player.PlayerWeaponManager.Inventory[1] != null) {
                if (closestWeapon != null) {
                    if (_player.PlayerWeaponManager.Inventory[1].WeaponScore() < closestWeapon.GetComponent<Weapon>().WeaponScore()) {
                        isWeaponWorthIt = true;
                    }
                }

                if (closestWeaponChest != null) {
                    if (_player.PlayerWeaponManager.Inventory[1].WeaponScore() < 12.1f) {
                        isChestWorthIt = true;
                    }
                }
            } else {
                isWeaponWorthIt = true;
                isChestWorthIt = true;
            }

            if (closestWeapon == null) {
                isWeaponWorthIt = false;
            }

            if (closestWeaponChest == null) {
                isChestWorthIt = false;
            }

            bool isGoingForWeapon;
            if (isWeaponWorthIt && !isChestWorthIt) {
                isGoingForWeapon = true;
            } else if (isWeaponWorthIt && isChestWorthIt) {
                isGoingForWeapon = closestWeapon.GetComponent<Weapon>().WeaponScore() > 12.1f && closestWeaponSquaredDist < closestWeaponChestSquaredDist;
            } else if (!isWeaponWorthIt && isChestWorthIt) {
                isGoingForWeapon = false;
            } else {
                //nothing is worth it, just attack a player
                isGoodOnWeapons = true;
                goto MakeMovementDecision;
            }

            if (isGoingForWeapon) {
                //calculate the direction to the closest weapon
                if (closestWeapon != null) {
                    int deltaX = closestWeapon.Coord.x - _player.Coord.x;
                    int deltaY = closestWeapon.Coord.y - _player.Coord.y;

                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                        direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                        return direction;
                    } else {
                        direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                        return direction;
                    }
                }
            } else {
                //calculate the direction to the closest weapon chest
                if (closestWeaponChest != null) {
                    int deltaX = closestWeaponChest.Coord.x - _player.Coord.x;
                    int deltaY = closestWeaponChest.Coord.y - _player.Coord.y;

                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                        direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                        return direction;
                    } else {
                        direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                        return direction;
                    }
                }
            }

        } else {
            //check surroundings for players
            List<Player> surroundingPlayers = new List<Player>();
            List<float> surroundingPlayersProbabilities = new List<float>();
            List<Player> surroundingThreateningPlayers = new List<Player>();
            List<Player> surroundingNonthreateningPlayers = new List<Player>();

            foreach (Thing thing in things) {
                if (thing.thingsGameObject.GetComponent<Player>()) {
                    Player player = thing.thingsGameObject.GetComponent<Player>();
                    surroundingPlayers.Add(player);
                    surroundingPlayersProbabilities.Add(thing.probability);

                    if (thing.name == "threatening player") {
                        surroundingThreateningPlayers.Add(player);
                    } else {
                        surroundingNonthreateningPlayers.Add(player);
                    }
                }
            }

            if (surroundingPlayers.Count == 0) {
                //no surrounding players so find closest one the old fashion way
                goto FindClosestPlayer;
            }

            //get the closest threatening and nonthreatening players
            Player closestThreateningPlayer = null;
            float highestProbabilityOfReachingAThreateningPlayer = float.MinValue;
            for (int i = 0; i < surroundingThreateningPlayers.Count; i++) {
                float currentProbability = surroundingPlayersProbabilities[surroundingPlayers.IndexOf(surroundingThreateningPlayers[i])];
                if (currentProbability > highestProbabilityOfReachingAThreateningPlayer) {
                    highestProbabilityOfReachingAThreateningPlayer = currentProbability;
                    closestThreateningPlayer = surroundingThreateningPlayers[i];
                }
            }

            Player closestNonthreateningPlayer = null;
            float highestProbabilityOfReachingANonthreateningPlayer = float.MinValue;
            for (int i = 0; i < surroundingNonthreateningPlayers.Count; i++) {
                float currentProbability = surroundingPlayersProbabilities[surroundingPlayers.IndexOf(surroundingNonthreateningPlayers[i])];
                if (currentProbability > highestProbabilityOfReachingANonthreateningPlayer) {
                    highestProbabilityOfReachingAThreateningPlayer = currentProbability;
                    closestNonthreateningPlayer = surroundingNonthreateningPlayers[i];
                }
            }

            //determine what surrounding player to go for
            bool isGoingForThreateningPlayer;
            if (closestThreateningPlayer != null && closestNonthreateningPlayer == null) {
                isGoingForThreateningPlayer = true;
            } else if (closestThreateningPlayer != null && closestNonthreateningPlayer != null) {
                if (highestProbabilityOfReachingAThreateningPlayer <= highestProbabilityOfReachingANonthreateningPlayer) {
                    isGoingForThreateningPlayer = true;
                } else {
                    if (highestProbabilityOfReachingAThreateningPlayer + (1/3f) < highestProbabilityOfReachingANonthreateningPlayer) {
                        isGoingForThreateningPlayer = true;
                    } else {
                        isGoingForThreateningPlayer = false;
                    }
                }
            } else if (closestThreateningPlayer == null && closestNonthreateningPlayer != null) {
                isGoingForThreateningPlayer = false;
            } else {
                //no one there
                goto FindClosestPlayer;
            }

            //calculate direction to the target player
            if (isGoingForThreateningPlayer) {
                int deltaX = closestThreateningPlayer.Coord.x - _player.Coord.x;
                int deltaY = closestThreateningPlayer.Coord.y - _player.Coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                    goto FleeStatusCheck;
                } else {
                    direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                    goto FleeStatusCheck;
                }
            } else {
                int deltaX = closestNonthreateningPlayer.Coord.x - _player.Coord.x;
                int deltaY = closestNonthreateningPlayer.Coord.y - _player.Coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                    goto FleeStatusCheck;
                } else {
                    direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                    goto FleeStatusCheck;
                }
            }

        FindClosestPlayer:
            //find the closest player
            Player closestPlayer = null;
            int closestPlayerSquaredDist = int.MaxValue;
            foreach (Player player in _player.gameManager.Players) {
                int squaredDist = (player.Coord - _player.Coord).sqrMagnitude;
                if (squaredDist < closestPlayerSquaredDist) {
                    closestPlayerSquaredDist = squaredDist;
                    closestPlayer = player;
                }
            }

            //calculate direction to closest player
            if (closestPlayer != null) {
                int deltaX = closestPlayer.Coord.x - _player.Coord.x;
                int deltaY = closestPlayer.Coord.y - _player.Coord.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                    direction = deltaX < 0 ? Vector2Int.left : Vector2Int.right;
                } else {
                    direction = deltaY < 0 ? Vector2Int.down : Vector2Int.up;
                }
            } else {
                //player should win becuase no one else is alive
                Debug.Log("no players on grid (hard player ai)");
            }

            FleeStatusCheck:
            //change direction based on flee status
            void GetFleeDirection(int depth) {
                if (depth == 0) {
                    return;
                }

                if (isFleeing && isFleeingBecausePlayer) {
                    Vector2Int fleeDirection = new Vector2Int();
                    if (direction == Vector2Int.up || direction == Vector2Int.down) {
                        fleeDirection = Random.value > .5f ? Vector2Int.left : Vector2Int.right;
                    } else if (direction == Vector2Int.left || direction == Vector2Int.right) {
                        fleeDirection = Random.value > .5f ? Vector2Int.up : Vector2Int.down;
                    }

                    direction = fleeDirection;
                }

                foreach (Thing player in things) {
                    if (!(player.name == "threatening player" || player.name == "nonthreatening player")) {
                        continue;
                    }

                    if (System.Math.Sign(player.coord.x - _player.Coord.x) == direction.x && System.Math.Sign(player.coord.y - _player.Coord.y) == direction.y) {
                        if (player.probability > 0.3f) {
                            GetFleeDirection(depth - 1);
                        }
                    }
                }
            }

            GetFleeDirection(10);
        }

        return direction;
    }*/
    
    public int ChooseLimb(Vector4 percents, Player target) {
        int limbIndex = 0;

        switch (_aiDifficultyLevel) {
            case AIDifficultyLevel.Random:
                limbIndex = Random.Range(0, 5);
                break;

            case AIDifficultyLevel.Easy:
                limbIndex = ChooseEasyLimb(percents);
                break;

            case AIDifficultyLevel.Challenge:
                limbIndex = ChooseChallengeLimb(percents, target);
                break;
        }

        return limbIndex;
    }

    private int ChooseEasyLimb(Vector4 percents) {
        int limb = 0;

        float highestPercent = float.MinValue;

        if (percents.x > highestPercent) {
            highestPercent = percents.x;
            limb = 0;
        }

        if (percents.y > highestPercent) {
            highestPercent = percents.y;
            limb = 1;
        }

        if (percents.z > highestPercent) {
            highestPercent = percents.z;
            limb = 2;
        }

        if (percents.w > highestPercent) {
            limb = 3;
        }

        return limb;
    }

    private int ChooseChallengeLimb(Vector4 percents, Player target) {
        //checks target players health
        if (target.Health == 3) {
            //target is full

            if (percents.x > .33f) {
                //target head
                return 0;
            }

            if (percents.z > 0.5f || percents.w > 0.5f) {
                //target limbs
                return percents.z > percents.w ? 2 : 3;

            }

            //target chest
            return 1;
        } else if (target.Health == 2) {
            //target is 2 health

            if (percents.x > .4f) {
                //target head
                return 0;
            }

            if (percents.z > 0.4f || percents.w > 0.4f) {
                //target limbs
                return percents.z > percents.w ? 2 : 3;
            }

            //target chest
            return 1;
        } else {
            //target is a one shot
            
            if (percents.x > percents.y) {
                //target head
                return 0;
            }

            //target chest
            return 1;
        }
    }

    public Cell ChooseInteractCell(List<Cell> interactableCells) {
        Cell interactCell = null;

        switch (_aiDifficultyLevel) {
            case AIDifficultyLevel.Random:
                interactCell = ChooseRandomCell(interactableCells);
                break;

            case AIDifficultyLevel.Easy:
                interactCell = ChooseBothEasyChallengCell(interactableCells);
                break;

            case AIDifficultyLevel.Challenge:
                interactCell = ChooseBothEasyChallengCell(interactableCells);
                break;
        }

        return interactCell;
    }

    private Cell ChooseRandomCell(List<Cell> interactableCells) {
        //chooses randomly from all blocked cells
        List<Cell> blockedCells = new List<Cell>();
        foreach (Cell cell in interactableCells) {
            if (cell.CellState == CellState.Blocked) {
                blockedCells.Add(cell);
            }
        }

        int randCellIndex = Mathf.FloorToInt(blockedCells.Count * Random.value);

        if (blockedCells.Count > 0) {
            return blockedCells[randCellIndex];
        }

        randCellIndex = Mathf.FloorToInt(interactableCells.Count * Random.value);
        return interactableCells[randCellIndex];
    }

    private Cell ChooseBothEasyChallengCell(List<Cell> interactableCells) {
        //get all possible chests and players to interact with
        List<Cell> interactableChests = new List<Cell>();
        List<Cell> interactablePlayers = new List<Cell>();

        foreach (Cell cell in interactableCells) {
            if (cell.CellState != CellState.Blocked) {
                continue;
            }

            if (cell.player != null) {
                if (cell.player == _player) {
                    continue;
                }

                interactablePlayers.Add(cell);
            } else if (cell.chest != null) {
                if ((cell.CellCoord - _player.Coord).sqrMagnitude > 1) {
                    continue;
                }

                interactableChests.Add(cell);
            }
        }

        //determine needs
        bool isNeedHealth = _player.Health < 3 || _player.IsArmsCrippled || _player.IsLegsCrippled;
        bool isNeedWeapon = !(_player.PlayerWeaponManager.Inventory[1] == null || (_player.PlayerWeaponManager.Inventory[0].WeaponScriptableObject.isMelee && _player.PlayerWeaponManager.Inventory[1].WeaponScriptableObject.isMelee));

        //pick one
        Cell returnCell = null;

        //chests
        bool isPowerUpChestThere = false;
        foreach (Cell chestCell in interactableChests) {
            if (!chestCell.chest.isWeaponChest) {
                isPowerUpChestThere = true;
                returnCell = chestCell;
                break;
            }
        }

        if (isNeedHealth) {
            if (isPowerUpChestThere) {
                return returnCell;
            }
        }

        bool isWeaponChestThere = false;
        foreach (Cell chestCell in interactableChests) {
            if (chestCell.chest.isWeaponChest) {
                isWeaponChestThere = true;
                returnCell = chestCell;
                break;
            }
        }

        if (isNeedWeapon) {
            if (isWeaponChestThere) {
                return returnCell;
            }
        }

        //players
        if (interactablePlayers.Count == 0) {
            returnCell = interactableChests[0];
        } else {
            if (_aiDifficultyLevel == AIDifficultyLevel.Easy) {
                //choose a random cell
                returnCell = interactablePlayers[Mathf.FloorToInt(interactablePlayers.Count * Random.value)];
            } else {
                //choose closest cell
                Cell closestCell = null;
                float closestSquaredDist = float.MaxValue;
                foreach (Cell playerCell in interactablePlayers) {
                    float squaredDist = (playerCell.CellCoord - _player.Coord).sqrMagnitude;
                    if (squaredDist < closestSquaredDist) {
                        closestSquaredDist = squaredDist;
                        closestCell = playerCell;
                    }
                }

                returnCell = closestCell;
            }
        }

        

        return returnCell;
    }
}

public enum AIDifficultyLevel {
    Random,
    Easy,
    Challenge,
}