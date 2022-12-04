using UnityEditor;
using UnityEngine;

namespace Utility
{
    [CustomPropertyDrawer(typeof(RenameAttribute))]
    public class RenameEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var att = attribute as RenameAttribute;
            if (!att.IsFormated && property.serializedObject.targetObject is Component component)
            {
                att.Format(component);
            }
            label.text = att.NewName;
            label.tooltip = att.Tooltip == "" ? property.displayName : att.Tooltip;

            EditorGUI.PropertyField(position, property, label, true);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            var att = attribute as RenameAttribute;
            if (!att.IsFormated && property.serializedObject.targetObject is Component component)
            {
                att.Format(component);
            }
            label.text = att.NewName;
            label.tooltip = att.Tooltip == "" ? property.displayName : att.Tooltip;

            return EditorGUI.GetPropertyHeight(property, new GUIContent(att.NewName, att.Tooltip == "" ? property.displayName : att.Tooltip), true);
        }
    }
}