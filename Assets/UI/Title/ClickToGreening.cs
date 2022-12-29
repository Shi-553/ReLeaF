using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class ClickToGreening : MonoBehaviour
    {
        [SerializeField]
        AudioInfo seClick;
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
               
                var hit = Physics2D.GetRayIntersection(new Ray(transform.position, transform.position - Camera.main.transform.position));
                if (hit)
                {
                    var tilePos = DungeonManager.Singleton.WorldToTilePos(hit.point);
                    SEManager.Singleton.Play(seClick, transform.position,1.0f);
                    SEManager.Singleton.Play(seClick, transform.position, 1.0f);
                    GlobalCoroutine.Singleton.StartCoroutine(FindObjectOfType<AllGreening>().StartGreening(tilePos, false, true));
                }
            });
        }

    }
}
