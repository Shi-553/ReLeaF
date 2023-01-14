using System;
using UnityEditor;
using UnityEditor.EditorTools;

/// <summary>
/// Automatically manages visibility of UI and 3D objects based on whether the editor is in 2D or 3D mode.
/// </summary>
[InitializeOnLoad]
internal class Disable2DChangeRectTool
{
    private static bool isLastModeWas2D;
    static Type lastBeforeToolType;
    static Type beforeToolType;
    static Disable2DChangeRectTool()
    {
        if (SceneView.lastActiveSceneView != null)
            isLastModeWas2D = SceneView.lastActiveSceneView.in2DMode;

        beforeToolType = ToolManager.activeToolType;

        EditorApplication.update += Update;
        ToolManager.activeToolChanged += ActiveToolChanged;
    }

    private static void ActiveToolChanged()
    {
        beforeToolType = ToolManager.activeToolType;
    }

    private static void Update()
    {
        var sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null)
            return;

        if (sceneView.in2DMode && !isLastModeWas2D && lastBeforeToolType != null)
            ToolManager.SetActiveTool(lastBeforeToolType);

        lastBeforeToolType = beforeToolType;
        isLastModeWas2D = sceneView.in2DMode;
    }

}

