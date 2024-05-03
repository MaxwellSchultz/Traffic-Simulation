using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locomotive_script : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;

    public float TurningSpeed = 1f;
    public GameObject destinationSensor;
    GameObject nextDestination;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        nextDestination = destinationSensor.GetComponent<sensor_script>().detectedDestination;
        if (nextDestination != null)
        {
            Vector3 lookPos = nextDestination.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * TurningSpeed);
            // Below line of code is responsible for moving locomotive
            rb.velocity = transform.forward * speed ;

        }

        // Vector3 lookPos = nextDestination.transform.position - transform.position;
        // lookPos.y = 0;
        // Quaternion rotation = Quaternion.LookRotation(lookPos);
        // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * TurningSpeed);

        // // Below line of code is responsible for moving locomotive
        // rb.velocity = transform.forward * speed ;

        // Vector3 tempVect = new Vector3(0,0,-1);
        // tempVect = tempVect.normalized * speed * Time.deltaTime;
        // rb.MovePosition(transform.position + tempVect);
    }
}




























// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class locomotive_script : MonoBehaviour
// {
//     public float speed;
//     public GameObject destination;
//     private Rigidbody rb;
//     public float TurningSpeed = 1f;
//     // Start is called before the first frame update
//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         Vector3 lookPos = destination.transform.position - transform.position;
//         lookPos.y = 0;
//         Quaternion rotation = Quaternion.LookRotation(lookPos);
//         transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * TurningSpeed);

//         // Below line of code is responsible for moving locomotive
//         rb.velocity = transform.forward * speed ;
//     }
// }
