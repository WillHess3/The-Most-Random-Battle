using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : Player {

    private AIPlayerDecider _aiPlayerDecider;
    private bool _isFleeingFromPlayer;

    private void Start() {
        _isControllablePlayer = false;

        Spawn();

        _interactableCells = new List<Cell>();

        _turn = new Turn(this);

        _aiPlayerDecider = new AIPlayerDecider(AIDifficultyLevel.Challenge, this);
    }

    public override void ChooseDirection() {
        _direction = _aiPlayerDecider.ChooseDirection(_turn.CurrentTurnState == TurnState.Fleeing, _isFleeingFromPlayer);
        _isFleeingFromPlayer = false;

        //equip weapon
        if (_playerWeaponManager.Inventory[1] != null) {
            _playerWeaponManager.Equip(1);
            PlayerWeaponEquip.ChangePlayerSprite(this);
        }

        _turn.Move();
    }

    public override void StartMovingProcess() {
        Move(Mathf.CeilToInt(Random.value * 6), _direction);
    }

    public override void Interact() {
        //check if interacting is possible
        if (IsInteractingPossible((int)_playerWeaponManager.EquipedWeapon.WeaponScriptableObject.attackRadius)) {
            //pick which cell to interact with
            Cell selectedCell = _interactableCells[0];

            if (selectedCell.player != null && selectedCell.player != this) {
                //set the player
                _playerWeAreAttacking = selectedCell.player;

                //generate hit chances
                _playerWeaponManager.EquipedWeapon.GenerateHitChances(selectedCell.CellCoord);

                //interact by getting limb
                Weapon weapon = _playerWeaponManager.EquipedWeapon;
                Vector4 weaopnPercents = new Vector4(weapon.WeaponHitChances.headHitChance, weapon.WeaponHitChances.chestHitChance, weapon.WeaponHitChances.armsHitChance, weapon.WeaponHitChances.legsHitChance);

                int limbIndex = _aiPlayerDecider.ChooseLimb(weaopnPercents, _playerWeAreAttacking);

                //attack selected player's limb
                Limbs limb = Limbs.Chest;

                switch (limbIndex) {
                    case 0:
                        limb = Limbs.Head;
                        break;
                    case 1:
                        limb = Limbs.Chest;
                        break;
                    case 2:
                        limb = Limbs.Arms;
                        break;
                    case 3:
                        limb = Limbs.Legs;
                        break;
                }

                _playerWeaponManager.Attack(_playerWeAreAttacking, limb);

                //flee
                _isFleeingFromPlayer = true;
                _turn.Flee();
            } else if (selectedCell.chest != null) {
                //open chest
                selectedCell.chest.OpenChest();

                //flee
                _turn.Flee();
            }
        } else {
            _turn.EndTurn();
        }
    }

    public override void Flee() {
        ChooseDirection();
    }

    public override void ReplaceWeapon() {
        throw new System.NotImplementedException();
    }

    private void Update() {
        if (_isInMotion) {
            InMotion();
        }

        //decrease flash red timer
        if (_flashRedTimer > 0) {
            _flashRedTimer -= 2 * Time.deltaTime;
            _spriteRenderer.material.SetFloat("_RedAmount", _flashRedTimer);
        } else {
            if (_flashRedTimer != 0) {
                _flashRedTimer = 0;
                _spriteRenderer.material.SetFloat("_RedAmount", _flashRedTimer);
            }
        }
    }
}
