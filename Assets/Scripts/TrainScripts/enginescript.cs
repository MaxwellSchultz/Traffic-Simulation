using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Engine : MonoBehaviour
{
    public Camera mainCamera;  // Assign the main camera in the Inspector
    public Camera followCamera;  // Assign the follow camera in the Inspector

    private Rigidbody rb;

    [SerializeField] private float power;
    [SerializeField] private float damping = 0.99f;

    private float currentSpeed = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera.gameObject.SetActive(true);
        followCamera.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchCamera();
        }

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
    void SwitchCamera()
    {
        bool isMainCameraActive = mainCamera.gameObject.activeSelf;
        
        // Toggle camera activation
        mainCamera.gameObject.SetActive(!isMainCameraActive);
        followCamera.gameObject.SetActive(isMainCameraActive);
    }
}
