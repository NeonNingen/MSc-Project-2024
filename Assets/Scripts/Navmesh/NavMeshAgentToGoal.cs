using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentToGoal : MonoBehaviour {
    public Transform goal;
    private NavMeshAgent agent;

    public float stoppingDistance = 0.5f;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        errorException(agent);

        agent.stoppingDistance = stoppingDistance;
    }

    void Update() {
        if (agent.isOnNavMesh && goal != null) {
            agent.SetDestination(goal.position);

            // Check if the agent has reached the goal
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
                agent.isStopped = true;
            }
        }
    }

    void errorException(NavMeshAgent agent) {
        if (agent == null) {
            Debug.LogError("NavMeshAgent component is missing on this GameObject.");
            return;
        }

        if (goal == null) {
            Debug.LogError("Goal Transform is not assigned.");
            return;
        }
    }
}
