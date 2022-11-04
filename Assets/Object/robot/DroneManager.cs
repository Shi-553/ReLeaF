using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class DroneManager : MonoBehaviour
    {
        [SerializeField]
        Drone dronePrefab;

        [SerializeField]
        DroneRoute droneRoute;

        [SerializeField]
        CinemachineBrain brain;
        [SerializeField]
        SelectSeed selectSeed;
        public bool IsSowRouting => droneRoute.IsRouting;

        [SerializeField]
        Transform droneRoot;

        Vector3 startPos;


        public static DroneManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public IEnumerator BeginSowRoute(Vector3 startPos)
        {
            this.startPos = startPos;
            droneRoute.transform.position = startPos;
            Time.timeScale = 0.1f;

            brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            brain.m_IgnoreTimeScale = true;

            yield return StartCoroutine(droneRoute.Begin());

            EndSowRoute();
        }
        public void MoveSowRoute(Vector2Int dir)
        {
            droneRoute.SetDir(dir);
        }
        public void EndSowRoute(bool isCollect = true)
        {
            if (Time.timeScale == 1)
            {
                return;
            }

            brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
            brain.m_IgnoreTimeScale = false;
            Time.timeScale = 1;

            droneRoute.End(!isCollect);

            if (isCollect)
            {
                var drone = Instantiate(dronePrefab, transform.position, Quaternion.identity, droneRoot);
                drone.transform.position = startPos;
                drone.SowSeed(new List<Foundation>(droneRoute.LastTargets), selectSeed.CurrentSeed.PlantType);
            }
        }
        public void Cancel()
        {
            foreach (Transform t in droneRoot)
            {
                t.GetComponent<Drone>().Stop();
            }
            if (IsSowRouting)
            {
                EndSowRoute(false);
            }
        }
    }
}