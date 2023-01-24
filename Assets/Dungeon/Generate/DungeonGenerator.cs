using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [System.Flags]
    public enum EntranceDirection
    {
        None = 0,
        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,

        TopBottom = Top | Bottom,
        LeftRignt = Left | Right,
        All = Top | Bottom | Left | Right,
    }
    public static class EntranceDirectionExtension
    {
        public static Vector2Int GetVector2Int(this EntranceDirection dir)
        {
            return dir switch
            {
                EntranceDirection.Top => Vector2Int.down,
                EntranceDirection.Bottom => Vector2Int.up,
                EntranceDirection.Left => Vector2Int.left,
                EntranceDirection.Right => Vector2Int.right,
                _ => Vector2Int.zero,
            };
        }
        public static EntranceDirection Inverse(this EntranceDirection dir)
        {
            var inverse = EntranceDirection.None;

            if (dir.HasFlag(EntranceDirection.Top))
                inverse |= EntranceDirection.Bottom;
            if (dir.HasFlag(EntranceDirection.Bottom))
                inverse |= EntranceDirection.Top;
            if (dir.HasFlag(EntranceDirection.Left))
                inverse |= EntranceDirection.Right;
            if (dir.HasFlag(EntranceDirection.Right))
                inverse |= EntranceDirection.Left;

            return inverse;
        }
        public static EntranceDirection RandomOneDir()
        {
            return (EntranceDirection)(0x1 << Random.Range(0, 4));
        }

    }

    public class DungeonGenerator : SingletonBase<DungeonGenerator>
    {
        public ReadOnlyDictionary<TileType, TerrainTile[]> TerrainTileDic => DungeonTilePalette.Singleton.TerrainTileDic;

        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        EnemyMover[] enemys;
        public ReadOnlyDictionary<EnemyType, EnemyMover> EnemyDic { get; private set; }
        [SerializeField]
        ItemBase[] items;
        public ReadOnlyDictionary<ItemType, ItemBase> ItemDic { get; private set; }

        Dictionary<Vector2Int, EntranceDirection> roomDic = new();
        Queue<Vector2Int> roomQueue = new();

        bool TryAddRoom(Vector2Int pos)
        {

            if (roomDic.ContainsKey(pos))
                return false;

            roomQueue.Enqueue(pos);

            var entranceDirection = EntranceDirection.None;

            entranceDirection |= EntranceDirectionExtension.RandomOneDir();

            if (Random.value < 0.5f)
                entranceDirection |= EntranceDirectionExtension.RandomOneDir();

            roomDic[pos] = entranceDirection;
            return true;
        }

        [SerializeField]
        int roomMax = 10;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {

                EnemyDic = new(enemys.ToDictionary(e => e.GetComponent<EnemyCore>().EnemyType, e => e));
                ItemDic = new(items.ToDictionary(i => i.ItemBaseInfo.ItemType, e => e));

                var outerGenerator = new OuterGenerator(TerrainTileDic[TileType.Wall][0], TerrainTileDic[TileType.Wall][1]);

                roomQueue.Enqueue(Vector2Int.zero);
                roomDic.Add(Vector2Int.zero, EntranceDirection.All);

                int roomCount = 0;
                while (true)
                {
                    if (!roomQueue.TryDequeue(out var pos))
                        break;

                    foreach (var dir in roomDic[pos].GetUniqueFlags())
                    {
                        if (TryAddRoom(pos + dir.GetVector2Int()))
                            roomCount++;

                        if (roomCount > roomMax)
                            break;
                    }
                    if (roomCount > roomMax)
                        break;
                }

                foreach (var (pos, entranceDir) in roomDic.ToArray())
                {
                    foreach (var flag in entranceDir.GetUniqueFlags())
                    {
                        var otherPos = pos + flag.GetVector2Int();
                        if (!roomDic.TryGetValue(otherPos, out var otherEntranceDir))
                        {
                            roomDic[pos] &= ~flag;
                            continue;
                        }

                        if (!otherEntranceDir.Inverse().HasFlag(flag))
                        {
                            roomDic[otherPos] |= flag.Inverse();
                            //roomDic[pos] &= ~flag;
                        }
                    }
                }

                foreach (var (pos, entranceDir) in roomDic.ToArray())
                {
                    if (entranceDir != EntranceDirection.None)
                        continue;

                    foreach (var dir in EntranceDirection.All.GetUniqueFlags())
                    {
                        var otherPos = pos + dir.GetVector2Int();
                        if (roomDic.ContainsKey(otherPos))
                        {
                            roomDic[otherPos] |= dir.Inverse();
                            roomDic[pos] |= dir;
                            break;
                        }
                    }
                }

                var roomGenerators = new List<RoomGenerator>();
                foreach (var (pos, entranceDir) in roomDic)
                {
                    //Random.Range(10, 30), Random.Range(10, 30)
                    var room = new NormalRoomGenerator(pos * 41, new(20, 20), entranceDir);
                    roomGenerators.Add(room);

                    room.GenerateTileMap();
                }

                foreach (var room in roomGenerators)
                {
                    foreach (var (type, localPos) in room.RoomTileMapWithPos)
                    {
                        DungeonManager.Singleton.Tilemap.SetTile((Vector3Int)(room.Center + localPos), TerrainTileDic[type][0]);

                    }
                }
                foreach (var room in roomGenerators)
                {
                    foreach (var dir in room.EntranceDirection.GetUniqueFlags())
                    {
                        if (!dir.HasAny(EntranceDirection.Top | EntranceDirection.Left))
                            continue;

                        var isTop = dir.HasFlag(EntranceDirection.Top);

                        var size = isTop ? room.HalfSize.y : room.HalfSize.x;
                        var adjacent = isTop ? Vector2Int.left : Vector2Int.down;

                        DungeonManager.Singleton.Tilemap.SetTile((Vector3Int)(room.Center + dir.GetVector2Int() * (size + 1)), TerrainTileDic[TileType.Sand][0]);
                        DungeonManager.Singleton.Tilemap.SetTile((Vector3Int)(room.Center + dir.GetVector2Int() * (size + 1) + adjacent), TerrainTileDic[TileType.Sand][0]);
                    }
                }

                foreach (var room in roomGenerators)
                {
                    room.GenerateRoom();
                }

                outerGenerator.GenerateWall();
            }
        }


        public bool SpawnEnemy(Vector2Int tilePos, EnemyType type, Transform room = null)
        {
            for (int x = 0; x < EnemyDic[type].TileSize.x; x++)
            {
                for (int y = 0; y < EnemyDic[type].TileSize.y; y++)
                {
                    var tile = DungeonManager.Singleton.Tilemap.GetTile<TerrainTile>(new(tilePos.x + x, tilePos.y + y));

                    if (tile == null || tile.CurrentTileObject.TileType != TileType.Sand)
                        return false;
                }
            }

            var pos = DungeonManager.Singleton.TilePosToWorld(tilePos);
            var enemy = Instantiate(EnemyDic[type]);
            enemy.transform.position = pos;
            enemy.transform.parent = room != null ? room : RoomManager.Singleton.transform;

            return true;
        }

        public ItemType GetRandomItemType()
        {
            return (ItemType)Random.Range(0, ItemType.Max.ToInt32());
        }
        public void SpawnItem(Vector2Int tilePos, ItemType type, Transform room = null)
        {
            var pos = DungeonManager.Singleton.TilePosToWorld(tilePos);
            var item = Instantiate(ItemDic[type]);
            item.transform.position = pos;
            item.transform.parent = room != null ? room : RoomManager.Singleton.transform;
        }
    }
}
