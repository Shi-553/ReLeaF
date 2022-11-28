namespace Utility
{
    public class BGMManager : SoundManager<BGMManager>
    {
        protected override int InitSourceCount => 1;

        public void Play(AudioInfo info, float volumeScale = 1.0f)
        {
            StopAll();
            var source = GetSource(false);
            source.clip = info.clip;
            source.loop = true;
            source.volume = info.volume * volumeScale;
            source.Play();
        }
    }
}
