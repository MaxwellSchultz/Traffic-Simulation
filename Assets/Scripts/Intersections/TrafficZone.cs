using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficZone : MonoBehaviour
{
    [SerializeField]
    private FourWayStopSign fws;
    [SerializeField]
    private int id;
    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            int intent;
            float turn = other.GetComponentInParent<CarAI>().WillTurn();
            if (turn > 0)
            {
                intent = 0;
            } else if (turn < 0)
            {
                intent = 2;
            } else
            {
                intent = 1;
            }
            fws.SignalIntent(id, intent, other.transform.parent.gameObject);
        }
    }
}
