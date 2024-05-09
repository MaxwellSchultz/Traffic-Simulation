using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightZones : MonoBehaviour
{
    [SerializeField]
    private TrafficLightManager tlm;
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
            tlm.Enqueue(id, other.transform.parent.gameObject);
        }
    }
}
