using UnityEngine;

namespace ReLeaf
{

    public class Sand : TileObject
    {
        SandInfo SandInfo => Info as SandInfo;

        MeshRenderer meshRenderer;

        protected override void FasterInitOnlyOnceImpl()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
        protected override void InitImpl()
        {
            base.InitImpl();

            meshRenderer.material = SandInfo.NoRoomMat;
        }

        public override void SetRoom(Room room)
        {
            base.SetRoom(room);

            if (Room != null)
                meshRenderer.material = SandInfo.RoomMat;
        }
    }
}
