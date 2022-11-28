using DebugLogExtension;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public class GenerateAudioInfo : AssetPostprocessor
    {
        static string GetSourcePath()
        {
            var audioSoruceTargetLocations = AssetDatabase.FindAssets("t:AudioInfoSourceLocation");
            var target = audioSoruceTargetLocations.FirstOrDefault();
            if (target == null)
            {
                throw new System.Exception("AudioSoruceTargetLocation  Not Found !");
            }
            return AssetDatabase.GUIDToAssetPath(target);
        }
        static bool CheckBaseOf(Uri parentUri, string[] children)
        {
            foreach (var child in children)
            {
                var childUri = new Uri(Path.GetFullPath(child));
                if (parentUri != childUri && parentUri.IsBaseOf(childUri))
                {
                    return true;
                }

            }
            return false;
        }
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {

            var path = GetSourcePath();

            var parentUri = new Uri(new DirectoryInfo(Path.GetFullPath(path)).Parent.FullName + "/");

            if (CheckBaseOf(parentUri, importedAssets) || CheckBaseOf(parentUri, deletedAssets) || CheckBaseOf(parentUri, movedAssets) || CheckBaseOf(parentUri, movedFromAssetPaths))
            {
                Debug.Log("Update AudioInfo");
                Update(path.Replace("/" + Path.GetFileName(path), ""));
            }

        }
        static void CreateSubFolder(string path, string dirName, string newDirName)
        {
            var subFolderPaths = AssetDatabase.GetSubFolders(path);
            foreach (var subFolderPath in subFolderPaths)
            {
                var newPath = subFolderPath.Replace(dirName, newDirName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                CreateSubFolder(subFolderPath, dirName, newDirName);
            }
        }
        static void DeleteIfEmpty(string folder)
        {
            foreach (var subdir in Directory.GetDirectories(folder))
                DeleteIfEmpty(subdir);

            if (IsDirectoryEmpty(folder))
                AssetDatabase.DeleteAsset("assets/" + Path.GetRelativePath(Application.dataPath, folder));
        }

        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        static void Update(string sourcePath)
        {

            if (string.IsNullOrEmpty(sourcePath))
                return;


            var dirName = Path.GetFileName(sourcePath);

            CreateSubFolder(sourcePath, dirName, "Info");

            string[] childAssetPathList = AssetDatabase.FindAssets("", new[] { sourcePath })
                .Select(AssetDatabase.GUIDToAssetPath).ToArray();

            AssetDatabase.Refresh();

            var infoDir = sourcePath.Replace(dirName, "Info");

            var audioInfos = AssetDatabase.FindAssets("t:AudioInfo", new[] { infoDir })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AudioInfo>).ToArray();

            foreach (var childPath in childAssetPathList)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(childPath);
                if (clip == null)
                    continue;
                var newPath = childPath.Replace(dirName, "Info") + ".asset";

                if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.dataPath), newPath).Replace("/", "\\")))
                {
                    var f = audioInfos.FirstOrDefault(info => info.clip == clip);
                    if (f != null)
                    {
                        AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(f), newPath);
                    }
                    else
                    {
                        var info = ScriptableObject.CreateInstance<AudioInfo>();
                        info.clip = clip;
                        AssetDatabase.CreateAsset(info, newPath);
                        newPath.DebugLog();
                    }
                }
            }
            foreach (var info in audioInfos)
            {
                if (info.clip == null)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(info));
            }
            DeleteIfEmpty(Path.GetFullPath(infoDir));
            AssetDatabase.Refresh();
        }
    }
}