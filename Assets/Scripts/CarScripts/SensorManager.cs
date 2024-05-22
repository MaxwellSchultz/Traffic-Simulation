using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    private CarAI carAI;
    public string tagName;
    private bool active;

    void Start()
    {
        carAI = gameObject.transform.parent.GetComponent<CarAI>();
        //carAI = gameObject.GetComponent<CarAI>();
        active = true;
    }

    private void OnTriggerStay(Collider car)
    {
        if (active)
        {
            if (car.CompareTag("Barrier"))
            {
                carAI.move = false;
            }
            if (car.gameObject.transform.parent != null)
            {
                if (car.gameObject.transform.parent.CompareTag(tagName))
                {
                    carAI.move = false;
                }
            }
        }


    }

    private void OnTriggerExit(Collider car)
    {
        if (car.CompareTag("Barrier"))
        {
            carAI.move = true;
        }
        if (car.gameObject.transform.parent != null)
        {
            if (car.gameObject.transform.parent.CompareTag(tagName))
            {
                carAI.move = true;
            }
        }
    }

    public void Active(bool set)
    {
        active = set;
    }
}
