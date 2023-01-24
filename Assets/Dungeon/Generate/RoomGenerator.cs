using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public abstract class RoomGenerator
    {
        public Vector2Int Center { get; private set; }
        public Vector2Int Size { get; private set; }
        public Vector2Int HalfSize => Size / 2;
        public Vector2Int LocalMin => -HalfSize;
        public Vector2Int LocalMax => HalfSize - Vector2Int.one;

        public EntranceDirection EntranceDirection { get; private set; }

        Vector2 perlinNoiseZero;
        static float PERLIN_NOISE_RANGE = 10;
        static float PERLIN_NOISE_SCALE = 1.5f;
        public RoomGenerator(Vector2Int center, Vector2Int halfSize, EntranceDirection entranceDirection)
        {
            this.Center = center;
            this.Size = halfSize * 2;
            this.EntranceDirection = entranceDirection;

            roomTileMap = new TileType[Size.x, Size.y];

            perlinNoiseZero = new(Random.Range(-PERLIN_NOISE_RANGE / 2, PERLIN_NOISE_RANGE / 2), Random.Range(-PERLIN_NOISE_RANGE / 2, PERLIN_NOISE_RANGE / 2));
        }

        TileType[,] roomTileMap;
        public IEnumerable<TileType> RoomTileMap
        {
            get
            {
                foreach (var item in roomTileMap)
                    yield return item;
            }
        }
        public IEnumerable<int> AllXPositionsWithin
        {
            get
            {
                for (int x = 0; x < Size.x; x++)
                    yield return x - HalfSize.x;
            }
        }
        public IEnumerable<int> AllYPositionsWithin
        {
            get
            {
                for (int y = 0; y < Size.y; y++)
                    yield return y - HalfSize.y;
            }
        }
        public IEnumerable<Vector2Int> AllPositionsWithin
        {
            get
            {
                for (int x = 0; x < Size.x; x++)
                {
                    for (int y = 0; y < Size.y; y++)
                    {
                        yield return new(x - HalfSize.x, y - HalfSize.y);
                    }
                }
            }
        }
        public IEnumerable<(TileType type, Vector2Int localPos)> RoomTileMapWithPos
        {
            get
            {
                for (int x = 0; x < Size.x; x++)
                {
                    for (int y = 0; y < Size.y; y++)
                    {
                        var pos = new Vector2Int(x - HalfSize.x, y - HalfSize.y);
                        yield return (GetTile(pos), pos);
                    }
                }
            }
        }

        public TileType GetTile(Vector2Int pos)
        {
            return roomTileMap[pos.x + HalfSize.x, pos.y + HalfSize.y];
        }
        public void SetTile(Vector2Int pos, TileType type)
        {
            roomTileMap[pos.x + HalfSize.x, pos.y + HalfSize.y] = type;
        }


        protected void FillTileMap(TileType type)
        {
            foreach (var pos in AllPositionsWithin)
            {
                SetTile(pos, type);
            }
        }

        public void GenerateTileMap()
        {
            GenerateTileMapImpl();

            foreach (var x in AllXPositionsWithin)
            {
                var topPos = new Vector2Int(x, LocalMin.y);
                var bottomPos = new Vector2Int(x, LocalMax.y);

                GenerateAround(topPos, x, EntranceDirection.Top);
                GenerateAround(bottomPos, x, EntranceDirection.Bottom);
            }

            foreach (var y in AllYPositionsWithin)
            {
                var leftPos = new Vector2Int(LocalMin.x, y);
                var rightPos = new Vector2Int(LocalMax.x, y);

                GenerateAround(leftPos, y, EntranceDirection.Left);
                GenerateAround(rightPos, y, EntranceDirection.Right);
            }
        }
        public abstract void GenerateTileMapImpl();

        void GenerateAround(Vector2Int pos, float distanceToCenter, EntranceDirection dir)
        {
            var type = (EntranceDirection.HasFlag(dir) && Mathf.Abs(distanceToCenter + 0.5f) < 1.0f) ?
                TileType.Entrance :
                TileType.Wall;


            if (type == TileType.Entrance)
            {
                int size = dir.HasAny(EntranceDirection.TopBottom) ? Size.y : Size.x;

                var dirVec = dir.GetVector2Int();
                for (int i = 0; i < size / 2; i++)
                {
                    SetTile(pos - dirVec * i, TileType.Sand);
                }
            }
            SetTile(pos, type);
        }


        public void GenerateRoom()
        {
            var roomObj = new GameObject("Room");
            var roomTrans = roomObj.transform;
            roomTrans.parent = RoomManager.Singleton.transform;
            roomTrans.position = DungeonManager.Singleton.TilePosToWorld(Center);

            var room = roomObj.AddComponent<Room>();
            var rate = roomObj.AddComponent<RoomBlastRate>();

            GenerateRoomImpl(room, rate);
        }
        public abstract void GenerateRoomImpl(Room room, RoomBlastRate rate);

        public float PerlinNoise(Vector2Int pos)
        {
            return Mathf.PerlinNoise(perlinNoiseZero.x + pos.x * PERLIN_NOISE_SCALE, perlinNoiseZero.y + pos.y * PERLIN_NOISE_SCALE);
        }
    }
}
