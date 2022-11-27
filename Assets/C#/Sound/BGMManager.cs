using UnityEngine;

namespace Utility
{
    public class BGMManager : SoundManager<BGMManager>
    {
        protected override int InitSourceCount => 1;

        public void Play(AudioClip clip, float volumeScale = 1.0f)
        {
            StopAll();
            var source = GetSource(false);
            source.clip = clip;
            source.loop = true;
            source.volume = volumeScale;
            source.Play();
        }
    }
}
