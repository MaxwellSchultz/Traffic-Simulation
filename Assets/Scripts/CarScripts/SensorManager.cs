using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    private CarAI carAI;
    public string tagName;
    private bool active;
    private float startUpDelay = 5;

    void Start()
    {
        carAI = gameObject.transform.parent.GetComponent<CarAI>();
        //carAI = gameObject.GetComponent<CarAI>();
        active = true;
        startUpDelay = Random.Range(0, 5);
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
            SetActiveWithDelay(true, startUpDelay);
        }
        if (car.gameObject.transform.parent != null)
        {
            if (car.gameObject.transform.parent.CompareTag(tagName))
            {
                SetActiveWithDelay(true, startUpDelay);
            }
        }
    }

    public void SetActiveWithDelay(bool set, float delay)
    {
        StartCoroutine(SetActiveAfterDelay(set, delay));
    }

    private IEnumerator SetActiveAfterDelay(bool set, float delay)
    {
        yield return new WaitForSeconds(delay);
        carAI.move = true;
    }

    public void Active(bool set)
    {
        active = set;
    }
}
