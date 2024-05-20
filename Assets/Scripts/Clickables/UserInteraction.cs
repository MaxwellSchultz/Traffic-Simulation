using UnityEngine;
using Mirror;

public class UserInteraction : NetworkBehaviour
{
    private Collider prevHit;
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E)) // 0 corresponds to the left mouse button
        {
            if (prevHit != null)
            {
                prevHit.GetComponent<IsHitReaction>().UnreactToHit();
                CmdRemoveAuthority(prevHit.GetComponent<NetworkIdentity>());
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                IsHitReaction hitReaction = hit.collider.GetComponent<IsHitReaction>();
                if (hitReaction != null)
                {
                    prevHit = hit.collider;
                    CmdAssignAuthority(hit.collider.GetComponent<NetworkIdentity>());
                    hitReaction.ReactToHit();
                }
            }
        }
    }
    [Command]
    void CmdAssignAuthority(NetworkIdentity targetId)
    {
        targetId.AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [Command]
    void CmdRemoveAuthority(NetworkIdentity targetId) {
        targetId.RemoveClientAuthority();
    }
}

