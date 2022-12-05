using UnityEditor;
using UnityEngine;

namespace Utility
{
    [CustomPropertyDrawer(typeof(EditTilePosAttribute))]
    public class EditTilePosDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            var att = attribute as EditTilePosAttribute;

            var buttonPos = pos;

            buttonPos.width = 50;
            buttonPos.x = EditorGUIUtility.currentViewWidth - 58;
            buttonPos.height = EditorGUIUtility.singleLineHeight;

            if (GUI.Button(buttonPos, "Edit"))
            {
                TilePosEditor.Open(property, att.direction);
            }

            do
            {
                if (property.isArray)
                {
                    break;
                }
            } while (property.Next(true));

            pos.width -= 60;
            EditorGUI.PropertyField(pos, property, label, true);

        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            do
            {
                if (property.isArray)
                {
                    break;
                }
            } while (property.Next(true));
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            return height;
        }
    }
}