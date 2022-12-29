using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utility
{
    public static class EventSystemUtility
    {
        /// <summary>
        /// フェードしないSetSelectedGameObject
        /// </summary>
        public static void SetSelectedGameObjectNoFade(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            var inSelectable = gameObject.GetComponent<Selectable>();
            var outObject = EventSystem.current.currentSelectedGameObject;
            var outSelectable = outObject == null ? null : outObject.GetComponent<Selectable>();

            var inDuration = NoFade(inSelectable);
            var outDuration = NoFade(outSelectable);

            EventSystem.current.SetSelectedGameObject(gameObject);

            SetFade(inSelectable, inDuration);
            SetFade(outSelectable, outDuration);

        }
        static float NoFade(Selectable selectable)
        {
            if (selectable == null)
                return 0;

            var color = selectable.colors;
            var oldDuration = color.fadeDuration;
            color.fadeDuration = 0;
            selectable.colors = color;
            return oldDuration;
        }
        static void SetFade(Selectable selectable, float fadeDuration)
        {
            if (selectable == null)
                return;
            var color = selectable.colors;
            color.fadeDuration = fadeDuration;
            selectable.colors = color;
        }
    }
}
