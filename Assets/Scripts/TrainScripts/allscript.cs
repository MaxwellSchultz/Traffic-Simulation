using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class RailCart : MonoBehaviour
{
    [SerializeField] private SplineContainer rail;

    private Spline currentSpline;

    private Rigidbody rb;

    public void HitJunction(Spline rail)
    {
        currentSpline = rail;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rail != null && rail.Splines.Count > 0)
        {
            currentSpline = rail.Splines[0];
        }
        else
        {
            Debug.LogError("Rail spline is not set or empty.");
        }
    }

    private void FixedUpdate()
    {
        if (currentSpline == null)
        {
            Debug.LogError("Current spline is not set.");
            return;
        }

        var native = new NativeSpline(currentSpline);
        float distance = SplineUtility.GetNearestPoint(native, transform.position, out float3 nearest, out float t);

        // Update the cart's position
        transform.position = nearest;

        // Calculate forward and up vectors
        Vector3 forward = Vector3.Normalize(native.EvaluateTangent(t));
        Vector3 up = native.EvaluateUpVector(t);

        // Axis remap for proper orientation
        var remappedForward = new Vector3(0, 0, 1);
        var remappedUp = new Vector3(0, 1, 0);
        var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

        // Update the cart's rotation
        transform.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;

        // Adjust the velocity direction
        Vector3 engineForward = transform.forward;
        if (Vector3.Dot(rb.velocity, transform.forward) < 0)
        {
            engineForward *= -1;
        }
        rb.velocity = rb.velocity.magnitude * engineForward;
    }
}
