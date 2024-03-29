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


        public bool CanUse { get; set; } = true;
        public bool CanThrow { get; set; } = true;

        int itemCount = 0;
        public int ItemCount
        {
            private set
            {
                itemCount = value;


                if (itemCount == 0)
                {
                    Selector.gameObject.SetActive(false);
                }
                else
                {
                    // update index
                    if (itemCount == index)
                        index--;
                    Selector.gameObject.SetActive(true);
                    Selector.transform.localPosition = Current.GetItemLocalPos();
                }


                UpdateDescription();
            }
            get => itemCount;
        }

        int index = 0;
        public int Index
        {
            private set
            {
                index = (ItemCount != 0) ? ((value + ItemCount) % ItemCount) : 0;
                UpdateDescription();
                Selector.transform.localPosition = Current.GetItemLocalPos();
            }
            get => index;
        }
        void UpdateDescription()
        {
            if (itemCount == 0)
            {
                ItemDescription.Singleton.ResetItemDescription();
            }
            else
            {
                var description = Current.Item.ItemBaseInfo.Description;
                ItemDescription.Singleton.SetItemDescription(description, Current.GetItemLocalPos());
            }
        }
        ItemUI Current => itemUIs[index];

        public Vector2Int ItemDir { get; set; } = Vector2Int.up;
        public Vector2Int OldItemDir { get; private set; }
        public bool WasChangedItemDirThisFrame => ItemDir != OldItemDir;

        PlayerMover mover;
        ItemUI previewd;

        Vector3 itemOffset;

        HashSet<Vector2Int> previews = new();

        Coroutine useCo;

        bool IsUseingNow => useCo != null;

        private void Start()
        {
            ItemUIRoot.GetComponentsInChildren(true, itemUIs);
            itemOffset = itemUIs[1].transform.localPosition - itemUIs[0].transform.localPosition;

            mover = GetComponentInParent<PlayerMover>();
            ItemCount = 0;


            foreach (var item in itemUIs)
            {
                item.Offset = itemOffset;
            }
        }

        public bool AddItem(ItemBase itemBase)
        {
            if (itemBase.ItemBaseInfo.IsImmediate)
            {
                StartCoroutine(itemBase.Use(mover.TilePos, ItemDir));
                return true;
            }
            if (itemUIs.Count <= ItemCount)
            {
                return false;
            }

            var item = itemUIs[ItemCount];
            item.Index = ItemCount;
            item.Init(itemBase);


            SEManager.Singleton.Play(seGetItem);
            ItemCount++;
            return true;
        }

        public void ThrowItem()
        {
            if (!CanThrow)
                return;
            if (GameRuleManager.Singleton.IsPrepare)
                return;
            if (ItemCount == 0)
                return;
            var now = Time.time;

            if (now - beforeUseTime < 0.3f)
                return;

            beforeUseTime = now;

            var throwItem = Current;

            throwItem.Item.transform.position = PlayerMover.Singleton.transform.position;
            throwItem.Item.gameObject.SetActive(true);
            throwItem.Item.ReStart();
            StartCoroutine(throwItem.Item.WaitCollisionDisable());

            itemUIs.RemoveAt(Index);
            itemUIs.Add(throwItem);

            throwItem.Uninit();

            for (int i = Index; i < ItemCount; i++)
            {
                itemUIs[i].Index = i;
            }
            ItemCount--;
        }
        float beforeUseTime = -1;

        public IEnumerator UseItem()
        {
            if (!CanUse)
                yield break;
            if (GameRuleManager.Singleton.IsPrepare)
                yield break;
            if (ItemCount == 0)
                yield break;
            var now = Time.time;

            if (now - beforeUseTime < 0.3f)
                yield break;

            beforeUseTime = now;

            var useItem = Current;

            if (IsUseingNow)
            {
                StartCoroutine(useItem.Item.Use(mover.TilePos, ItemDir));
                yield break;
            }


            useCo = StartCoroutine(useItem.Item.Use(mover.TilePos, ItemDir));

            if (!useItem.Item.IsUsing)
            {
                useCo = null;
                yield break;
            }

            yield return new WaitUntil(() => useItem.Item.IsFinishUse);

            itemUIs.RemoveAt(Index);
            itemUIs.Add(useItem);

            useItem.Uninit();

            for (int i = Index; i < ItemCount; i++)
            {
                itemUIs[i].Index = i;
            }
            ItemCount--;

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

            if (mover.WasChangedTilePosThisFrame || WasChangedItemDirThisFrame || previewd != Current)
            {
                specialPreviewMarkerManager.ResetAllMarker();
                previewd = Current;
                if (ItemCount == 0 || previewd == null || previewd.Item == null)
                {
                    return;
                }

                previews.Clear();
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
