using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class DieRoller : MonoBehaviour {

    public static event Action<int> RollDie;

    [SerializeField] private List<Sprite> _sprites;
    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void OnDieRolled() {
        int _rolledNumber = Mathf.CeilToInt(UnityEngine.Random.value * 6);

        _image.sprite = _sprites[_rolledNumber - 1];

        RollDie?.Invoke(_rolledNumber);
    }

}
