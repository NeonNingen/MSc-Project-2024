using UnityEngine;

public class Draggable : MonoBehaviour {
    private Camera mainCamera;
    private float initialYPosition;
    private Vector3 offset;
    private Rigidbody rb;

    public float upwardMovementSpeed = 2f;

    void Start() {
        mainCamera = Camera.main;
        initialYPosition = transform.position.y;

        rb = GetComponent<Rigidbody>();

        if (rb == null)  Debug.LogError("No Rigidbody component found on this GameObject.");
    }

    void OnMouseDown() {
        if (rb != null) {
            rb.useGravity = false;
        }

        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag() {
        Vector3 newPosition = GetMouseWorldPos() + offset;
        newPosition.y += upwardMovementSpeed * Time.deltaTime;

        // Restrict the Y position to not go below the initial Y position
        if (newPosition.y < initialYPosition) {
            newPosition.y = initialYPosition;
        }

        transform.position = newPosition;
    }

    void OnMouseUp() {
        if (rb != null) {
            rb.useGravity = true;
        }
    }

    private Vector3 GetMouseWorldPos() {
        Vector3 mousePoint = Input.mousePosition;

        // Set the Z distance so the object stays at the same depth from the camera
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
