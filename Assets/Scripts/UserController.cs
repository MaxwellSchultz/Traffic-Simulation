using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float rotationSpeed = 100f; // Speed of rotation
    private float pitch = 0f;

    void Update()
    {
        // Movement input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction based on input
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;

        // Move the object
        transform.Translate(movement);

        // Rotation input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate rotation based on mouse movement
        float rotationAmountX = mouseX * rotationSpeed * Time.deltaTime;
        float rotationAmountY = mouseY * rotationSpeed * Time.deltaTime;

        // Rotate the object around the Y axis
        transform.Rotate(Vector3.up, rotationAmountX);

        // Adjust pitch (rotation around local x-axis)
        pitch -= rotationAmountY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        transform.localRotation = Quaternion.Euler(pitch, transform.localRotation.eulerAngles.y, 0f);
    }
}
