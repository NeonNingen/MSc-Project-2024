using UnityEngine;

public class LockRotation : MonoBehaviour {
    void Start() {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null) {
            // Lock goals in place
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        } else {
            Debug.LogWarning("No Rigidbody found on " + gameObject.name);
        }
    }
}
