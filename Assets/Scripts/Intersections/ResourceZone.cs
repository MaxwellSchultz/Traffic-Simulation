using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceZone : MonoBehaviour
{
    [SerializeField]
    int id;
    [SerializeField]
    IntersectionResourceManager resourceManager;
    // Start is called before the first frame update
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        { resourceManager.ReturnPartialTurn(id); }
    }
}
