namespace Utility
{
    public class BGMManager : SoundManager<BGMManager>
    {
        protected override int InitSourceCount => 1;

        public void Play(AudioInfo info)
        {
            if (!CanPlayStart)
                return;
            StopAll();
            var source = GetSource(false);
            source.clip = info.clip;
            source.loop = true;
            source.volume = info.volume;
            source.Play();
        }

        public void Stop()
        {
            StopAll();
        }
    }
}
