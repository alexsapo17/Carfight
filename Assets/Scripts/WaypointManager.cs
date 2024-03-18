using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class WaypointPath
{
    public List<Transform> waypoints;
}
public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    public List<WaypointPath> waypointPaths; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
