using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class SeaUrchinSpecialPower : SowSeedSpecialPowerBase
    {
        [SerializeField]
        SeaUrchinSpecialPowerInfo info;
        protected override ISowSeedSpecialPowerInfo SowSeedSpecialPowerInfo => info;


        Vector2Int target;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns)
        {
            if (!IsUsing)
                target = tilePos + (dir * info.Distance);

            base.PreviewRange(target, dir, returns);
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            using var _ = RobotGreening.Singleton.StartGreening();
            target = tilePos + (dir * info.Distance);

            var worldTarget = DungeonManager.Singleton.TilePosToWorld(target);


            var mover = RobotMover.Singleton;

            mover.UpdateManualOperation(worldTarget, info.Speed, true);

            yield return new WaitUntil(() => mover.Distance < info.StartSowSeedDistance);

            mover.IsStop = true;
            mover.GetComponent<RobotAnimation>().Thrust();

            yield return new WaitForSeconds(0.5f);

            yield return base.UseImpl(target, dir);

            mover.IsStop = false;
        }
    }
}
