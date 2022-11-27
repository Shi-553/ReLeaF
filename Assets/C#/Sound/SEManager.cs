using UnityEngine;

namespace Utility
{
    public class SEManager : SoundManager<SEManager>
    {
        protected override int InitSourceCount => 5;

        public void Play(AudioClip clip, Vector3 pos, float volumeScale = 1.0f)
        {
            var source = GetSource(true);
            source.transform.position = pos;
            source.PlayOneShot(clip, volumeScale);
        }
    }
}
