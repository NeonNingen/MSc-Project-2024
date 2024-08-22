using UnityEngine;

// The CreateAssetMenu puts this object into Unity's menu.
[CreateAssetMenu()]
public class AgentPositionsData : ScriptableObject {
    public Vector3[] positionsAgent1;
    public Vector3[] positionsGoal1;
    public Vector3[] positionsAgent2;
    public Vector3[] positionsGoal2;
}
