using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{

    [ClassSummary("アイテムを引き寄せて回収する")]
    public class ItemCollector : MonoBehaviour
    {
        HashSet<ItemBase> floatItems = new HashSet<ItemBase>();

        [SerializeField, Rename("アイテム引き寄せ速度(nマス/秒)")]
        float floatItemMoveSpeed = 1;

        [SerializeField, Rename("アイテムを回収して使えるようにする範囲(nマス)")]
        float collectRange = 0.1f;

        [SerializeField]
        ItemManager itemManager;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Item"))
            {
                if (other.gameObject.TryGetComponent<ItemBase>(out var item))
                {
                    floatItems.Add(item);
                }
            }
        }
        private void Update()
        {
            foreach (var item in floatItems)
            {
                item.transform.position = Vector3.MoveTowards(item.transform.position, transform.position, floatItemMoveSpeed * Time.deltaTime * DungeonManager.CELL_SIZE);
            }

            floatItems.RemoveWhere(item =>
            {
                if ((item.transform.position - transform.position).sqrMagnitude < collectRange * collectRange * DungeonManager.CELL_SIZE && !item.IsFetched)
                {
                    if (itemManager.AddItem(item))
                    {
                        item.Fetch();
                    }
                    else
                    {
                        item.ReStart();
                    }
                    return true;
                }

                return false;
            });
        }
    }
}
