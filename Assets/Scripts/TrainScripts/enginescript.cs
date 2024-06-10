using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Engine : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float power;
    [SerializeField] private float damping = 0.99f;

    private float currentSpeed = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("working w");
            Throttle(power);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("working s");
            Throttle(-power);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed *= damping;
            rb.velocity = currentSpeed * transform.forward;
        }
    }

    private void Throttle(float power)
    {
        currentSpeed += power * Time.deltaTime;
        Vector3 dir = currentSpeed * transform.forward;
        rb.velocity = dir;
    }
}
