using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utility
{

    public class DocumantGenerator : EditorWindow
    {
        [MenuItem("Window/DocumantGenerator")]
        static void Open()
        {
            var window = GetWindow<DocumantGenerator>();
            window.titleContent = new GUIContent("DocumantGenerator");
        }

        private void OnGUI()
        {

            GUILayout.Label("Document Generator", EditorStyles.boldLabel);

            string text = "";

            if (GUILayout.Button("Generate!!"))
            {
                var activeScenePath = EditorSceneManager.GetActiveScene().path;

                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return;
                }

                var types = new Type[] { typeof(GameObject), typeof(ScriptableObject), typeof(GameObject), typeof(SceneAsset) };
                var assets = FindAssetsByType(types);

                // ディレクトリごと
                foreach (var dirAssetsPair in assets)
                {
                    var tempText = "";
                    int index = 0;
                    // アセットごと
                    foreach (var (name, asset) in dirAssetsPair.Value)
                    {
                        if (asset is GameObject obj && obj != null)
                        {
                            var components = obj.GetComponentsInChildren<Component>(true);

                            // コンポーネントごと
                            foreach (var component in components)
                            {
                                if (GetObject(index, dirAssetsPair.Key + "\\" + component.transform.GetFullPath("-") + "->" + component.GetType().Name, component, out var add))
                                {
                                    tempText += add;
                                    index++;
                                }
                            }
                        }
                        if (asset is ScriptableObject scriptableObject && scriptableObject != null)
                        {
                            if (GetObject(index, dirAssetsPair.Key + "\\" + name, scriptableObject, out var add))
                            {
                                tempText += add;
                                index++;
                            }
                        }
                        if (asset is SceneAsset sceneAsset && sceneAsset != null)
                        {
                            var scenePath = dirAssetsPair.Key + "\\" + name + ".unity";

                            try
                            {
                                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                                var gameobjects = scene.GetRootGameObjects();
                                foreach (var gameObject in gameobjects)
                                {
                                    var components = gameObject.GetComponentsInChildren<Component>(true);

                                    // コンポーネントごと
                                    foreach (var component in components)
                                    {
                                        if (PrefabUtility.IsPartOfAnyPrefab(component))
                                        {
                                            continue;
                                        }

                                        if (GetObject(index, dirAssetsPair.Key + "\\" + name + "\\" + component.transform.GetFullPath("-") + "->" + component.GetType().Name, component, out var add))
                                        {
                                            tempText += add;
                                            index++;
                                        }
                                    }
                                }
                            }
                            catch (ArgumentException)
                            {
                                // シーンを読み込めなかったのでスルー
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(tempText))
                    {
                        text += "\t" + dirAssetsPair.Key + "\n";
                        text += tempText + "\n";
                    }
                }

                EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);

                string path = EditorUtility.SaveFilePanelInProject("Save Document Text", "Document", "txt", "Save Document Text");
                if (path == "")
                    return;

                File.WriteAllText(path, text);
            }
        }


        /// <summary>
        /// クラス単位でテキストにする
        /// </summary>
        bool GetObject(int index, string path, Object c, out string text)
        {
            bool isFirst = true;
            text = "";
            foreach (var line in GetLine(c))
            {
                text += index;
                if (isFirst)
                {
                    text += "\t\"";
                    var classAttribute = c.GetType().GetCustomAttribute<ClassSummaryAttribute>();
                    if (classAttribute != null)
                    {
                        if (!classAttribute.IsFormated)
                        {
                            classAttribute.Format(c as Component, path);
                        }
                        text += classAttribute.Sammary + "\n";
                    }
                    text += path + "\"\t";
                    isFirst = false;
                }
                else
                {
                    text += "\t\t";
                }
                text += line + "\n";
            }
            return !isFirst;
        }

        /// <summary>
        /// 実際にイレテータをまわしてSerializedPropertyを問い合わせる
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerable<string> GetLine(Object target)
        {
            var serializedObject = new SerializedObject(target);
            var serializedProperty = serializedObject.GetIterator();

            while (serializedProperty.Next(true))
            {
                var att = serializedProperty.GetAttributes<RenameAttribute>(false);
                if (att != null)
                {
                    if (!att.IsFormated && target is Component component)
                    {
                        att.Format(component);
                    }
                    var type = (serializedProperty.isArray) ? serializedProperty.arrayElementType + "[]" : serializedProperty.type;
                    var valString = SerializedPropertyToString(serializedProperty);

                    yield return serializedProperty.name + "\t" + type + "\t" + valString + "\t" + "\"" + att.NewName + (att.Tooltip == "" ? "" : "\n" + att.Tooltip) + "\"";
                }
            }
        }

        /// <summary>
        /// SerializedProperty単位（変数単位）で文字列にする
        /// 配列も可能
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <returns></returns>
        string SerializedPropertyToString(SerializedProperty serializedProperty)
        {
            string text = "";
            if (serializedProperty.isArray)
            {
                var size = serializedProperty.arraySize;
                for (int i = 0; i < size; i++)
                {
                    text += SerializedPropertyToString(serializedProperty.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                var val = serializedProperty.GetValue();
                var valString = val.ToString();

                if (valString == val.GetType().ToString())
                {
                    text += JsonUtility.ToJson(val);
                }
                else
                {
                    text += valString;
                }
            }
            return text;
        }

        /// <summary>
        /// AssetsフォルダからTypeで検索する
        /// </summary>
        /// <param name="types"></param>
        /// <returns>Dictionary<ディレクトリ, List<Tuple<ファイル名, アセット>>></returns>
        public static Dictionary<string, List<Tuple<string, Object>>> FindAssetsByType(Type[] types)
        {
            var assets = new Dictionary<string, List<Tuple<string, Object>>>();
            string[] guids = AssetDatabase.FindAssets("t:" + String.Join(" t:", types.Select(t => t.Name)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (asset != null)
                {
                    var dirname = Path.GetDirectoryName(assetPath);
                    var filename = Path.GetFileNameWithoutExtension(assetPath);
                    if (assets.TryGetValue(dirname, out var list))
                    {
                        list.Add(new(filename, asset));
                    }
                    else
                    {
                        assets.Add(dirname, new List<Tuple<string, Object>>() { new(filename, asset) });
                    }
                }
            }
            return assets;
        }
    }
}
