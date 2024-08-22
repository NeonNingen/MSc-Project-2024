using UnityEngine;
using UnityEngine.UI;

public class MovingAgentsOnChunkSize : MonoBehaviour {
    public Slider terrainSizeSlider;
    public GameObject[] agents;
    public GameObject[] goals;
    public AgentPositionsData positionsData;

    public float storedValue;
    private bool sliderWasActive;

    void Start() {
        if (positionsData == null) Debug.LogError("No AgentPositionsData assigned. Please assign a ScriptableObject in the Inspector.");

        if (terrainSizeSlider != null) {
            // Immediately synchronize storedValue with the current slider value
            storedValue = terrainSizeSlider.value;
            ApplyValueToAgentsAndGoals();
            terrainSizeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        sliderWasActive = terrainSizeSlider != null && terrainSizeSlider.gameObject.activeInHierarchy;
    }

    void OnSliderValueChanged(float value) {
        storedValue = value;
        ApplyValueToAgentsAndGoals();
    }

    void Update() {
        if (terrainSizeSlider != null) {
            if (terrainSizeSlider.gameObject.activeInHierarchy && !sliderWasActive) {
                // When the slider becomes active again, update the stored value and agents/goals positions
                terrainSizeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
                storedValue = terrainSizeSlider.value;
                ApplyValueToAgentsAndGoals();
                terrainSizeSlider.onValueChanged.AddListener(OnSliderValueChanged);

                sliderWasActive = true;
                Debug.Log("Slider reactivated, agents and goals positions updated to: " + storedValue);
            }
            else if (!terrainSizeSlider.gameObject.activeInHierarchy && sliderWasActive) {
                // If the slider was active but is now inactive, unsubscribe from the event
                terrainSizeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
                sliderWasActive = false;
                Debug.Log("Slider deactivated, listener removed");
            }
        }
    }

    void ApplyValueToAgentsAndGoals() {
        if (positionsData == null) Debug.LogError("No AgentPositionsData assigned. Cannot update positions.");
        int index = Mathf.Clamp((int)storedValue, 0, positionsData.positionsAgent1.Length - 1);

        // Update each agent's position based on the stored value and corresponding position
        if (agents.Length >= 2 && goals.Length >= 2) {
            agents[0].transform.position = positionsData.positionsAgent1[index];
            goals[0].transform.position = positionsData.positionsGoal1[index];

            agents[1].transform.position = positionsData.positionsAgent2[index];
            goals[1].transform.position = positionsData.positionsGoal2[index];
        }

        Debug.Log("Agents' and Goals' positions updated to index: " + index);
    }
}
