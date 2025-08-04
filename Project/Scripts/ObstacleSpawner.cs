using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private Transform gameFieldParent;

    private readonly List<GameObject> obstacleList = new();
    private NavMeshSurface navSurface;

    private void Awake()
    {
        navSurface = gameFieldParent.GetComponent<NavMeshSurface>();
        HostObstacleSpawn();

        navSurface.BuildNavMesh();
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedCallback;
    }

    private void HostObstacleSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            float w = 0f;

            for (int r = 0; r <= 1; r++)
            {
                for (float l = -19; l < 11;)
                {
                    l += Random.Range(4f, 5f);

                    if (r == 0)
                        w = Random.Range(-11f, -3f);
                    else if (r == 1)
                        w = Random.Range(3f, 11f);

                    GameObject rndObstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                    Vector3 positionOffset = new(l, 1, w);
                    GameObject spawnedObstacle = Instantiate(rndObstaclePrefab, rndObstaclePrefab.transform.position + positionOffset,
                        Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), gameFieldParent);

                    obstacleList.Add(spawnedObstacle);
                }
            }
        }
    }

    private void ClientConnectedCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.IsClient)
        {
            foreach (var obstacle in obstacleList)
            {
                obstacle.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
