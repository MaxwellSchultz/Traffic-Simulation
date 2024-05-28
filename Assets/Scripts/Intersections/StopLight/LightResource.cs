using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightResource : MonoBehaviour
{
    [SerializeField]
    int id;
    [SerializeField]
    StopLightManager lightManager;
    List<GameObject> queue = new List<GameObject>();
    // Start is called before the first frame update
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car") && queue.Contains(other.gameObject))
        {
            lightManager.ReturnPartialTurn(id);
            queue.Remove(other.gameObject);
        }
    }
    public void AssignCar(GameObject car)
    {
        queue.Add(car);
    }
}
