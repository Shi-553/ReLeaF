using Pickle;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class EnemyCore : MonoBehaviour, IEnemyDamageable, IRoomBlastTarget
    {
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
        public bool IsDeath { get; private set; }


        EnemyMover enemyMover;
        EnemyAttacker attacker;

        [SerializeField, Pickle(ObjectProviderType.Assets)]
        ItemBase specialPowerPrefab;
        [SerializeField]
        MarkerManager weakMarkerManager;
        [SerializeField]
        AudioInfo seEnemyDeath;

        [SerializeField]
        AudioInfo seEnemyDamaged;

        [SerializeField]
        SpriteRenderer sprite;

        int greeningCount = 0;

        public event Action OnDeath;

        private void Start()
        {
            TryGetComponent(out enemyMover);
            HP = enemyDamageableInfo.HPMax;
            TryGetComponent(out attacker);

            spriteOriginalLocalPos = sprite.transform.localPosition;
        }
        private IEnumerator Death()
        {
            HP = 0;
            SEManager.Singleton.Play(seEnemyDeath, enemyMover.WorldCenter);

            attacker.Stop();

            for (int x = 0; x < enemyMover.TileSize.x; x++)
            {
                for (int y = 0; y < enemyMover.TileSize.y; y++)
                {
                    DungeonManager.Singleton.SowSeed(new Vector2Int(enemyMover.TilePos.x + x, enemyMover.TilePos.y + y), true);
                }
            }
            ResetWeekMarker();

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

                if (DungeonManager.Singleton.TryGetTile(worldTilePos, out var tile) && tile.CanOrAleeadyGreening(true))
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
            if (weakMarkerManager.ResetMarker(tilePos))
            {
                greeningCount++;
                Damaged(atk * greeningCount);
                return;
            }
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

            DamagedMotion();
        }

        float damagedTime = 0;
        Coroutine damageMotionCo;
        void DamagedMotion()
        {
            var currentTime = Time.time;

            if (currentTime - damagedTime > 0.1f)
            {
                if (damageMotionCo != null)
                {
                    StopCoroutine(damageMotionCo);
                }

                damageMotionCo = StartCoroutine(DamagedMotionWait());
            }

            damagedTime = currentTime;
        }
        Vector3 spriteOriginalLocalPos;
        Color damagedColor = new(1, 0.5f, 0.5f);
        float damagedOneMotionDuration = 0.1f;
        IEnumerator DamagedMotionWait()
        {
            var spriteTransform = sprite.transform;
            var targetFirst = spriteOriginalLocalPos + Vector3.right * enemyDamageableInfo.DamageMotionXMax;
            var targetSecond = spriteOriginalLocalPos - Vector3.right * enemyDamageableInfo.DamageMotionXMax;

            if (spriteTransform.localPosition.x < spriteOriginalLocalPos.x)
                (targetFirst, targetSecond) = (targetSecond, targetFirst);

            float time = 0;

            var color = Color.white;
            var wasFlashing = color != sprite.color;

            while (true)
            {
                Vector3 target;

                if (time < damagedOneMotionDuration)
                    target = targetFirst;
                else if (time < damagedOneMotionDuration * 2)
                    target = targetSecond;
                else if (time < damagedOneMotionDuration * 3)
                    target = spriteOriginalLocalPos;
                else
                    break;

                spriteTransform.localPosition = Vector3.Lerp(spriteTransform.localPosition, target, 0.5f);


                bool isFlashing = (((int)(time / damagedOneMotionDuration)) % 2) == 0;
                if (wasFlashing)
                    isFlashing = !isFlashing;

                var currentColor = isFlashing ? damagedColor : color;

                if (currentColor != sprite.color)
                    sprite.color = currentColor;

                yield return null;
                time += Time.deltaTime;
            }
            spriteTransform.localPosition = spriteOriginalLocalPos;
            sprite.color = color;
            damageMotionCo = null;
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
            ResetWeekMarker();
            attacker.Stop();
            IsDeath = true;
        }
        public void Greening()
        {
            Damaged(999);
        }

    }
}
