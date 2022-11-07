using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class Foundation : Plant
    {
        [SerializeField]
        GameObject highLightObj;

        public bool IsHighlighting => highLightObj.activeSelf;

        public PlantType SowScheduledPlantType { private set; get; }
        public bool IsSowScheduled => SowScheduledPlantType != PlantType.None;

        private void Start()
        {
            Init();
            SowScheduledPlantType = PlantType.None;
        }


        public void SetSowSchedule(PlantType type)
        {
            SowScheduledPlantType=type;
            highLightObj.SetActive(true);
        }
        public void ReSetSowSchedule()
        {
            SowScheduledPlantType = PlantType.None;
            highLightObj.SetActive(false);
        }


    }
}