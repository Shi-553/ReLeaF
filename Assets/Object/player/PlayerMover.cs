using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("プレイヤーの移動")]
    public partial class PlayerMover : MonoBehaviour
    {

        [SerializeField, Rename("プレイヤーの移動スピード(nマス/秒)")]
        float moveSpeed = 5;

        [SerializeField, Rename("ダッシュ中の移動スピード倍率(n倍)")]
        float dashSpeedMagnification = 2;

        [SerializeField, Rename("ノックバックの減衰率（0でノックバックしない、1.0でノックバックし続ける）")]
        float knockBackDampingRate = 0.9f;

        [SerializeField, Rename("ダッシュで消費するスタミナ(n/秒)")]
        float dashConsumeEnergy = 0.1f;

        [SerializeField, Rename("1マス緑化したときのエネルギー回復量(n/マス)")]
        float energyRecoveryPoint = 1.0f;

        [SerializeField, Rename("緑化マスでの自動エネルギー回復量(n/秒)")]
        float energyAutoRecoveryPoint = 1.0f;

        [SerializeField]
        ValueGaugeManager energyGauge;
        Rigidbody2DMover mover;

        bool isKnockback = false;

        [Serializable]
        class SequentialSE
        {
            int index = 0;
            [SerializeField]
            AudioInfo[] list;
            public AudioInfo Get()
            {
                var clip = list[index];
                index = (index + 1) % list.Length;
                return clip;
            }
        }
        [Serializable]
        class MoveSE
        {
            [SerializeField]
            SequentialSE walk;
            [SerializeField]
            SequentialSE dash;
            public AudioInfo Get(bool isDash)
            {
                return isDash ? dash.Get() : walk.Get();
            }
        }
        [SerializeField]
        MoveSE seSandMove;
        [SerializeField]
        MoveSE seGrassMove;


        public Vector2 Move { get; set; }
        public bool IsMove => Move != Vector2.zero;
        public bool IsLeft { get; private set; }
        public bool IsDash { get; set; }

        public Vector2Int OldTilePos { get; private set; }
        public Vector2Int TilePos { get; private set; }

        public bool WasChangedTilePosThisFrame => OldTilePos != TilePos;

        private void Awake()
        {
            TryGetComponent(out mover);
            isKnockback = false;
        }
        void Update()
        {
            if (GameRuleManager.Singleton.IsPrepare)
                return;

            OldTilePos = TilePos;
            TilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);

            if (Move.x != 0)
                IsLeft = Move.x < 0;

            var speed = moveSpeed;

            var currentTile = DungeonManager.Singleton.GetTile(TilePos);

            var isFullGrowthPlant = currentTile is Plant plantTile && plantTile.IsFullGrowth;
            if (isFullGrowthPlant)
            {
                energyGauge.RecoveryValue(energyAutoRecoveryPoint * Time.deltaTime);
            }

            if (IsDash)
            {
                // 緑化マスではエネルギー消費無し
                if (isFullGrowthPlant || energyGauge.ConsumeValue(dashConsumeEnergy * Time.deltaTime))
                {
                    speed *= dashSpeedMagnification;
                }
                else
                {
                    IsDash = false;
                }
            }


            mover.MoveDelta(DungeonManager.CELL_SIZE * speed * Move);

            if (WasChangedTilePosThisFrame && IsMove)
            {
                if (currentTile != null)
                {
                    var se = currentTile.TileType == TileType.Plant ? seGrassMove : seSandMove;
                    SEManager.Singleton.Play(se.Get(IsDash), transform.position);
                }
            }

        }
        public IEnumerator KnockBack(Vector3 impulse)
        {
            isKnockback = true;
            while (true)
            {
                mover.MoveDelta(DungeonManager.CELL_SIZE * impulse);


                impulse *= knockBackDampingRate;

                if (impulse.sqrMagnitude < 0.01f)
                {
                    isKnockback = false;
                    yield break;
                }
                yield return null;
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (GameRuleManager.Singleton.IsPrepare || isKnockback)
                return;
            if (IsMove && collision.gameObject.CompareTag("Sand"))
            {
                if (DungeonManager.Singleton.SowSeed(DungeonManager.Singleton.WorldToTilePos(collision.transform.position), PlantType.Foundation))
                {
                    energyGauge.RecoveryValue(energyRecoveryPoint);

                }
            }
        }
    }
}
