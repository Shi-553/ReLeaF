using UnityEngine;

namespace Utility
{
    public class AudioInfo : ScriptableObject
    {
        public AudioClip clip;
        public float volume = 1.0f;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                var source = BGMManager.Singleton.GetPlayingSource(clip);
                source = (source == null) ? SEManager.Singleton.GetPlayingSource(clip) : source;
                if (source == null)
                    return;
                source.volume = volume;
            }
        }
    }
}