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
                prevHit = null;
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                IsHitReaction hitReaction = hit.collider.GetComponent<IsHitReaction>();
                if (hitReaction != null)
                {
                    NetworkIdentity hitIdentity = hit.collider.GetComponent<NetworkIdentity>();
                    if (hitIdentity != null)
                    {
                        // Check if the object does not have previous ownership
                        // if (hitIdentity.clientAuthorityOwner == null)
                        // {
                            prevHit = hit.collider;
                            CmdAssignAuthority(hit.collider.GetComponent<NetworkIdentity>());
                            hitReaction.ReactToHit();
                        // }
                    }
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
    void CmdRemoveAuthority(NetworkIdentity targetId)
    {
        targetId.RemoveClientAuthority();
    }
}

