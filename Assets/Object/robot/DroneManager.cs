using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    [SerializeField]
    Drone dronePrefab;

    [SerializeField]
    DroneRoute droneRoute;

    Coroutine droneCo;
    [SerializeField]
    CinemachineBrain brain;
    public bool IsSowRouting { get; private set; }
    public void BeginSowRoute(Vector3 startPos)
    {
        transform.position = startPos;
        IsSowRouting = true;
        droneRoute.transform.localPosition = Vector3.zero;
        droneRoute.Begin();

        Time.timeScale = 0.1f;

        brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        brain.m_IgnoreTimeScale = true;
    }
    public void MoveSowRoute(Vector2Int dir)
    {
        droneRoute.SetDir(dir);
    }
    public void EndSowRoute()
    {
        brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
        brain.m_IgnoreTimeScale = false;
        Time.timeScale = 1;

        droneRoute.End();
        IsSowRouting = false;

        var drone=Instantiate(dronePrefab, transform.position, Quaternion.identity, transform);
        drone.transform.localPosition = Vector3.zero;
        droneCo = StartCoroutine(drone.SowSeed(new List<Foundation>(droneRoute.LastTargets), PlantType.Tree));
    }
    public void Cancel()
    {
        if (droneCo != null)
        {
            StopCoroutine(droneCo);
            droneCo = null;
        }
    }
}
