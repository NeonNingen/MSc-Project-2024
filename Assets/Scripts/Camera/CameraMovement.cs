using UnityEngine;

public class CameraController : MonoBehaviour {
    // Speed of camera movement
    public float moveSpeed = 20f;
    public float zoomSpeed = 20f;
    public float minZoomDistance = 20f;
    public float maxZoomDistance = 200f;
    public Vector2 panLimit = new Vector2(50f, 50f);

    private Vector3 initialPosition;

    void Start() {
        // Save the initial position to enforce zoom limits
        initialPosition = transform.position;
    }

    void Update() {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement() {
        Vector3 pos = transform.position;

        // Move the camera forward and backward using W and S keys
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow)) pos.z += moveSpeed * Time.deltaTime;
        if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow)) pos.z -= moveSpeed * Time.deltaTime;

        // Move the camera left and right using A and D keys
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow)) pos.x -= moveSpeed * Time.deltaTime;
        if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow)) pos.x += moveSpeed * Time.deltaTime;

        // Limit the camera's movement within a certain range
        pos.x = Mathf.Clamp(pos.x, initialPosition.x - panLimit.x, initialPosition.x + panLimit.x);
        pos.z = Mathf.Clamp(pos.z, initialPosition.z - panLimit.y, initialPosition.z + panLimit.y);

        transform.position = pos;
    }

    private void HandleZoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 direction = transform.forward;

        // Move the camera along its forward axis (zoom in or out)
        transform.position += direction * scroll * zoomSpeed;

        // Ensure the camera stays within the zoom distance limits
        float distanceFromInitial = Vector3.Distance(transform.position, initialPosition);
        if (distanceFromInitial < minZoomDistance) {
            transform.position = initialPosition + direction * minZoomDistance;
        }
        else if (distanceFromInitial > maxZoomDistance) {
            transform.position = initialPosition + direction * maxZoomDistance;
        }
    }
}
