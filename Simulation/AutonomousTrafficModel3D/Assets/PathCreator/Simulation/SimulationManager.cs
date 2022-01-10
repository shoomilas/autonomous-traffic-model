using UnityEngine;

public class SimulationManager : MonoBehaviour {
    [Range(0.0f, 5f)] public float timeScaling = 1f;

    private void Start() {
        Setup();
    }

    private void OnValidate() {
        Setup();
        Debug.Log("Called");
    }

    private void Setup() {
        Time.timeScale = timeScaling;
    }
}