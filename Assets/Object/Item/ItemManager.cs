﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class ItemManager : MonoBehaviour
    {
        List<ItemUI> itemUIs = new List<ItemUI>();
        [SerializeField]
        RectTransform itemUIRoot;
        [SerializeField]
        Transform selectFrame;
        [SerializeField]
        MarkerManager seedMarkerManager;

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
        ItemBase previewd;

        Vector3 itemOffset;

        List<Vector2Int> previews = new();

        Coroutine useCo;

        private void Start()
        {
            itemUIRoot.GetComponentsInChildren(true, itemUIs);
            itemOffset = itemUIs[1].transform.localPosition - itemUIs[0].transform.localPosition;
            mainCamera = Camera.main;

            mover = GetComponentInParent<PlayerMover>();
            ItemCount = 0;


            foreach (var item in itemUIs)
            {
                item.Offset = itemOffset;
            }
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
                item.transform.localPosition = local;
            }
            item.Index = ItemCount;

            SEManager.Singleton.Play(seGetItem, transform.position);
            ItemCount++;
        }

        public IEnumerator UseItem()
        {
            if (GameRuleManager.Singleton.IsPrepare)
                yield break;
            if (ItemCount == 0 || useCo != null)
                yield break;

            var useItem = Current;

            itemUIs.RemoveAt(Index);
            itemUIs.Add(useItem);

            useCo = StartCoroutine(useItem.Item.Use(mover.TilePos, ItemDir));

            SEManager.Singleton.Play(seUseItem, transform.position);

            useItem.Uninit();
            ItemCount--;

            for (int i = Index; i < ItemCount; i++)
            {
                itemUIs[i].Index = i;
            }

            yield return useCo;

            useCo = null;
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

            if (mover.WasChangedTilePosThisFrame || WasChangedItemDirThisFrame || previewd != Current.Item)
            {
                seedMarkerManager.ResetAllMarker();
                if (ItemCount == 0)
                {
                    return;
                }
                previewd = Current.Item;
                previewd.PreviewRange(mover.TilePos, ItemDir, previews);

                foreach (var p in previews)
                {
                    seedMarkerManager.SetMarker<SeedMarker>(p);
                }
            }
            OldItemDir = ItemDir;
        }
    }
}
