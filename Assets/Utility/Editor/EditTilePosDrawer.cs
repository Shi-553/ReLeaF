using UnityEditor;
using UnityEngine;

namespace Utility
{
    [CustomPropertyDrawer(typeof(EditTilePosAttribute))]
    public class EditTilePosDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            var buttonPos = pos;

            buttonPos.width = 50;
            buttonPos.x = EditorGUIUtility.currentViewWidth - 55;
            buttonPos.height = EditorGUIUtility.singleLineHeight;
            if (GUI.Button(buttonPos, "Edit"))
            {
                TilePosEditor.Open(property);
            }

            EditorGUI.PropertyField(pos, property, label, true);

        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            return height;
        }
    }
}