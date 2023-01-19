using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class SeaUrchinSpecialPower : SowSeedSpecialPowerBase
    {
        SeaUrchinSpecialPowerInfo Info => ItemBaseInfo as SeaUrchinSpecialPowerInfo;
        protected override ISowSeedSpecialPowerInfo SowSeedSpecialPowerInfo => Info;

        Vector2Int target;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, HashSet<Vector2Int> returns)
        {
            if (!IsUsing)
                target = tilePos + (dir * Info.Distance);

            base.PreviewRange(target, dir, returns);
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            using var _ = RobotGreening.Singleton.StartGreening();
            target = tilePos + (dir * Info.Distance);

            var worldTarget = DungeonManager.Singleton.TilePosToWorld(target);


            var mover = RobotMover.Singleton;

            mover.UpdateManualOperation(worldTarget, Info.Speed, true);

            yield return new WaitUntil(() => mover.Distance < Info.StartSowSeedDistance);
            mover.IsStop = true;
            mover.GetComponent<RobotAnimation>().Thrust();

            yield return new WaitForSeconds(0.5f);
            GamepadVibrator.Singleton.Vibrate(GamepadVibrator.VibrationStrength.Normal, 0.3f);
            SEManager.Singleton.Play(Info.SeUrchinSpecial);
            yield return base.UseImpl(target, dir);

            mover.IsStop = false;
        }
    }
}
