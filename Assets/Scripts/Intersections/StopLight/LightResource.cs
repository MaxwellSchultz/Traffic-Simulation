using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightResource : MonoBehaviour
{
    [SerializeField]
    int id;
    [SerializeField]
    StopLightManager lightManager;
    List<GameObject>[] lanes = Enumerable.Range(1, 5).Select(i => new List<GameObject>()).ToArray();
    // Start is called before the first frame update
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            for (int i = 0; i < lanes.Length; i++)
            {
                if (lanes[i].Contains(other.gameObject))
                {
                    lightManager.ReturnPartialTurn(i);
                    lanes[i].Remove(other.gameObject);
                }
            }

        }
    }
    public void AssignCar(GameObject car,int id)
    {
        lanes[id].Add(car);
    }
}
