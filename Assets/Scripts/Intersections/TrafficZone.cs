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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            fws.Signal(id, other.gameObject);
        }
    }
}
