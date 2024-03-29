using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    [ClassSummary("アイテムのUI")]
    public class ItemUI : MonoBehaviour
    {
        public ItemBase Item { get; private set; }
        [SerializeField]
        Image image;

        [SerializeField, Rename("正しい位置へ到達するまでのおおよその時間")]
        float smoothTime = 1.0f;
        [SerializeField, Rename("最大速度")]
        float maxSpeed = 1.0f;

        private Vector3 velocity = Vector3.zero;
        public bool IsValid => Item != null;

        public int Index { get; set; }
        public Vector3 Offset { get; set; }

        public void Init(ItemBase item)
        {
            Item = item;
            image.sprite = item.Icon;
            image.useSpriteMesh = true;
            gameObject.SetActive(true);
            transform.localPosition = GetItemLocalPos();
        }

        public void Uninit()
        {

            Item = null;
            image.sprite = null;
            gameObject.SetActive(false);
        }

        void Update()
        {
            var targetPos = GetItemLocalPos();
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, smoothTime, maxSpeed);

        }

        public Vector3 GetItemLocalPos() => Offset * Index;
    }
}
