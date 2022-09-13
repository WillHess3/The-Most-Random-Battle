using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Customize Game")]
    [SerializeField] private bool _isCameraTrackingAllPlayers;
    [SerializeField] private int _numberOfPlayers;
    [SerializeField] private int _numberOfControllablePlayers;

    [Space]
    [SerializeField] private int _numberOfPowerUpChests;
    [SerializeField] private int _numberOfWeaponChests;

    private Player[] _players;
    private Player _activePlayer;
    private int _activePlayerIndex = -1;

    public Player[] Players => _players;

    private int _numberOfReadyPlayers = 0;

    private List<Chest> _chests;

    private bool _isRoundInProgress;

    [Header("Prefabs")]
    [SerializeField] private GameObject _controllablePlayerPrefab;
    [SerializeField] private GameObject _aiPlayerPrefab;

    [Space]
    [SerializeField] private GameObject _powerUpChestPrefab;
    [SerializeField] private GameObject _weaponChestPrefab;

    public static event Action<Player> SwitchCameraTarget;

    private List<int> _unavailableColorIndecies = new List<int>();
    public List<int> UnavailableColorIndecies => _unavailableColorIndecies;
    public void SetUnavailableColor(int index) => _unavailableColorIndecies.Add(index);

    private void Awake() {
        SetUpGame();
    }

    private void SetUpGame() {
        //set up chests
        _chests = new List<Chest>();
        for (int i = 0; i < _numberOfPowerUpChests; i++) {
            GameObject chest = Instantiate(_powerUpChestPrefab);
            _chests.Add(chest.GetComponent<Chest>());
        }

        for (int i = 0; i < _numberOfPowerUpChests; i++) {
            GameObject chest = Instantiate(_weaponChestPrefab);
            _chests.Add(chest.GetComponent<Chest>());
        }

        //set up players
        _players = new Player[_numberOfPlayers];

        int controllablePlayerCount = 0;
        for (int i = 0; i < _players.Length; i++) {
            if (controllablePlayerCount < _numberOfControllablePlayers) {
                controllablePlayerCount++;

                GameObject player = Instantiate(_controllablePlayerPrefab);
                _players[i] = player.GetComponent<ControllablePlayer>();
                _players[i].GameManager = this;
            } else {
                GameObject player = Instantiate(_aiPlayerPrefab);
                _players[i] = player.GetComponent<AIPlayer>();
                _players[i].GameManager = this;
            }
        }

        ShufflePlayerArray();
    }

    public void ShufflePlayerArray() {
        Player tempPlayer;

        for (int i = 0; i < _players.Length; i++) {
            int randomIndex = UnityEngine.Random.Range(0, _players.Length);
            tempPlayer = _players[randomIndex];
            _players[randomIndex] = _players[i];
            _players[i] = tempPlayer;
        }
    }

    //makes sure all players are spawned in before the game starts
    public void PlayerReady() {
        _numberOfReadyPlayers++;

        if (_numberOfReadyPlayers == _numberOfPlayers) {
            StartGame();
        }
    }

    private void StartGame() {
        //sets initial camera target
        if (!_isCameraTrackingAllPlayers) {
            foreach (Player player in _players) {
                Debug.Log(player);
                if (player.IsControllablePlayer) {
                    SwitchCameraTarget?.Invoke(player);
                    break;
                }
            }
        } else {
            SwitchCameraTarget?.Invoke(_players[0]);
        }
        
        //initiates starting process
        StartCoroutine(DelayRoundStart());
    }

    private IEnumerator DelayRoundStart() {
        yield return new WaitForSeconds(1);

        _isRoundInProgress = true;
        NextTurn();
    }

    private void NextTurn() {
        if (_activePlayerIndex < _players.Length - 1) {
            _activePlayerIndex++;
            _activePlayer = _players[_activePlayerIndex];

            if (_isCameraTrackingAllPlayers || (_isCameraTrackingAllPlayers && _activePlayer.IsControllablePlayer)) {
                SwitchCameraTarget?.Invoke(_activePlayer);
            }

            StartCoroutine(StartNextTurnAfterDelay());
        } else {
            _isRoundInProgress = false;

            _activePlayerIndex = -1;

            StartCoroutine(DelayRoundStart());
        }
    }

    private IEnumerator StartNextTurnAfterDelay() {
        yield return new WaitForSeconds(1);
        _activePlayer.Turn.StartTurn();
    }

    private void Update() {
        if (_isRoundInProgress) {
            if (_activePlayer.Turn.CurrentTurnState == TurnState.Done) {
                _activePlayer.Turn.ConfirmTurnFinished();

                NextTurn();
            }
        }
    }

}
