using System.Collections.Generic;
using Utility;

namespace ReLeaf
{
    public class RoomManager : SingletonBase<RoomManager>
    {
        public Room[] Rooms { get; private set; }
        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }

        private void Start()
        {
            Rooms = GetComponentsInChildren<Room>();
        }

        public void GetAllEnemyCores(List<EnemyCore> enemyCores)
        {
            GetComponentsInChildren(enemyCores);
        }
    }
}
