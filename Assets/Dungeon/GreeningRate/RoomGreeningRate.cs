namespace ReLeaf
{
    public class RoomGreeningRate : GreeningRateBase
    {
        Room room;

        protected override void Start()
        {
            TryGetComponent(out room);
            base.Start();
            PlayerMover.Singleton.OnChangeRoom += OnChangeRoom;
        }

        private void OnChangeRoom(Room playerRoom)
        {
            if (playerRoom == null || playerRoom == room)
                RoomGreeningRateUI.Singleton.Slider.value = ValueRate;
        }

        protected override void CalculateMaxGreeningCount()
        {
            foreach (var pos in room.RoomTilePoss)
            {
                if (DungeonManager.Singleton.TryGetTile(pos, out var tile) && tile.CanOrAleeadyGreening(true))
                {
                    MaxGreeningCount++;
                }
            }
        }

        protected override void UpdateValue()
        {
            OnChangeRoom(PlayerMover.Singleton.LastRoom);
        }
        protected override void Finish()
        {
            room.GreeningRoom();
        }


        protected override bool IsValidChange(DungeonManager.TileChangedInfo obj) => room.ContainsRoom(obj.tilePos);
    }
}
