using UnityEngine;
using UnityEngine.EventSystems;

namespace ReLeaf
{
    public class SelectAtEnable : MonoBehaviour
    {
        [SerializeField]
        GameObject target;
        GameObject Target => target != null ? target : gameObject;
        void Start()
        {
            Select();
        }
        void OnEnable()
        {
            Select();
        }
        public void Select()
        {
            if (EventSystem.current)
                EventSystem.current.SetSelectedGameObject(Target);
        }
    }
}
