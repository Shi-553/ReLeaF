namespace ReLeaf
{
    public class SpawnLake : TileObject
    {
        public bool IsGreening { get; private set; }

        public void Greening()
        {
            IsGreening = true;
            SpawnLakeManager.Singleton.ChangeToDisabledLake(this);
        }
    }
}
