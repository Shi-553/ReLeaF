using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility{
    public class SEManager : SoundManager<SEManager>
    {
        protected override int InitSourceCount => 5;

        public AudioSource Play(AudioClip clip, Vector3 pos, float volumeScale = 1.0f, bool loop = false)
        {
            var source = GetSource(true);
            source.clip = clip;
            source.transform.position = pos;
            source.loop = loop;
            source.volume = volumeScale;
            source.Play();

            return source;
        }

        public void Stop(AudioClip clip)
        {
            var source = GetSource(false);
            source.clip = clip;
            source.Stop();
        }
    }
}
