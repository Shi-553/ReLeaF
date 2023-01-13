namespace ReLeaf
{
    public class SpawnLake : TileObject, ISetRoomTile
    {

        public bool IsGreening { get; private set; }

        public override bool CanGreening(bool useSpecial) => useSpecial && !IsGreening;
        public override bool IsAlreadyGreening => IsGreening;

        public SpawnLakeGroup Group { get; set; }

        public Room Room { get; private set; }
        public void SetRoom(Room room) => Room = room;

        protected override void InitImpl()
        {
            base.InitImpl();
            SpawnLakeManager.Singleton.AddEnabledLake(this);
        }

        public void Greening()
        {
            IsGreening = true;
            SpawnLakeManager.Singleton.ChangeToDisabledLake(this);
        }
    }
}
