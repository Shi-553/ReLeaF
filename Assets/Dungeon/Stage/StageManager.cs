using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class StageManager : SingletonBase<StageManager>
    {
        public override bool DontDestroyOnLoad => true;


        [field: SerializeField, Rename("今のステージ")]
        public StageInfo Current { get; set; }
        protected override void Init()
        {
        }

        void Start()
        {

        }

    }
}
