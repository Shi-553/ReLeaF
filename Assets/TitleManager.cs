using System.Collections;
using UnityEngine;
using Utility;


namespace ReLeaf
{
    public class TitleManager : SingletonBase<TitleManager>
    {
        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        AudioInfo titleBGM;

        protected override void Init()
        {
            
        }
        
        void Start()
        {
            BGMManager.Singleton.Play(titleBGM);
        }
    }
}
