using Pickle;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public enum EnemyType
    {
        None,
        Shark,
        Crab,
        SeaUrchin,
        Whale
    }

    public class EnemyCore : MonoBehaviour, IEnemyDamageable, IRoomBlastTarget
    {
        [SerializeField]
        EnemyType enemyType;
        public EnemyType EnemyType => enemyType;

        [SerializeField]
        EnemyDamageableInfo enemyDamageableInfo;

        [SerializeField, ReadOnly]
        float hp;
        public float HP
        {
            get => hp;
            private set
            {
                hp = value;
                if (hp <= 0)
                    IsDeath = true;
            }
        }
        public bool IsValid => !IsDeath && !IsStan;

        public bool IsDeath { get; private set; }
        int stanCount = 0;
        public bool IsStan => stanCount > 0;


        EnemyMover enemyMover;
        EnemyAttacker attacker;
        EnemyAnimationBase enemyAnimationBase;

        [SerializeField, Pickle(ObjectProviderType.Assets)]
        ItemBase specialPowerPrefab;
        [SerializeField]
        MarkerManager weakMarkerManager;
        [SerializeField]
        AudioInfo seEnemyDeath;

        [SerializeField]
        AudioInfo seEnemyDamaged;

        int greeningCount = 0;

        public event Action OnDeath;

        private void Start()
        {
            TryGetComponent(out enemyMover);
            HP = enemyDamageableInfo.HPMax;
            TryGetComponent(out attacker);
            TryGetComponent(out enemyAnimationBase);

        }
        private IEnumerator Death()
        {
            HP = 0;
            SEManager.Singleton.Play(seEnemyDeath, enemyMover.WorldCenter);
            GamepadVibrator.Singleton.Vibrate(GamepadVibrator.VibrationStrength.Normal, 0.5f);

            attacker.Stop();

            ResetWeekMarker();
            for (int x = 0; x < enemyMover.TileSize.x; x++)
            {
                for (int y = 0; y < enemyMover.TileSize.y; y++)
                {
                    var pos = new Vector2Int(enemyMover.TilePos.x + x, enemyMover.TilePos.y + y);
                    DungeonManager.Singleton.SowSeed(pos, true);

                    var worldPos = DungeonManager.Singleton.TilePosToWorld(pos);
                    TileEffectManager.Singleton.SetEffect(TileEffectType.Blast, worldPos);
                }
            }

            OnDeath?.Invoke();

            foreach (var collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
            if (TryGetComponent<EnemyAnimationBase>(out var animationBase))
            {
                yield return animationBase.DeathAnimation();
            }
            Destroy(gameObject);

            if (specialPowerPrefab != null)
                Instantiate(specialPowerPrefab, transform.position, Quaternion.identity, transform.parent).Init(); ;
        }

        public void SetWeekMarker()
        {
            greeningCount = 0;

            var localPoss = enemyDamageableInfo.WeakLocalTilePos.GetLocalTilePosList(enemyMover.DirNotZero);
            foreach (var localPos in localPoss)
            {
                var worldTilePos = enemyMover.TilePos + localPos;

                if (DungeonManager.Singleton.TryGetTile(worldTilePos, out var tile) && tile.CanOrAleadyGreening(true))
                {
                    var marker = weakMarkerManager.SetMarker<WeakMarker>(worldTilePos);
                    if (marker != null)
                    {
                        marker.SetEnemyDamageable(this);
                    }
                }
            }
        }
        public void ResetWeekMarker()
        {
            weakMarkerManager.ResetAllMarker();
        }
        public void DamagedGreening(Vector2Int tilePos, float atk)
        {
            if (HP == 0)
                return;
            greeningCount++;
            Damaged(atk * greeningCount);
        }

        public void Damaged(float atk)
        {
            if (HP == 0)
                return;

            if (DamageValueEffectManager.Singleton != null)
                DamageValueEffectManager.Singleton.SetDamageValueEffect((int)atk, enemyMover.WorldCenter);

            if (HP - atk <= 0)
            {
                StartCoroutine(Death());
                return;
            }
            HP -= atk;
            SEManager.Singleton.Play(seEnemyDamaged, enemyMover.WorldCenter);

            enemyAnimationBase.DamagedMotion();
            GamepadVibrator.Singleton.Vibrate(GamepadVibrator.VibrationStrength.Weak, 0.1f);
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Plant"))
            {
                if (collision.TryGetComponent<Plant>(out var plant) && plant.IsInvincible)
                {
                    Damaged(999);
                }
            }
        }

        public Vector3 Position => enemyMover.WorldCenter;
        public void BeginGreening()
        {
            Stan();
        }
        public void Greening()
        {
            Damaged(999);
        }


        public void Stan()
        {
            stanCount++;
            if (IsStan)
            {
                ResetWeekMarker();
                attacker.Stop();
                enemyAnimationBase.StanAnimation();
            }
        }
        public void UnStan()
        {
            stanCount--;
        }
    }
}
