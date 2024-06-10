using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterNavigationController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 720f;
    public float stopDistance = 0.5f;

    private Vector3 destination;
    public bool reachedDestination { get; private set; } // Changed to public with private setter
    private Vector3 lastPosition;
    private Vector3 velocity;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != destination)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;
            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance)
            {
                reachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                reachedDestination = true;
            }

            velocity = (transform.position - lastPosition) / Time.deltaTime;
            velocity.y = 0;
            var velocityMagnitude = velocity.magnitude;
            velocity = velocity.normalized;
            var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
            var rightDotProduct = Vector3.Dot(transform.right, velocity);

            _animator.SetFloat("Horizontal", rightDotProduct);
            _animator.SetFloat("Forward", fwdDotProduct);
        }
        else 
        {
            reachedDestination = true;
        }

        lastPosition = transform.position;
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }
}
