using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField]
        AudioInfo titleBGM;

        void Start()
        {
            BGMManager.Singleton.Play(titleBGM);

        }
    }
}
