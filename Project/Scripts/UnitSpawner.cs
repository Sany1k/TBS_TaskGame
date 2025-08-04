using Unity.Netcode;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Transform gameFieldParent;
    [SerializeField] private GameObject meleeUnitPrefab;
    [SerializeField] private GameObject rangeUnitPrefab;
    [SerializeField, Range(1, 4)] private int numberOfMeleeUnits;
    [SerializeField, Range(1, 4)] private int numberOfRangeUnits;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedCallback;
    }

    private void SpawnUnits(ulong clientId)
    {
        float spawnOffset = 1.5f;

        for (int i = 0; i < numberOfMeleeUnits; i++)
        {
            Vector3 spawnHostPosition = new(-24f, 1f, i * spawnOffset - (float)numberOfMeleeUnits / 2);
            GameObject spawnedHostUnit = Instantiate(meleeUnitPrefab, spawnHostPosition, Quaternion.identity, gameFieldParent);
            spawnedHostUnit.GetComponent<NetworkObject>().Spawn();
            spawnedHostUnit.GetComponent<Unit>().unitOwnerIdChangeMaterial.Value = OwnerType.Host;

            Vector3 spawnClientPosition = new(24f, 1f, i * spawnOffset - (float)numberOfMeleeUnits / 2);
            GameObject spawnedClientUnit = Instantiate(meleeUnitPrefab, spawnClientPosition, Quaternion.identity, gameFieldParent);
            spawnedClientUnit.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            spawnedClientUnit.GetComponent<Unit>().unitOwnerIdChangeMaterial.Value = OwnerType.Client;
        }
        for (int i = 0; i < numberOfRangeUnits; i++)
        {
            Vector3 spawnHostPosition = new(-22f, 1f, i * spawnOffset - (float)numberOfMeleeUnits / 2);
            GameObject spawnedHostUnit = Instantiate(rangeUnitPrefab, spawnHostPosition, Quaternion.identity, gameFieldParent);
            spawnedHostUnit.GetComponent<NetworkObject>().Spawn();
            spawnedHostUnit.GetComponent<Unit>().unitOwnerIdChangeMaterial.Value = OwnerType.Host;

            Vector3 spawnClientPosition = new(22f, 1f, i * spawnOffset - (float)numberOfMeleeUnits / 2);
            GameObject spawnedClientUnit = Instantiate(rangeUnitPrefab, spawnClientPosition, Quaternion.identity, gameFieldParent);
            spawnedClientUnit.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            spawnedClientUnit.GetComponent<Unit>().unitOwnerIdChangeMaterial.Value = OwnerType.Client;
        }

        Destroy(gameObject); // Delete spawner
    }

    private void ClientConnectedCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost)
            SpawnUnits(clientId);
    }
}
