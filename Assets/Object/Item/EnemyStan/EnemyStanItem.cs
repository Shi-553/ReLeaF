using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class EnemyStanItem : ItemBase
    {
        EnemyStanItemInfo Info => ItemBaseInfo as EnemyStanItemInfo;
        
        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, HashSet<Vector2Int> returns)
        {
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            GamepadVibrator.Singleton.Vibrate(GamepadVibrator.VibrationStrength.Normal, 0.3f);
            IsFinishUse = true;

            List<EnemyCore> cores = new();

            RoomManager.Singleton.GetAllEnemyCores(cores);

            cores.ForEach(core => core.Stan());
           

            yield return new WaitForSeconds(Info.Duration);

            cores.Where(core => core && core.gameObject)
                .ForEach(core => core.UnStan());
        }
    }
}
