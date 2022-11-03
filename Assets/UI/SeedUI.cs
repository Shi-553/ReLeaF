using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class SeedUI : MonoBehaviour
    {
        [SerializeField]
        PlantType plantType;
        public PlantType PlantType => plantType;
    }
}