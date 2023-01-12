using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class StatusChangeManager : SingletonBase<StatusChangeManager>
    {
        [SerializeField]
        Transform statusChangeParent;

        [SerializeField]
        StatusChange statusChangePrefab;

        [SerializeField]
        float marginX = 100;

        List<StatusChange> statusChanges = new();

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {

        }

        public void AddStatus(ChangeStatusinfo info)
        {
            var status = Instantiate(statusChangePrefab, statusChangeParent);
            statusChanges.Add(status);
            UpdateSutatusChangesPosition();
            status.Init(info);
        }

        public void RemoveStatus(StatusChange status)
        {
            statusChanges.Remove(status);
            Destroy(status.gameObject);
            UpdateSutatusChangesPosition();
        }

        public void UpdateSutatusChangesPosition()
        {
            for (int i = 0; i < statusChanges.Count; i++)
            {
                var statusChange = statusChanges[i];

                statusChange.transform.localPosition = new Vector3(marginX * i, 0, 0);
            }
        }
    }
}
