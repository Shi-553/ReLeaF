using Utility;

namespace ReLeaf
{
    public class RoomBlastRate : GreeningRateBase
    {
        Room room;

        int roomBlastTargetCount = 0;
        public bool AnyRoomBlastTargetCount => roomBlastTargetCount > 0;

        protected override void Start()
        {
            TryGetComponent(out room);
            base.Start();
            PlayerMover.Singleton.OnChangeRoom += OnChangeRoom;
            OnChangeRoom(PlayerMover.Singleton.Room);

            foreach (var enemyCore in GetComponentsInChildren<EnemyCore>())
            {
                enemyCore.OnDeath += Greening;
                roomBlastTargetCount++;
            }
            foreach (var spawnLakeGroups in GetComponentsInChildren<SpawnLakeGroup>())
            {
                spawnLakeGroups.Targets.ForEach(t => t.OnSpawnEnemy += OnSpawnEnemy);
                spawnLakeGroups.OnGreeningAll += Greening;
                roomBlastTargetCount++;
            }


        }

        private void OnSpawnEnemy(EnemyMover mover)
        {
            mover.GetComponent<EnemyCore>().OnDeath += Greening;
            roomBlastTargetCount++;
        }

        private void Greening()
        {
            roomBlastTargetCount--;
            if (PlayerMover.Singleton.Room == room && !AnyRoomBlastTargetCount)
            {
                RoomBlastRateUI.Singleton.Inactive();
            }
        }

        private void OnChangeRoom(Room playerRoom)
        {
            if (playerRoom == room && ValueRate <= 1.0f && AnyRoomBlastTargetCount)
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
            if (AnyRoomBlastTargetCount)
                room.GreeningRoom();
        }


        protected override bool IsValidChange(DungeonManager.TileChangedInfo obj) => room.ContainsRoom(obj.tilePos);
    }
}
