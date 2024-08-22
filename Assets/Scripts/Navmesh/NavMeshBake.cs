using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshBake : MonoBehaviour {
    public Transform[] agents;
    public Transform[] goals;
    
    private int agentsTouched = 0;
    private int goalsTouched = 0;

    private NavMeshSurface navMeshSurface;
    private MapPreview mapPreview;

    void Start() {
        mapPreview = GetComponentInParent<MapPreview>();

        navMeshSurface = GetComponent<NavMeshSurface>();
        if (navMeshSurface == null) Debug.LogWarning("NavMeshSurface is not assigned.");

        // Register collision detection for each agent
        foreach (Transform agent in agents) {
            if (agent != null) {
                GoalCollisionDetector detector = agent.gameObject.AddComponent<GoalCollisionDetector>();
                detector.OnGoalTouched += OnAgentTouched;
            }
        }

        // Register collision detection for each goal
        foreach (Transform goal in goals) {
            if (goal != null) {
                GoalCollisionDetector detector = goal.gameObject.AddComponent<GoalCollisionDetector>();
                detector.OnGoalTouched += OnGoalTouched;
            }
        }
    }

    private void OnAgentTouched() {
        agentsTouched++;
        if (goalsTouched >= goals.Length && agentsTouched >= agents.Length) {
            BakeNavMesh();
        }
    }

    private void OnGoalTouched() {
        goalsTouched++;
        if (goalsTouched >= goals.Length && agentsTouched >= agents.Length) {
            BakeNavMesh();
        }
    }

    private void BakeNavMesh() {
        navMeshSurface.buildHeightMesh = true;
        navMeshSurface.enabled = true;
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh baked after goals and agents touched the ground.");

        UpdateMeshCollider();
    }

    private void UpdateMeshCollider() {
        if (mapPreview != null) {
            Mesh newMesh = mapPreview.meshFilter.sharedMesh;

            GameObject previewMesh = mapPreview.meshFilter.gameObject; 
            MeshCollider meshCollider = previewMesh.GetComponent<MeshCollider>();

            if (meshCollider != null) {
                meshCollider.sharedMesh = newMesh;
            } else {
                Debug.LogWarning("No MeshCollider found on " + previewMesh.name);
            }
        } else {
            Debug.LogWarning("MapPreview reference is missing.");
        }
    }
}

// A separate class to handle collision detection for both goals and agents
public class GoalCollisionDetector : MonoBehaviour {
    public delegate void GoalTouchedAction();
    public event GoalTouchedAction OnGoalTouched;

    private void OnCollisionEnter(Collision collision) {
        // Check if the goal or agent has collided with a mesh collider (assuming terrain has a mesh collider)
        if (collision.gameObject.GetComponent<MeshCollider>() != null) {
            OnGoalTouched?.Invoke();
            Destroy(this);
        }
    }
}