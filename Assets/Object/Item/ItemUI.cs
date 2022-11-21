using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    [ClassSummary("アイテムのUI")]
    public class ItemUI : MonoBehaviour
    {
        public ItemBase Item { get; private set; }
        [SerializeField]
        Image image;

        [SerializeField,Rename("正しい位置へ到達するまでのおおよその時間")]
        float smoothTime =1.0f;
        [SerializeField,Rename("最大速度")]
        float maxSpeed =1.0f;

        private Vector3 velocity = Vector3.zero;
        public bool IsValid => Item != null;

        public int Index { get; set; }

        public void Init(ItemBase item)
        {
            Item = item;
            image.sprite = item.Icon;
            gameObject.SetActive(true);
        }

        public void Uninit()
        {
            Destroy(Item.gameObject);

            Item= null;
            image.sprite=null;
            gameObject.SetActive(false);
        }

         void Update()
        {
            var targetPos = GetItemLocalPos(Index);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity,smoothTime,maxSpeed);

        }

        Vector3 GetItemLocalPos(int i) => new Vector3(180 * i, 0, 0);
    }
}
