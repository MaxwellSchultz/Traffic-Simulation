using UnityEngine;
using Mirror;



public class UserController : NetworkBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float rotationSpeed = 100f; // Speed of rotation
    public Camera playerCamera;
    private float pitch = 0f;
    private bool isRotating = false;

    void Update()
    {
        if (isLocalPlayer)
        {

            // Movement input
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate movement direction based on input
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;

            // Move the object
            playerCamera.transform.Translate(movement);

            // Check if the right mouse button is pressed down
            if (Input.GetMouseButtonDown(1))
            {
                isRotating = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                isRotating = false; // Set flag to false when right mouse button is released
            }

            // Rotate the camera if the right mouse button is held down
            if (isRotating)
            {
                // Rotation input
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Calculate rotation based on mouse movement
                float rotationAmountX = mouseX * rotationSpeed * Time.deltaTime;
                float rotationAmountY = mouseY * rotationSpeed * Time.deltaTime;

                // Rotate the object around the Y axis (horizontal rotation)
                playerCamera.transform.Rotate(Vector3.up, rotationAmountX);

                // Adjust pitch (vertical rotation around local x-axis)
                pitch -= rotationAmountY;
                pitch = Mathf.Clamp(pitch, -80f, 80f);
                playerCamera.transform.localRotation = Quaternion.Euler(pitch, transform.localRotation.eulerAngles.y, 0f);
            }
        }
    }
}
