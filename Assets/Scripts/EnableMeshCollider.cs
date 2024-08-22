using UnityEngine;

public class EnableMeshCollider: MonoBehaviour {
    void Start() {
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        if (meshCollider != null) {
            meshCollider.enabled = true;
        } else {
            Debug.LogWarning("No MeshCollider found on " + gameObject.name);
        }
    }
}
