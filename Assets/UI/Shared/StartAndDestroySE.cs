using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class StartAndDestroySE : MonoBehaviour
    {
        [SerializeField]
        AudioInfo seStart;
        [SerializeField]
        AudioInfo seDestroy;
        void Start()
        {
            SEManager.Singleton.Play(seStart);
        }

        void OnDestroy()
        {
            SEManager.Singleton.Play(seDestroy);
        }
    }
}
