using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationTime : MonoBehaviour
{
    [Range(0.0f, 75.0f)] [SerializeField]private float simulationTime;
    void Update()
    {
        Time.timeScale = simulationTime;
    }
}
