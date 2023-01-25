using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("プレイヤーの移動")]
    public partial class PlayerMover : SingletonBase<PlayerMover>
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

        [SerializeField]
        CircleCollider2D rangeCollider;

        bool isKnockback = false;

        public event Action<Room> OnChangeRoom;
        public event Action<Vector2Int> OnGreening;
        public event Action<Vector2Int> OnChangeTilePos;

        Room room;
        public Room Room
        {
            get => room;
            set
            {
                if (room != value)
                {
                    room = value;
                    OnChangeRoom?.Invoke(Room);
                }
            }
        }

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

        public Vector2 SpecialDir { get; private set; }
        public float SpecialSpeed { get; private set; }
        public bool IsSpecialMoving { get; private set; }
        public void StartSpecialMove(Vector2 dir, float speed)
        {
            IsSpecialMoving = true;
            SpecialDir = dir;
            SpecialSpeed = speed;
        }
        public void FinishSpecialMove()
        {
            IsSpecialMoving = false;
        }

        Vector2 dir;
        public Vector2 Dir
        {
            get => IsSpecialMoving ? SpecialDir : dir;
            set
            {
                dir = value;

                if (!IsMove)
                    isDash = false;
            }
        }
        public float Speed
        {
            get => IsSpecialMoving ? SpecialSpeed : moveSpeed;
            set => moveSpeed = value;
        }
        public bool IsMove => Dir != Vector2.zero;
        public bool IsLeft { get; private set; }

        bool isDash;
        public bool IsDash
        {
            get => isDash;
            set
            {
                isDash = IsMove ? value : false;
            }
        }

        public Vector2Int OldTilePos { get; private set; }
        public Vector2Int TilePos { get; private set; }

        public bool WasChangedTilePosThisFrame => OldTilePos != TilePos;

        HashSet<TileObject> underTiles = new(20);
        HashSet<Vector2Int> waitGreeningTiles = new(10);

        bool isForeachingWaitGreeningTiles = false;

        bool CanSowSeed => GameRuleManager.Singleton.IsPlaying && !isKnockback && IsMove;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                TryGetComponent(out mover);
                isKnockback = false;
                energyGauge.Slider = PlayerStatusUI.Singleton.EnelgySlider;
                TilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);
                OldTilePos = TilePos;
            }
        }


        void Update()
        {
            if (!GameRuleManager.Singleton.IsPlaying)
                return;

            OldTilePos = TilePos;
            TilePos = DungeonManager.Singleton.WorldToTilePos(transform.position);

            if (Dir.x != 0)
                IsLeft = Dir.x < 0;

            var speed = Speed;

            var currentTile = DungeonManager.Singleton.GetTile(TilePos);

            var isFullGrowthPlant = (currentTile is Plant plantTile && plantTile.IsFullGrowth) ||
                                    (currentTile is not Plant && currentTile.IsAlreadyGreening);

            if (!IsSpecialMoving)
            {
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
            }

            if (!IsSpecialMoving)
                speed += addedMoveSpeed;

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
                isForeachingWaitGreeningTiles = true;
                foreach (var underTile in waitGreeningTiles)
                {
                    SowSeed(underTile);
                }
                isForeachingWaitGreeningTiles = false;
                waitGreeningTiles.Clear();
            }

            if (WasChangedTilePosThisFrame)
                OnChangeTilePos?.Invoke(TilePos);
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
                OnGreening?.Invoke(tilePos);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // タイルじゃないときreturn
            if (!collision.TryGetComponent<TileObject>(out var tileObject))
                return;

            if (tileObject.TileType == TileType.Entrance && tileObject is EntranceTile entrance)
                Room = entrance.Room;

            // 緑化できないときreturn
            if (!tileObject.CanOrAleadyGreening(false))
                return;
            // 既に処理済みの場合return
            if (!underTiles.Add(tileObject))
                return;


            if (!CanSowSeed)
            {
                if (!isForeachingWaitGreeningTiles)
                    waitGreeningTiles.Add(tileObject.TilePos);
                return;
            }

            SowSeed(tileObject.TilePos);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.TryGetComponent<TileObject>(out var tileObject))
                return;

            if (tileObject.TileType == TileType.Entrance && tileObject is EntranceTile entrance)
            {
                if (!entrance.Room.ContainsRoom(TilePos))
                    Room = null;
            }

            underTiles.Remove(tileObject);

            if (!CanSowSeed && !isForeachingWaitGreeningTiles)
            {
                waitGreeningTiles.Remove(tileObject.TilePos);
            }
        }

        float addedMoveSpeed = 0;
        public void SpeedUp(float value)
        {
            addedMoveSpeed += value;
        }
        public void SpeedDown(float value)
        {
            addedMoveSpeed -= value;
        }


        public void AddGreeningRange(float value)
        {
            rangeCollider.radius += value;
        }
        public void RemoveGreeningRange(float value)
        {
            rangeCollider.radius -= value;
        }
    }
}
