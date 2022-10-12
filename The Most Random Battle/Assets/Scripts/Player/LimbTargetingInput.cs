using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LimbTargetingInput : MonoBehaviour {

    private bool _isLimbTargetOptionsShowing;

    [SerializeField] private GameObject _attackButtons;
    [SerializeField] private List<TMP_Text> _limbPercents;

    public static event Action<Limbs> LimbSelected;

    private void Awake() {
        ControllablePlayer.ToggleAttackButtons += OnToggleLimbTargetOptions;
    }

    private void OnToggleLimbTargetOptions(Vector4 percents) {
        _isLimbTargetOptionsShowing = !_isLimbTargetOptionsShowing;

        SetLimbPercents(percents);
        _attackButtons.SetActive(_isLimbTargetOptionsShowing);
    }

    public void OnLimbSelected(int limbIndex) {
        Limbs limb;

        if (limbIndex == 0) {
            limb = Limbs.Head;
        } else if (limbIndex == 1) {
            limb = Limbs.Chest;
        } else if (limbIndex == 2) {
            limb = Limbs.Arms;
        } else {
            limb = Limbs.Legs;
        }

        LimbSelected?.Invoke(limb);

        OnToggleLimbTargetOptions(Vector4.zero);
    }

    private void SetLimbPercents(Vector4 percents) {
        _limbPercents[0].text = Mathf.RoundToInt(percents.x * 100).ToString() + "%";
        _limbPercents[1].text = Mathf.RoundToInt(percents.y * 100).ToString() + "%";
        _limbPercents[2].text = Mathf.RoundToInt(percents.z * 100).ToString() + "%";
        _limbPercents[3].text = Mathf.RoundToInt(percents.w * 100).ToString() + "%";
    }

    private void OnDestroy() {
        ControllablePlayer.ToggleAttackButtons -= OnToggleLimbTargetOptions;
    }
}
