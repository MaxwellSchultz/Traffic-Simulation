using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    private CarAI carAI;
    public string tagName;
    private bool active;
    private float startupDelay;

    void Start()
    {
        carAI = gameObject.transform.parent.GetComponent<CarAI>();
        //carAI = gameObject.GetComponent<CarAI>();
        active = true;
        startupDelay = Random.Range(0f, 2.5f);
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
            SetActiveWithDelay(true, startupDelay);
        }
        if (car.gameObject.transform.parent != null)
        {
            if (car.gameObject.transform.parent.CompareTag(tagName))
            {
                SetActiveWithDelay(true, startupDelay);
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
        carAI.move = set;
    }

    public void SetActive(bool set)
    {
        active = set;
    }
}
