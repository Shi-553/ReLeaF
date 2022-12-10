using UnityEngine;
using Utility;


namespace ReLeaf
{
    public class TitleManager : SingletonBase<TitleManager>
    {
        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        AudioInfo titleBGM;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }

        void Start()
        {
            BGMManager.Singleton.Play(titleBGM);
        }
    }
}
