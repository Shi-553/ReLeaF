using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class ItemManager : MonoBehaviour
    {
        List<ItemUI> itemUIs = new List<ItemUI>();

        RectTransform ItemUIRoot => ItemSelectorUI.Singleton.ItemsRoot;
        Transform Selector => ItemSelectorUI.Singleton.Selector;

        [SerializeField]
        MarkerManager specialPreviewMarkerManager;

        [SerializeField]
        AudioInfo seGetItem;

        [SerializeField]
        AudioInfo seUseItem;

        int itemCount = 0;
        int ItemCount
        {
            set
            {
                itemCount = value;
                if (itemCount == 0)
                {
                    Selector.gameObject.SetActive(false);
                }
                else
                {
                    Selector.gameObject.SetActive(true);
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
                index = (ItemCount != 0) ? ((value + ItemCount) % ItemCount) : 0;

            }
            get => index;
        }
        ItemUI Current => itemUIs[index];

        Camera mainCamera;

        public Vector2Int ItemDir { get; set; }
        public Vector2Int OldItemDir { get; private set; }
        public bool WasChangedItemDirThisFrame => ItemDir != OldItemDir;

        PlayerMover mover;
        ItemUI previewd;

        Vector3 itemOffset;

        List<Vector2Int> previews = new();

        Coroutine useCo;

        bool IsUseingNow => useCo != null;

        private void Start()
        {
            ItemUIRoot.GetComponentsInChildren(true, itemUIs);
            itemOffset = itemUIs[1].transform.localPosition - itemUIs[0].transform.localPosition;
            mainCamera = Camera.main;

            mover = GetComponentInParent<PlayerMover>();
            ItemCount = 0;


            foreach (var item in itemUIs)
            {
                item.Offset = itemOffset;
            }
        }

        public bool AddItem(ItemBase itemBase)
        {
            if (itemUIs.Count <= ItemCount)
            {
                return false;
            }
            var item = itemUIs[ItemCount];
            item.Init(itemBase);

            var screen = mainCamera.WorldToScreenPoint(itemBase.transform.position);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(ItemUIRoot, screen, mainCamera, out var local))
            {
                item.transform.localPosition = local;
            }
            item.Index = ItemCount;

            SEManager.Singleton.Play(seGetItem);
            ItemCount++;
            return true;
        }

        public IEnumerator UseItem()
        {
            if (GameRuleManager.Singleton.IsPrepare)
                yield break;
            if (ItemCount == 0)
                yield break;

            if (IsUseingNow)
            {
                Current.Item.UseCount++;
                yield break;
            }

            var useItem = Current;


            useCo = StartCoroutine(useItem.Item.Use(mover.TilePos, ItemDir));

            SEManager.Singleton.Play(seUseItem);


            yield return useCo;

            itemUIs.RemoveAt(Index);
            itemUIs.Add(useItem);

            useItem.Uninit();
            ItemCount--;

            for (int i = Index; i < ItemCount; i++)
            {
                itemUIs[i].Index = i;
            }

            useCo = null;
        }

        public void SelectMoveLeft()
        {
            if (IsUseingNow)
            {
                return;
            }
            Index--;
        }
        public void SelectMoveRight()
        {
            if (IsUseingNow)
            {
                return;
            }
            Index++;
        }

        private void Update()
        {
            Selector.transform.position = Current.transform.position;

            if (mover.WasChangedTilePosThisFrame || WasChangedItemDirThisFrame || previewd != Current)
            {
                specialPreviewMarkerManager.ResetAllMarker();
                previewd = Current;
                if (ItemCount == 0 || previewd == null || previewd.Item == null)
                {
                    return;
                }

                previewd.Item.PreviewRange(mover.TilePos, ItemDir, previews);

                foreach (var p in previews)
                {
                    specialPreviewMarkerManager.SetMarker<SpecialPreviewMarker>(p);
                }
            }

            OldItemDir = ItemDir;
        }

        private void OnDestroy()
        {
            specialPreviewMarkerManager.ResetAllMarker();
        }
    }
}
