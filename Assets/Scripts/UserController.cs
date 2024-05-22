using UnityEngine;
using UnityEngine.UI;
using Mirror;



public class UserController : NetworkBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float rotationSpeed = 100f; // Speed of rotation
    private Transform playerCamera;
    public Transform cameraTarget;
    public GameObject myUIPrefab;
    private GameObject myUI;
    private GameObject myCanvas;
    private float pitch = 0f;
    private bool isRotating = false;

    public override void OnStartLocalPlayer()
    {
        playerCamera = Camera.main.transform;
        playerCamera.position = cameraTarget.transform.position;
        playerCamera.rotation = cameraTarget.transform.rotation;
        playerCamera.SetParent(this.transform);

        myCanvas = GameObject.Find("SceneCanvas");
        myUI = Instantiate(myUIPrefab);
        myUI.transform.SetParent(myCanvas.transform, false);

        Transform speedSliderTransform = myUI.transform.Find("SimSpeedSlider");
        Transform sensSliderTransform = myUI.transform.Find("SensitivitySlider");
        Slider speedSlider = speedSliderTransform.GetComponent<Slider>();
        Slider sensSlider = sensSliderTransform.GetComponent<Slider>();

        // speedSlider.onValueChanged.AddListener(CmdSliderValueChanged);
        // sensSlider.onValueChanged.AddListener(CmdOnToggle);
    }
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
            this.transform.Translate(movement);

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
                this.transform.Rotate(Vector3.up, rotationAmountX);

                // Adjust pitch (vertical rotation around local x-axis)
                pitch -= rotationAmountY;
                pitch = Mathf.Clamp(pitch, -80f, 80f);
                this.transform.localRotation = Quaternion.Euler(pitch, transform.localRotation.eulerAngles.y, 0f);
            }
        }
    }
}
