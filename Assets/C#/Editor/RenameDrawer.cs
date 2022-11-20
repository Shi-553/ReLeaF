using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Runtime.InteropServices.ComTypes;

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
        EditorGUI.PropertyField(position, property, new GUIContent( att.NewName, att.Tooltip == "" ? property.displayName : att.Tooltip), true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        var att = attribute as RenameAttribute;
        if (!att.IsFormated&& property.serializedObject.targetObject is Component component)
        {
            att.Format(component);
        }

        return EditorGUI.GetPropertyHeight(property, new GUIContent(att.NewName, att.Tooltip == "" ? property.displayName : att.Tooltip), true);
    }
}