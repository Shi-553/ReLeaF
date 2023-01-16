using UnityEditor;
using UnityEngine;

public static class HideFlagsUtility
{
    [MenuItem("Help/Hide Flags/Show All Objects")]
    [MenuItem("Tools/Hide Flags/Show All Objects")]
    private static void ShowAll()
    {
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            switch (go.hideFlags)
            {
                case HideFlags.HideAndDontSave:
                    go.hideFlags = HideFlags.DontSave;
                    break;
                case HideFlags.HideInHierarchy:
                case HideFlags.HideInInspector:
                    go.hideFlags = HideFlags.None;
                    break;
            }
        }
    }
}