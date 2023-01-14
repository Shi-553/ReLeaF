namespace ReLeaf
{
    public class EntranceTile : TileObject, ISetRoom
    {
        public Room Room { get; private set; }
        public void SetRoom(Room room) => Room = room;
    }
}
