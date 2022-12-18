#if UNITY_EDITOR
#endif

namespace ReLeaf
{
    public class Rotated270Tile : RotatedSandPaddingTile
    {
        public override int Angle => 270;

        public override bool IsSizeReverseXY => true;
    }
}
