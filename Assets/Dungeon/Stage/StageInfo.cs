using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("ステージ情報")]
    [CreateAssetMenu(menuName = "Stage/StageInfo")]
    public class StageInfo : ScriptableObject
    {
        [SerializeField, Rename("そのステージがあるシーン")]
        SceneType scene;
        public SceneType Scene => scene;

        [SerializeField, Rename("有効なボタンの画像")]
        Sprite activebutton;
        public Sprite Activebutton => activebutton;

        [SerializeField, Rename("無効なボタンの画像")]
        Sprite disableButton;
        public Sprite DisableButton => disableButton;

    }
}
