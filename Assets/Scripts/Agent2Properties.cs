using UnityEngine;

public class Agent2Properties : MonoBehaviour {
    private Rigidbody rb;

    public delegate void AgentTouchedGroundAction();
    public event AgentTouchedGroundAction OnAgentTouchedGround;

    void Start() {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnCollisionEnter(Collision collision) {
        // Check if the agent has collided with the ground
        if (collision.gameObject.GetComponent<MeshCollider>() != null) {
            Destroy(rb);
            OnAgentTouchedGround?.Invoke();
            this.enabled = false;
        }
    }
}
