using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public enum OwnerType : byte
{
    None = 0,
    Host = 1,
    Client = 2
}
public class Unit : NetworkBehaviour
{
    [SerializeField] private float attackRange;
    [SerializeField] private float moveDistance;
    [SerializeField] private GameObject walkArea;
    [SerializeField] private GameObject attackArea;
    [SerializeField] private GameObject pickupMark;
    [SerializeField] private GameObject targetMark;
    [SerializeField] private MeshRenderer unitOwnMesh;
    [SerializeField] private Material hostMaterial;
    [SerializeField] private Material clientMaterial;

    [HideInInspector] public readonly NetworkVariable<OwnerType> unitOwnerIdChangeMaterial = new();
    private NavMeshAgent navAgent;

    public float AttackRange { get => attackRange; }
    public float MoveDistance { get => moveDistance; }

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        walkArea.transform.localScale *= moveDistance;
        attackArea.transform.localScale *= attackRange;
    }

    public void UnitSelected()
    {
        pickupMark.SetActive(true);
        walkArea.SetActive(true);
        attackArea.SetActive(true);
    }

    public void UnitDeselected()
    {
        pickupMark.SetActive(false);
        walkArea.SetActive(false);
        attackArea.SetActive(false);
    }

    [ServerRpc]
    public void GoToDestinationServerRpc(Vector3 point)
    {
        if (Vector3.Distance(transform.position, point) <= MoveDistance)
        {
            navAgent.destination = point;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void KillUnitServerRpc()
    {
        Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        unitOwnerIdChangeMaterial.OnValueChanged += OnChangeMaterial;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        unitOwnerIdChangeMaterial.OnValueChanged -= OnChangeMaterial;
    }

    private void OnChangeMaterial(OwnerType previousValue, OwnerType newValue)
    {
        switch (newValue)
        {
            case OwnerType.Host:
                unitOwnMesh.sharedMaterial = hostMaterial;
                break;
            case OwnerType.Client:
                unitOwnMesh.sharedMaterial = clientMaterial;
                break;
        }
    }
}
