using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class HoverToSelect : MonoBehaviour, IPointerEnterHandler
    {
        private Selectable selectable = null;
       
        private void Awake()
        {
            TryGetComponent(out selectable);
         

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            selectable.Select();
        }

       
    }
}
