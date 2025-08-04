using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkData : NetworkBehaviour
{
    [HideInInspector] public readonly NetworkVariable<OwnerType> ownerTurn = new(OwnerType.Host);
    [HideInInspector] public readonly NetworkVariable<int> gameTimer = new();
    [HideInInspector] public readonly NetworkVariable<int> gameTurnsCount = new();

    private Coroutine currentRoutine;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameManager.Instance.CreateNetData();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchTurnServerRpc()
    {
        StopCoroutine(currentRoutine);

        if (ownerTurn.Value == OwnerType.Host)
            ownerTurn.Value = OwnerType.Client;
        else
            ownerTurn.Value = OwnerType.Host;

        gameTurnsCount.Value++;
        StartTimer();
    }

    public void StartTimer()
    {
        currentRoutine = StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        gameTimer.Value = 60;

        while (gameTimer.Value > 0)
        {
            yield return new WaitForSeconds(1f);
            gameTimer.Value -= 1;
        }

        GameManager.Instance.SwitchPlayerTurn();
    }
}
