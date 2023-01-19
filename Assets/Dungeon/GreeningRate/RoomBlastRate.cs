namespace ReLeaf
{
    public class RoomBlastRate : GreeningRateBase
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
            if (playerRoom == room && ValueRate <= 1.0f)
            {
                RoomBlastRateUI.Singleton.Active();
                RoomBlastRateUI.Singleton.SetValue(ValueRate);
            }
        }

        protected override void CalculateMaxGreeningCount()
        {
            foreach (var pos in room.RoomTilePoss)
            {
                if (DungeonManager.Singleton.TryGetTile(pos, out var tile) && tile.CanOrAleadyGreening(true))
                {
                    MaxGreeningCount++;
                }
            }
        }

        protected override void UpdateValue()
        {
            OnChangeRoom(PlayerMover.Singleton.Room);
        }
        protected override void Finish()
        {
            room.GreeningRoom();
        }


        protected override bool IsValidChange(DungeonManager.TileChangedInfo obj) => room.ContainsRoom(obj.tilePos);
    }
}
