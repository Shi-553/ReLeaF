using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Stage/StageInfo")]
    public class StageInfo : ScriptableObject
    {
        [SerializeField]
        SceneType scene;
        public SceneType Scene => scene;

        [SerializeField]
        string stageName = "";
        public string StageName => stageName;
    }
}
