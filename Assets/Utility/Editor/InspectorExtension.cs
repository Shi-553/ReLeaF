using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorScripts
{
    public abstract class InspectorExtension<T> : Editor where T : Component
    {
        //Unity's built-in editor
        protected Editor defaultEditor;
        protected T[] components;
        protected T component;

        protected abstract string GetTypeName();

        void OnEnable()
        {
            var type = typeof(Editor)
                .Assembly
                .GetType(GetTypeName());

            //When this inspector is created, also create the built-in inspector
            defaultEditor = Editor.CreateEditor(targets, type);
            components = targets.Select(t => t as T).ToArray();
            component = target as T;

            Init();
        }
        protected virtual void Init() { }

        void OnDisable()
        {
            //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
            //Also, make sure to call any required methods like OnDisable
            MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (disableMethod != null)
            {
                disableMethod.Invoke(defaultEditor, null);
            }
            DestroyImmediate(defaultEditor);
        }
    }
}
