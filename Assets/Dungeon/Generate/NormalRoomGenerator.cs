using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class NormalRoomGenerator : RoomGenerator
    {
        public NormalRoomGenerator(Vector2Int center, Vector2Int halfSize, EntranceDirection entranceDirection) : base(center, halfSize, entranceDirection)
        {
        }

        public override void GenerateRoomImpl(Room room, RoomBlastRate rate)
        {
            rate.targetRate = 0.4f;

            room.InitRoomTileEarly();

            var tiles = RoomTileMapWithPos.Where(t => t.type == TileType.Sand && room.ContainsRoom(t.localPos + Center)).ToArray();

            var enemyCount = Random.Range(2.0f, 6.0f) * Mathf.Pow(Size.x * Size.y, 0.9f) / 1000.0f;
            for (int i = 0; i < enemyCount; i++)
            {
                var pos = tiles[Random.Range(0, tiles.Length)].localPos + Center;
                if (!DungeonGenerator.Singleton.SpawnEnemy(pos, EnemyType.Crab, room.transform))
                    i--;
            }
            var itemCount = Random.Range(2.0f, 5.0f) * Mathf.Pow(Size.x * Size.y, 0.9f) / 1000.0f;
            for (int i = 0; i < itemCount; i++)
            {
                var pos = tiles[Random.Range(0, tiles.Length)].localPos + Center;
                var type = DungeonGenerator.Singleton.GetRandomItemType();
                DungeonGenerator.Singleton.SpawnItem(pos, type, room.transform);
            }
        }

        public override void GenerateTileMapImpl()
        {
            var perlinThreshold = Random.Range(0.2f, 0.8f);

            foreach (var pos in AllPositionsWithin)
            {
                var size = Mathf.Abs(pos.x) > Mathf.Abs(pos.y) ? Size.x : Size.y;
                var perlin = PerlinNoise(pos) * Mathf.Pow(pos.magnitude, 1.5f) / size;

                if (perlin > perlinThreshold)
                {
                    SetTile(pos, TileType.Wall);
                }
            }
        }
    }
}
