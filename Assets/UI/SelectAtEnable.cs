using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

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
            if (EventSystem.current != null)
            {
               
                EventSystemUtility.SetSelectedGameObjectNoFade(Target);
             
                if (EventSystem.current.currentInputModule != null)
                {
                    EventSystem.current.currentInputModule.enabled = false;
                    EventSystem.current.currentInputModule.enabled = true;
                   
                }
            }
            
        }
    }
}
