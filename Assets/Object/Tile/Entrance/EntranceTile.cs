namespace ReLeaf
{
    public class EntranceTile : TileObject, ISetRoomTile
    {
        public Room Room { get; private set; }
        public void SetRoom(Room room) => Room = room;
    }
}
