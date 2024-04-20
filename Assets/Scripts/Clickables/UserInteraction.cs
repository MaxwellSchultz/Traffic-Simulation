using UnityEngine;

public class UserInteraction : MonoBehaviour
{
    private IsHitReaction prevHit;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 corresponds to the left mouse button
        {
            if (prevHit != null) {
               prevHit.UnreactToHit();
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                IsHitReaction hitReaction = hit.collider.GetComponent<IsHitReaction>();
                if (hitReaction != null)
                {   
                    prevHit = hitReaction;
                    hitReaction.ReactToHit();
                }
            }
        }
    }
}
