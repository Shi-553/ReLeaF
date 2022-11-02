using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    [SerializeField]
    Drone drone;

    public bool IsHarvest { get; private set; }

    [SerializeField]
    DroneRange range;

    Coroutine waitHarvest;

    private void Start()
    {
        IsHarvest = false;

    }

    public void SetupRange(Transform center)
    {
        if (IsHarvest)
        {
            return;
        }
        range.BeginTarget(center);
    }
    public void Cancel()
    {
        range.EndTarget(true);
        if (waitHarvest != null)
        {
            StopCoroutine(waitHarvest);
            IsHarvest = false;
            waitHarvest = null;
            drone.GetComponent<Drone>().Cancel();
            drone.gameObject.SetActive(false);
        }
    }
    public void Harvest()
    {
        if (!range.IsTargeting)
        {
            return;
        }
        range.EndTarget(false);
        waitHarvest=StartCoroutine(WaitHarvest());

    }
    IEnumerator WaitHarvest()
    {
        IsHarvest = true;
        drone.gameObject.SetActive(true);
        if (range.TargetFruits.Count > 0)
        {
            yield return drone.GetComponent<Drone>().Harvest(range.TargetFruits);
        }

        drone.gameObject.SetActive(false);
        IsHarvest = false;
        waitHarvest = null;
    }

}
