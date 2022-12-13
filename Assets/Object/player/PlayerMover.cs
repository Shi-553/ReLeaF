using System;
using System.Collections;
using System.Collections.Generic;
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


        public Vector2 Dir { get; set; }
        public bool IsMove => Dir != Vector2.zero;
        public bool IsLeft { get; private set; }
        public bool IsDash { get; set; }

        public Vector2Int OldTilePos { get; private set; }
        public Vector2Int TilePos { get; private set; }

        public bool WasChangedTilePosThisFrame => OldTilePos != TilePos;

        HashSet<TileObject> underTiles = new(20);
        HashSet<Vector2Int> waitGreeningTiles = new(10);

        bool CanSowSeed => !GameRuleManager.Singleton.IsPrepare && !isKnockback && IsMove;

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

            if (Dir.x != 0)
                IsLeft = Dir.x < 0;

            var speed = moveSpeed;

            var currentTile = DungeonManager.Singleton.GetTile(TilePos);

            var isFullGrowthPlant = (currentTile is Plant plantTile && plantTile.IsFullGrowth) ||
                                    (currentTile is not Plant && currentTile.IsAlreadyGreening);
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


            mover.MoveDelta(DungeonManager.CELL_SIZE * speed * Dir);

            if (WasChangedTilePosThisFrame && IsMove)
            {
                if (currentTile != null)
                {
                    var se = currentTile.TileType == TileType.Foundation ? seGrassMove : seSandMove;
                    SEManager.Singleton.Play(se.Get(IsDash), transform.position);
                }
            }

            if (waitGreeningTiles.Count > 0 && CanSowSeed)
            {
                foreach (var underTile in waitGreeningTiles)
                {
                    SowSeed(underTile);
                }
                waitGreeningTiles.Clear();
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



        void SowSeed(Vector2Int tilePos)
        {
            if (DungeonManager.Singleton.SowSeed(tilePos))
            {
                energyGauge.RecoveryValue(energyRecoveryPoint);
                underTiles.Add(DungeonManager.Singleton.GetTile(tilePos));
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent<TileObject>(out var tileObject))
                return;

            if (!underTiles.Add(tileObject))
                return;

            if (!tileObject.CanOrAleeadyGreening(false))
                return;

            if (!CanSowSeed)
            {
                waitGreeningTiles.Add(tileObject.TilePos);
                return;
            }
            SowSeed(tileObject.TilePos);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.TryGetComponent<TileObject>(out var tileObject))
                return;

            underTiles.Remove(tileObject);

            if (!CanSowSeed)
            {
                waitGreeningTiles.Remove(tileObject.TilePos);
            }
        }
    }
}
