using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility{
    public class BGMManager : SoundManager<BGMManager>
    {
        protected override int InitSourceCount => 1;

        public void Play(AudioClip clip, float volumeScale = 1.0f)
        {
            StopAll();
            GetSource(false).PlayOneShot(clip, volumeScale);
        }
    }
}
