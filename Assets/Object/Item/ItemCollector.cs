using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ReLeaf
{

    public class ItemCollector : MonoBehaviour
    {
        HashSet<ItemBase> floatItems = new HashSet<ItemBase>();

        [SerializeField]
        float floatItemMoveSpeed = 1;
        [SerializeField]
        float collectRange = 0.1f;

        [SerializeField]
        ItemManager itemManager ;


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
                item.transform.position = Vector3.MoveTowards(item.transform.position, transform.position, floatItemMoveSpeed * Time.deltaTime*DungeonManager.CELL_SIZE);
            }

            floatItems.RemoveWhere(item =>
            {
                if ((item.transform.position - transform.position).sqrMagnitude < collectRange* collectRange * DungeonManager.CELL_SIZE && item.Fetch())
                {
                    itemManager.AddItem(item);
                    return true;
                }

                return false;
            });
        }
    }
}
