using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ReLeaf
{
    [ClassSummary("プレイヤーの移動")]
    public class PlayerMover : MonoBehaviour
    {

        [SerializeField,Rename("プレイヤーの移動スピード(nマス/秒)")]
        float moveSpeed = 5;

        [SerializeField, Rename("ダッシュ中の移動スピード倍率(n倍)")]
        float dashSpeedMagnification = 2;

        [SerializeField, Rename("ノックバックの減衰率（0でノックバックしない、1.0でノックバックし続ける）")]
        float knockBackDampingRate = 0.9f;

        [SerializeField, Rename("ダッシュで消費するスタミナ(n/秒)")]
        float dashConsumeEnergy = 0.1f;

        [SerializeField, Rename("1マス緑化したときのエネルギー回復量(n/マス)")]
        float energyRecoveryPoint = 1.0f;

        [SerializeField]
        ValueGaugeManager energyGauge;
        Rigidbody2DMover mover;

        public Vector2 Move { get; set; }
        public bool IsDash { get; set; }

        public Vector2Int OldTilePos { get; private set; }
        public Vector2Int TilePos { get; private set; }

        public bool WasChangedTilePosThisFrame => OldTilePos != TilePos;

        private void Awake()
        {
            TryGetComponent(out mover);
        }
        void Update()
        {
            OldTilePos = TilePos;
            TilePos = DungeonManager.Instance.WorldToTilePos(transform.position);

            var speed = moveSpeed;

            if (IsDash && energyGauge.ConsumeValue(dashConsumeEnergy * Time.deltaTime))
            {
                speed *= dashSpeedMagnification;
            }

            mover.Move(speed * Move);

            if (DungeonManager.Instance.SowSeed(TilePos, PlantType.Foundation))
            {
                energyGauge.RecoveryValue(energyRecoveryPoint);
            }

        }
        public IEnumerator KnockBack(Vector3 impulse)
        {
            while (true)
            {
                mover.Move(impulse);


                impulse *= knockBackDampingRate;

                if (impulse.sqrMagnitude < 0.01f)
                {
                    yield break;
                }
                yield return null;
            }
        }
    }
}
