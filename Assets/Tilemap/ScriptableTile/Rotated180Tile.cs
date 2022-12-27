#if UNITY_EDITOR
#endif

namespace ReLeaf
{
    public class Rotated180Tile : RotatedSandPaddingTile
    {
        public override int Angle => 180;

        public override bool IsSizeReverseXY => false;
    }
}
