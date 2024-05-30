using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficZone : MonoBehaviour
{
    [SerializeField]
    private Intersection fws;
    [SerializeField]
    private int id;

    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            int intent;
            float turn = other.GetComponentInParent<CarAI>().WillTurn();
            other.gameObject.transform.parent.Find("StopBox").gameObject.SetActive(false);
            if (turn > 0)
            {
                if (turn == 720)
                {
                    intent = 3;
                }else
                {
                    intent = 0;
                }
            } else if (turn < 0)
            {
                intent = 2;
            } else
            {
                intent = 1;
            }
            fws.SignalIntent(id, intent, other.transform.parent.gameObject);
        }
        if(other.CompareTag("StoppingBox"))
        {
            other.gameObject.GetComponentInParent<CarAI>().ForceSlowSpeed();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StoppingBox"))
        {
            other.gameObject.GetComponentInParent<CarAI>().ReleaseSlowSpeed();
        }
    }
}
