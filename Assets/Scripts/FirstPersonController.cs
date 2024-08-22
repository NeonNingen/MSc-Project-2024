using UnityEngine;

public class FirstPersonController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravity = -9.81f;
    public float lookSpeed = 2f;
    public float maxLookAngle = 85f;

    private CharacterController characterController;
    private Vector3 velocity;
    private float rotationX = 0f;
    private bool isPaused = false; // Added to handle the pause state

    void Start() {
        characterController = GetComponent<CharacterController>();

        // Check if the CharacterController component is assigned
        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found on the GameObject. Please ensure the script is attached to an object with a CharacterController.");
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        if (characterController == null) return;
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();

        // Only handle movement and mouse look if not paused
        if (!isPaused) {
            HandleMovement();
            HandleMouseLook();
            ApplyGravity();
        }
    }

    void TogglePause() {
        isPaused = !isPaused;

        if (isPaused) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleMovement()
    {
        if (characterController.isGrounded && velocity.y < 0) velocity.y = -2f;

        float moveDirectionX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveDirectionZ = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 move = transform.right * moveDirectionX + transform.forward * moveDirectionZ;

        characterController.Move(move * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && characterController.isGrounded) velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    void HandleMouseLook() {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);

        // Apply vertical rotation to the local rotation of the camera
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Apply horizontal rotation to the parent object or self if no parent
        transform.Rotate(Vector3.up * mouseX);
    }

    void ApplyGravity() {
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
