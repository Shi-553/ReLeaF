using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class ItemManager :MonoBehaviour
    {
        List<ItemUI> itemUIs = new List<ItemUI>();
        [SerializeField]
        RectTransform itemUIRoot;
        [SerializeField]
        Transform selectFrame;
        [SerializeField]
        MarkerManager seedMarkerManager;

        int itemCount = 0;
        int ItemCount
        {
            set
            {
                itemCount = value;
                if (itemCount == 0)
                {
                    selectFrame.gameObject.SetActive(false);
                }
                else
                {
                    selectFrame.gameObject.SetActive(true);
                }

                // update index
                if (itemCount == index)
                    Index--;
            }
            get => itemCount;
        }

        int index = 0;
        int Index
        {
            set
            {
                index = (ItemCount != 0) ? ((value+ ItemCount) % ItemCount) : 0;

            }
            get => index;
        }
        ItemUI Current => itemUIs[index];

        Camera mainCamera;

        public Vector2Int ItemDir { get; set; }
        public Vector2Int OldItemDir { get; private set; }
        public bool WasChangedItemDirThisFrame => ItemDir != OldItemDir;

        PlayerMover mover;
        ItemBase previewd;

        private void Start()
        {
            itemUIRoot.GetComponentsInChildren(true, itemUIs);
            mainCamera = Camera.main;

            mover=GetComponentInParent<PlayerMover>();

        }

        public void AddItem(ItemBase itemBase)
        {
            if (itemUIs.Count <= ItemCount)
            {
                return;
            }
            var item = itemUIs[ItemCount];
            item.Init(itemBase);

            var screen = mainCamera.WorldToScreenPoint(itemBase.transform.position);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(itemUIRoot, screen, mainCamera, out var local))
            {
                item.transform.localPosition= local;
            }
            item.Index = ItemCount;

            ItemCount++;
        }

        public void UseItem()
        {
            if (ItemCount == 0) 
                return;

            var useItem = Current;

            itemUIs.RemoveAt(Index);
            itemUIs.Add(useItem);

            useItem.Item.Use(mover.TilePos, ItemDir);

            useItem.Uninit();
            ItemCount--;

            for (int i = Index; i < ItemCount; i++)
            {
                itemUIs[i].Index = i;
            }
        }

        public void SelectMoveLeft()
        {
            Index--;
        }
        public void SelectMoveRight()
        {
            Index++;
        }

        private void LateUpdate()
        {
            selectFrame.transform.position = Current.transform.position;
        }
        private void Update()
        {

            if (mover.WasChangedTilePosThisFrame || WasChangedItemDirThisFrame || previewd!=Current.Item)
            {
                seedMarkerManager.ResetAllMarker();
                if (ItemCount == 0)
                {
                    return;
                }
                previewd = Current.Item;
                var previews = previewd.PreviewRange(mover.TilePos, ItemDir);

                foreach (var p in previews)
                {
                    seedMarkerManager.SetMarker<SeedMarker>(p);
                }
            }
            OldItemDir = ItemDir;
        }
    }
}
