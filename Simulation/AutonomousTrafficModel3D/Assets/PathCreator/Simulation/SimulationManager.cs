using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {
    [Range(0.0f, 5f)] public float timeScaling = 1f;

    void Setup() {
        Time.timeScale = timeScaling;
    }
    void Start()
    {
        Setup();
    }

    private void OnValidate() {
        Setup();
        Debug.Log("Called");
    }
}
