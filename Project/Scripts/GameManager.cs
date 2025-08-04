using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Action<OwnerType> PlayerTurnEndEvent;
    public Action<int> TimerChangedEvent;
    public Action<int> NextGameTurnsEvent;

    [SerializeField] private GameObject networkDataObjectPrefab;
    private NetworkData networkData;
    private GameplayUIBehaviour gameplayUI;
    private int attackRemains = 1;
    private int movesRemains = 1;
    private bool hasNetworkDataSpawned = false;

    public OwnerType LocalOwnerTurn { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        gameplayUI = GetComponent<GameplayUIBehaviour>();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }

    public void CreateNetData()
    {
        networkData = FindFirstObjectByType<NetworkData>();
        hasNetworkDataSpawned = true;
        if (NetworkManager.Singleton.IsHost) networkData.StartTimer();
        SubscribeOnNetworkDataValueChanged();
        gameplayUI.SubscribeOnGameManagerEvents();
    }

    public bool YourTurn()
    {
        bool isYourTurn = false;

        if (hasNetworkDataSpawned)
        {
            isYourTurn = LocalOwnerTurn == networkData.ownerTurn.Value;
        }

        return isYourTurn;
    }

    public bool HasAttacks()
    {
        return attackRemains > 0;
    }

    public bool HasMoves()
    {
        return movesRemains > 0;
    }

    public void SubtractAttack()
    {
        if (attackRemains > 0)
        {
            attackRemains--;
            CheckEndTactics();
        }

        gameplayUI.UpdateAttacksAmout(attackRemains);
    }

    public void SubtractMove()
    {
        if (movesRemains > 0)
        {
            movesRemains--;
            CheckEndTactics();
        }

        gameplayUI.UpdateMovesAmout(movesRemains);
    }

    public void SwitchPlayerTurn()
    {
        networkData.SwitchTurnServerRpc();
        UpdateLocalTacticks();
    }

    private void UpdateLocalTacticks()
    {
        gameplayUI.UpdateMovesAmout(movesRemains = 1);
        gameplayUI.UpdateAttacksAmout(attackRemains = 1);
    }

    private void CheckEndTactics()
    {
        if (attackRemains == 0 && movesRemains == 0)
        {
            SwitchPlayerTurn();
        }
    }

    private void SubscribeOnNetworkDataValueChanged()
    {
        networkData.ownerTurn.OnValueChanged += OnPlayerTurnChanged;
        networkData.gameTimer.OnValueChanged += OnTimerValueChanged;
        networkData.gameTurnsCount.OnValueChanged += OnNextTurnValue;
    }

    private void OnClientConnectedCallback(ulong clientId)
    {

        if (NetworkManager.Singleton.IsHost)
        {
            LocalOwnerTurn = OwnerType.Host;
            GameObject go = Instantiate(networkDataObjectPrefab);
            go.GetComponent<NetworkObject>().Spawn();
        }
        else
            LocalOwnerTurn = OwnerType.Client;

        gameplayUI.UpdateAttacksAmout(attackRemains);
        gameplayUI.UpdateMovesAmout(movesRemains);
    }

    private void OnPlayerTurnChanged(OwnerType previousType, OwnerType newType)
    {
        PlayerTurnEndEvent?.Invoke(newType);
    }

    private void OnTimerValueChanged(int previousValue, int newValue)
    {
        TimerChangedEvent?.Invoke(newValue);
    }

    private void OnNextTurnValue(int previousValue, int newValue)
    {
        NextGameTurnsEvent?.Invoke(newValue);
    }
}
