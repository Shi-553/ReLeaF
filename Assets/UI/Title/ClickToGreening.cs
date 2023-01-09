using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class ClickToGreening : MonoBehaviour
    {

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {

                var hit = Physics2D.GetRayIntersection(new Ray(transform.position, transform.position - Camera.main.transform.position));
                if (hit)
                {
                    var tilePos = DungeonManager.Singleton.WorldToTilePos(hit.point);
                    GlobalCoroutine.Singleton.StartCoroutine(FindObjectOfType<AllGreening>().StartGreening(tilePos, false, true));
                }
            });
        }

    }
}
