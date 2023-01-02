using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    public class BlockDoubleClick : MonoBehaviour
    {
        Button[] buttons;

        void Start()
        {
            buttons = GetComponentsInChildren<Button>(true);

            foreach (var button in buttons)
            {
                button.onClick.AddListener(() => OnClick(button));
            }
        }

        void OnClick(Button clicked)
        {
            foreach (var button in buttons)
            {
                if (clicked == button)
                {
                    clicked.targetGraphic = null;
                    clicked.enabled = false;
                    continue;
                }
                button.interactable = false;
            }
        }
    }
}
