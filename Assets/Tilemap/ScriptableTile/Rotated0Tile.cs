#if UNITY_EDITOR
#endif

namespace ReLeaf
{
    public class Rotated0Tile : RotatedSandPaddingTile
    {
        public override int Angle => 0;

        public override bool IsSizeReverseXY => false;
    }
}
