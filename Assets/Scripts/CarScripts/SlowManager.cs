using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlowManager : MonoBehaviour
{
    // Component References
    private Rigidbody rb;
    [SerializeField]
    private Collider stopBox;
    
    [SerializeField]
    private float stoppingDist = 5f; // Depends on car (Maybe find a way to get by weight?
    [SerializeField]
    private float minStopping = 1f; // Change this to lane width

    // Holder Variables
    private Vector3 velocity;
    private Vector3 forward;
    private float fComponent;
    private float colliderStartPos;
    private Vector3 oldScale;
    private Vector3 newScale;
    private Vector3 newPos;
    private Vector3 oldPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.transform.parent.GetComponent<Rigidbody>();
        colliderStartPos = stopBox.transform.localPosition.z; // Where we set it
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        AdjustScale();
    }

    void AdjustScale()
    {
        // Get velocity
        velocity = rb.velocity;
        forward = rb.transform.forward;

        // Get Dot product
        fComponent = Vector3.Dot(velocity, forward);
        fComponent *= stoppingDist;
        // Stopping distance equals lesser of
        // Lane width
        // Stopping distance * speed component
        fComponent = Math.Max(minStopping, fComponent);
        // Adjust size of collider
        oldScale = stopBox.transform.localScale;
        oldPos = stopBox.transform.localPosition;
        newScale = new Vector3(oldScale.x, oldScale.y, fComponent);
        newPos = new Vector3(oldPos.x, oldPos.y, colliderStartPos + (fComponent * .5f));
        stopBox.transform.localPosition = newPos;
        stopBox.transform.localScale = newScale;
    }
}
