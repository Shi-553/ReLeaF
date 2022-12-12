using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneCleaner : EditorWindow
{
    [InitializeOnLoadMethod]
    static void OnProjectLoadedInEditor()
    {
        EditorSceneManager.sceneSaving += OnSceneSaving;
    }

    private static void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
    {
        var objs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        m_Objects.Clear();
        m_Objects.AddRange(objs);
        var array = m_Objects.Where(o => o != null && o.hideFlags.HasFlag(HideFlags.HideInHierarchy)).ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            if (!PrefabUtility.IsPartOfAnyPrefab(array[i]))
            {
                continue;
            }
            if (AssetDatabase.Contains(array[i]))
            {
                continue;
            }
            DestroyImmediate(array[i], false);
        }

    }

    [MenuItem("Window/SceneCleaner")]
    static void Open()
    {
        var window = GetWindow<SceneCleaner>();
        window.titleContent = new GUIContent("SceneCleaner");
    }

    static List<GameObject> m_Objects = new List<GameObject>();
    void FindObjectsAll()
    {
        var objs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        m_Objects.Clear();
        m_Objects.AddRange(objs);
    }


    void OnGUI()
    {
        if (GUILayout.Button("find ALL object"))
        {
            FindObjectsAll();
            for (int i = 0; i < m_Objects.Count; i++)
            {
                GameObject O = m_Objects[i];
                if (O == null)
                    continue;
                HideFlags flags = O.hideFlags;
                if (flags.HasFlag(HideFlags.HideInHierarchy))
                {
                    flags &= ~HideFlags.HideInHierarchy;
                    flags |= HideFlags.DontSaveInEditor;
                }
                O.hideFlags = flags;
            }
        }
    }
}