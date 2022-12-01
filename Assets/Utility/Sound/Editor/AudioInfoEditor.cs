using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    [CustomEditor(typeof(AudioInfo))]
    [CanEditMultipleObjects]
    public class AudioInfoEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            var info = target as AudioInfo;
            var infos = targets.OfType<AudioInfo>();

            using var scope = new EditorGUI.ChangeCheckScope();

            EditorGUI.BeginDisabledGroup(true);
            info.clip = EditorGUILayout.ObjectField("Clip", info.clip, typeof(AudioClip), false) as AudioClip;
            EditorGUI.EndDisabledGroup();

            var volume = EditorGUILayout.Slider("Volume", info.volume, 0, 1);

            if (scope.changed)
            {
                foreach (var i in infos)
                {
                    i.volume = volume;

                    EditorUtility.SetDirty(i);
                    if (Application.isPlaying)
                    {
                        var source = BGMManager.Singleton.GetPlayingSource(i.clip);
                        source = (source == null) ? SEManager.Singleton.GetPlayingSource(i.clip) : source;
                        if (source == null)
                            continue;
                        source.volume = volume;
                    }
                }
            }
        }
    }
}
