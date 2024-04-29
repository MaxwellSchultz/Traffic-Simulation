using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sink : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("StoppingBox")) {
            if (other.gameObject.transform.parent.CompareTag("Car"))
            {
                Destroy(other.gameObject.transform.parent.gameObject);
            }
        }
    }
}
