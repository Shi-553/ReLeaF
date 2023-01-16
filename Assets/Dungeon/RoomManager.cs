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
            if (isFirstInit)
            {
                Rooms = GetComponentsInChildren<Room>();
            }
        }

        List<EnemyCore> allEnemyCores = new();

        /// <summary>
        /// GC free and Temporary
        /// </summary>
        public IReadOnlyList<EnemyCore> GetTempAllEnemyCores()
        {
            GetAllEnemyCores(allEnemyCores);
            return allEnemyCores;
        }

        public void GetAllEnemyCores(List<EnemyCore> enemyCores)
        {
            GetComponentsInChildren(enemyCores);
        }
    }
}
