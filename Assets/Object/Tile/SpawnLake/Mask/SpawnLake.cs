using System;

namespace ReLeaf
{
    public class SpawnLake : TileObject
    {
        public bool IsGreening { get; private set; }

        public override bool CanGreening(bool useSpecial) => useSpecial && !IsGreening;
        public override bool IsAlreadyGreening => IsGreening;

        public event Action OnGreening;

        protected override void Start()
        {
            SpawnLakeManager.Singleton.AddEnabledLake(this);
        }

        public void Greening()
        {
            IsGreening = true;
            GamepadVibrator.Singleton.Vibrate(GamepadVibrator.VibrationStrength.Normal, 0.2f);
            SpawnLakeManager.Singleton.ChangeToDisabledLake(this);

            var worldPos = DungeonManager.Singleton.TilePosToWorld(TilePos);
            TileEffectManager.Singleton.SetEffect(TileEffectType.Blast, worldPos);
            OnGreening?.Invoke();
        }
    }
}
