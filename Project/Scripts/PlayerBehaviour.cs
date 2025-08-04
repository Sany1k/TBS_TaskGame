using Unity.Netcode;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Unit pickedUnit;

    private void Update()
    {
        if (!GameManager.Instance.YourTurn()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                if (hit.collider.CompareTag("Unit") && 
                    hit.collider.gameObject.GetComponent<NetworkObject>().IsOwner)
                {
                    DeselectActiveUnit();

                    pickedUnit = hit.collider.gameObject.GetComponent<Unit>();
                    pickedUnit.UnitSelected();
                }
                else
                {
                    DeselectActiveUnit();
                    pickedUnit = null;
                }
            }
            else
            {
                DeselectActiveUnit();
                pickedUnit = null;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (pickedUnit != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    if (hit.collider.CompareTag("Ground") && GameManager.Instance.HasMoves()) // Move
                    {
                        pickedUnit.GoToDestinationServerRpc(hit.point);
                        GameManager.Instance.SubtractMove();
                    }
                    if (hit.collider.CompareTag("Unit") && GameManager.Instance.HasAttacks() &&
                        hit.collider.gameObject.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId) // Enemy object
                    {
                        Unit enemyUnit = hit.collider.gameObject.GetComponent<Unit>();

                        if (Vector3.Distance(pickedUnit.transform.position, enemyUnit.transform.position) <= pickedUnit.AttackRange) // Attack
                        {
                            enemyUnit.KillUnitServerRpc();
                            GameManager.Instance.SubtractAttack();
                        }
                    }

                    DeselectActiveUnit();
                    pickedUnit = null;
                }
            }
        }
    }

    private void DeselectActiveUnit()
    {
        if (pickedUnit != null)
        {
            pickedUnit.UnitDeselected();
        }
    }
}
