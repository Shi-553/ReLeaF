#if UNITY_EDITOR
#endif

namespace ReLeaf
{
    public class Rotated90Tile : RotatedSandPaddingTile
    {
        public override int Angle => 90;

        public override bool IsSizeReverseXY => true;
    }
}
