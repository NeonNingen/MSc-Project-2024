using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ResetNavMeshSettingsUI : MonoBehaviour {
    public Transform aiParentObj;
    public Button resetButton;
    public AgentPositionsData positionsData;
    public MovingAgentsOnChunkSize movingAgentsScript;

    void Start() {
        if (resetButton != null) resetButton.onClick.AddListener(ResetAll);
    }

    public void ResetAll() {
        if (positionsData == null) Debug.LogError("No AgentPositionsData assigned. Please assign a ScriptableObject in the Inspector.");
        if (movingAgentsScript == null) Debug.LogError("No MovingAgentsOnChunkSize script assigned. Please assign it in the Inspector.");

        int currentIndex = Mathf.Clamp((int)movingAgentsScript.storedValue, 0, positionsData.positionsAgent1.Length - 1);

        // Reset positions of all children based on the current chunk size index
        ResetPositionsFromData(currentIndex);

        // Restart NavMeshAgents
        RestartNavMeshAgents();

        // Reset all scripts attached to the children
        for (int i = 0; i < aiParentObj.childCount; i++) {
            MonoBehaviour[] childScripts = aiParentObj.GetChild(i).GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour script in childScripts) {
                script.enabled = false;
                script.enabled = true;
            }
        }

        // Reset NavMeshBakeScript if it exists
        NavMeshBake navmeshBakeScript = GetComponent<NavMeshBake>();
        if (navmeshBakeScript != null) {
            navmeshBakeScript.enabled = false;
            navmeshBakeScript.enabled = true;
        }

        Debug.Log("AI positions, scripts, and NavMesh have been reset.");
    }

    private void ResetPositionsFromData(int index) {
        if (aiParentObj.childCount >= 4) {
            aiParentObj.GetChild(0).position = positionsData.positionsAgent1[index];
            aiParentObj.GetChild(1).position = positionsData.positionsGoal1[index];
            aiParentObj.GetChild(2).position = positionsData.positionsAgent2[index];
            aiParentObj.GetChild(3).position = positionsData.positionsGoal2[index];
        } else {
            Debug.LogWarning("aiParentObj does not contain the expected number of children.");
        }
    }

    private void RestartNavMeshAgents() {
        // Restart each NavMeshAgent to ensure they start moving towards their goal again
        for (int i = 0; i < aiParentObj.childCount; i++) {
            NavMeshAgent agent = aiParentObj.GetChild(i).GetComponent<NavMeshAgent>();
            NavMeshAgentToGoal agentToGoal = aiParentObj.GetChild(i).GetComponent<NavMeshAgentToGoal>();

            if (agent != null && agentToGoal != null) {
                agent.isStopped = false;
                agent.SetDestination(agentToGoal.goal.position);
            }
        }
    }
}
