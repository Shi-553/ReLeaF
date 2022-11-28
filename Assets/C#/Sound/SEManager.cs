
using System.Collections;
using UnityEngine;

namespace Utility
{
    public class SEManager : SoundManager<SEManager>
    {
        protected override int InitSourceCount => 5;

        public AudioSource Play(AudioInfo info, Vector3 pos)
        {
            var source = GetSource(true);
            source.clip = info.clip;
            source.transform.position = pos;
            source.loop = false;
            source.volume = info.volume;
            source.Play();

            return source;
        }
        public AudioSource PlayLoop(AudioInfo info, Transform target)
        {
            var source = GetSource(true);
            source.clip = info.clip;
            source.loop = true;
            source.volume = info.volume;
            source.Play();
            StartCoroutine(FollowTarget(source, info.clip, target));
            return source;
        }
        IEnumerator FollowTarget(AudioSource source, AudioClip clip, Transform target)
        {
            var wait = new WaitForSeconds(clip.length);

            while (true)
            {
                source.transform.position = target.position;
                yield return wait;
                if (!source.isPlaying || source.clip != clip)
                    yield break;
            }
        }

        public void Stop(AudioClip clip)
        {
            var source = GetSource(false);
            source.clip = clip;
            source.Stop();
        }
    }
}
