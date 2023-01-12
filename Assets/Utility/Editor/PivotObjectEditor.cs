using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace EditorScripts
{

    public abstract class PivotObjectEditor<T> : InspectorExtension<T> where T : Transform
    {
        bool isPivot = false;
        bool isOldPivot = false;

        Type beforeToolType;

        SerializedObject current;

        protected override void Init()
        {
            current = serializedObject;
        }
        public override void OnInspectorGUI()
        {
            //１つでも子供がいないのを選択していたらお断りする
            if (components.Any(t => t.childCount == 0))
            {
                defaultEditor.OnInspectorGUI();
                isPivot = false;
                return;
            }
            //ピボットモードじゃなかったらトグルを用意してさようなら
            if (!isPivot)
            {
                defaultEditor.OnInspectorGUI();
                isPivot = GUILayout.Toggle(isPivot, "Edit Pivot", "Button", GUILayout.Width(100));

                if (isOldPivot)
                {
                    if (beforeToolType != null)
                        ToolManager.SetActiveTool(beforeToolType);
                    isOldPivot = false;
                }
                return;
            }

            if (!isOldPivot)
            {
                beforeToolType = ToolManager.activeToolType;
                ToolManager.SetActiveTool<PivotObjectEditorTool>();
                isOldPivot = true;
            }

            //子たち
            var childrens = components.SelectMany(t =>
            {
                List<Transform> c = new List<Transform>();
                for (int i = 0; i < t.childCount; i++)
                {
                    c.Add(t.GetChild(i));
                }
                return c;
            }).ToArray();

            //タプル
            (Vector3, Quaternion)[] posRotates = childrens.Select(t => (t.position, t.rotation)).ToArray();

            defaultEditor.OnInspectorGUI();


            isPivot = GUILayout.Toggle(isPivot, "Edit Pivot", "Button", GUILayout.Width(100));

            if (current.UpdateIfRequiredOrScript())
            {
                Undo.RecordObjects(childrens, typeof(T).Name);


                for (int i = 0; i < childrens.Length; i++)
                {
                    Transform t = childrens[i];
                    t.SetPositionAndRotation(posRotates[i].Item1, posRotates[i].Item2);
                }
            }
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Transform))]
    public class PivotTransformEditor : PivotObjectEditor<Transform>
    {
        protected override string GetTypeName()
        {
            return "UnityEditor.TransformInspector";
        }
    }
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RectTransform))]
    public class PivotRectTransformEditor : PivotObjectEditor<RectTransform>
    {
        protected override string GetTypeName()
        {
            return "UnityEditor.RectTransformEditor";
        }
    }



    /// <summary>
    /// Pivot的なオブジェクトを子供を気にせずに動かしたいときにどうぞ。Scaleは面倒みないから注意
    /// </summary>
    [EditorTool("Pivot Move Rotate Tool")]
    public class PivotObjectEditorTool : EditorTool
    {
        public override void OnToolGUI(EditorWindow window)
        {

            // これ以降に定義されるGUIの変更を監視する
            EditorGUI.BeginChangeCheck();

            // ハンドルの現在のワールド座標と回転状況を取得する
            var pos = Tools.handlePosition;
            var rotate = Tools.handleRotation;
            Handles.TransformHandle(ref pos, ref rotate);

            // 監視してたGUIが変更されていればこのif内に入る
            if (EditorGUI.EndChangeCheck())
            {
                //１つでも子供がいないのを選択していたらお断りする
                if (Selection.transforms.Any(t => t.childCount == 0))
                {
                    Debug.LogWarning("子供がないオブジェクトが含まれています。");
                    return;
                }

                //親たち
                var parents = Selection.transforms;
                var parentCount = parents.Length;

                //子たち
                var childrens = parents.SelectMany(t =>
                {
                    List<Transform> c = new List<Transform>();
                    for (int i = 0; i < t.childCount; i++)
                    {
                        c.Add(t.GetChild(i));
                    }
                    return c;
                });

                //親達と子達を連結させた
                var parentAndChilds = parents.Concat(childrens).ToArray();

                // この移動をUndoしたときに戻せるように履歴を保存しておく
                Undo.RecordObjects(parentAndChilds, typeof(PivotObjectEditorTool).Name);

                // 直前のハンドル位置からどれだけ移動したかを計算する
                var deltaPosition = pos - Tools.handlePosition;
                var deltaRotate = rotate * Quaternion.Inverse(Tools.handleRotation);

                var poss = parentAndChilds.Select(t => t.position).ToArray();
                var rotates = parentAndChilds.Select(t => t.rotation).ToArray();


                // その差分を現在ヒエラルキーで選択しているすべてのオブジェクトに対して適用する
                for (int i = 0; i < parentCount; i++)
                {
                    parentAndChilds[i].rotation = deltaRotate * rotates[i];
                    parentAndChilds[i].position = poss[i] + deltaPosition;
                }

                // その子供に影響のないようにする
                for (int i = parentCount; i < parentAndChilds.Length; i++)
                {
                    parentAndChilds[i].rotation = rotates[i];
                    parentAndChilds[i].position = poss[i];
                }
            }
        }
    }
}
