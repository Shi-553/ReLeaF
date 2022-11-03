using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class Room : MonoBehaviour
    {
        [SerializeField]
        GameObject roomDoor;
        [SerializeField]
        GameObject enemyRoot;

        public void EnterRoom()
        {
            roomDoor.SetActive(true);
            var enemys = enemyRoot.GetComponentsInChildren<IRoomEnemy>();
            foreach (var enemy in enemys)
            {
                enemy.CanAttackPlayer = true;
            }
        }
        private void Update()
        {
            if (enemyRoot.transform.childCount == 0)
            {
                roomDoor.SetActive(false);
            }
        }

    }
}