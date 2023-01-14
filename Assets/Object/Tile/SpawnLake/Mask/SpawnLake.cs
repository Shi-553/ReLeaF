namespace ReLeaf
{
    public class SpawnLake : TileObject
    {
        public bool IsGreening { get; private set; }

        public override bool CanGreening(bool useSpecial) => useSpecial && !IsGreening;
        public override bool IsAlreadyGreening => IsGreening;

        private void Start()
        {
            SpawnLakeManager.Singleton.AddEnabledLake(this);
        }

        public void Greening()
        {
            IsGreening = true;
            SpawnLakeManager.Singleton.ChangeToDisabledLake(this);
        }
    }
}
