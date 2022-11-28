using DebugLogExtension;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public class GenerateAudioInfo : EditorWindow
    {
        private DefaultAsset targetFolder = null;

        [MenuItem("Window/AudioInfoGenerater")]
        static void Open()
        {
            var window = GetWindow<GenerateAudioInfo>();
            window.titleContent = new GUIContent("AudioInfoGenerater");
        }

        void CreateSubFolder(string path, string dirName, string newDirName)
        {
            var subFolderPaths = AssetDatabase.GetSubFolders(path);
            foreach (var subFolderPath in subFolderPaths)
            {
                var newPath = subFolderPath.Replace(dirName, newDirName);
                if (!Directory.Exists(newPath))
                {
                    newPath.DebugLog();
                    Directory.CreateDirectory(newPath);
                }
                CreateSubFolder(subFolderPath, dirName, newDirName);
            }
        }
        private void OnGUI()
        {

            GUILayout.Label("Document Generator", EditorStyles.boldLabel);

            targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Select source Folder",
                targetFolder,
                typeof(DefaultAsset),
                false);


            if (GUILayout.Button("Generate!!"))
            {
                if (targetFolder == null)
                {
                    Debug.LogError("Not valid Folder !");
                    return;
                }
                var path = AssetDatabase.GetAssetPath(targetFolder);

                if (string.IsNullOrEmpty(path))
                    return;

                // ディレクトリでなければ、指定を解除する
                bool isDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory);
                if (isDirectory == false)
                {
                    targetFolder = null;
                    return;
                }

                var dirName = Path.GetFileName(path);

                CreateSubFolder(path, dirName, "Info");

                string[] childAssetPathList = AssetDatabase.FindAssets("", new[] { path })
                    .Select(AssetDatabase.GUIDToAssetPath).ToArray();
                AssetDatabase.Refresh();

                foreach (var childPath in childAssetPathList)
                {
                    var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(childPath);
                    if (clip == null)
                        continue;
                    var newPath = childPath.Replace(dirName, "Info") + ".asset";

                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.dataPath), newPath).Replace("/", "\\")))
                    {
                        var info = CreateInstance<AudioInfo>();
                        info.clip = clip;
                        AssetDatabase.CreateAsset(info, newPath);
                        newPath.DebugLog();
                    }
                }
                AssetDatabase.Refresh();
            }
        }
    }
}