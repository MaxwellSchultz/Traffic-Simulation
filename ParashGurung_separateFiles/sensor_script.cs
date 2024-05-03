using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensor_script : MonoBehaviour
{
    public GameObject detectedDestination;
    // Start is called before the first frame update
    void Start()
    {
        detectedDestination = null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TrainDestination"){
            detectedDestination = other.gameObject;
            Debug.Log("Destination Point Detected");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TrainDestination"){
            Debug.Log("Destination Point UnDetected");
            // detectedDestination = null;
        }
    }
}
