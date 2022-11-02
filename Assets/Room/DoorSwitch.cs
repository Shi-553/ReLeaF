using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponentInParent<Room>().EnterRoom();
            other.GetComponent<PlayerControler>().EnterRoom();
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Fruit"))
        {
            Destroy(other.gameObject);
        }
    }
}
