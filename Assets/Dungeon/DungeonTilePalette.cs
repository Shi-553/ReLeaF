using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class DungeonTilePalette : SingletonBase<DungeonTilePalette>
    {

        [SerializeField]
        TerrainTile[] terrainTiles;
        [SerializeField]
        TileArray[] terrainTileArrays;
        public ReadOnlyDictionary<TileType, TerrainTile[]> TerrainTileDic { get; private set; }


        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                terrainTileArrays.ForEach(arr => arr.Sort());

                var dic = terrainTileArrays.ToDictionary(t => t.tiles.First().CurrentTileObject.TileType, t => t.tiles);

                foreach (var terrainTile in terrainTiles)
                {
                    dic.Add(terrainTile.CurrentTileObject.TileType, new[] { terrainTile });
                }

                foreach (var terrainTile in dic)
                {
                    terrainTile.Value.ForEach(tag => tag.Init());
                }

                TerrainTileDic = new ReadOnlyDictionary<TileType, TerrainTile[]>(dic);
            }
        }


    }
}
