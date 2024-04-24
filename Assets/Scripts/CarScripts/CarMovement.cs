using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : MonoBehaviour
{
    public Transform destination;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = destination.position;
    }
    void OnTriggerEnter(Collider other)
    {
        Stop();
    }

    void OnTriggerExit(Collider other)
    {
        Go(); // Simple, may require more checks when more objects are added
    }

    private void Stop()
    {
        
    }

    private void Go()
    {
    }
}
