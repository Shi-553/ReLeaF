using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class DoorSwitch : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GetComponentInParent<Room>().EnterRoom();
                other.GetComponent<PlayerController>().EnterRoom();
                Destroy(gameObject);
            }
            else if (other.gameObject.CompareTag("Fruit")&&other.TryGetComponent<Fruit>(out var fruit)&&fruit.IsAttack)
            {
                Destroy(other.gameObject);
            }
        }
    }
}