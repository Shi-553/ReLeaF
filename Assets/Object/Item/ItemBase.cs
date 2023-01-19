using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField]
        ItemBaseInfo itemBaseInfo;
        public ItemBaseInfo ItemBaseInfo => itemBaseInfo;

        new Collider2D collider2D;
        public Sprite Icon => GetComponentInChildren<SpriteRenderer>().sprite;


        public bool IsFetched { get; private set; }

        AnimancerComponent animancer;
        bool isFirst = true;

        public void Init()
        {
            Init(isFirst);
            isFirst = false;
        }
        protected virtual void Init(bool isFirst)
        {
            if (isFirst)
            {
                TryGetComponent(out animancer);
                collider2D = GetComponentInChildren<Collider2D>();
            }

            animancer.Stop();
            animancer.Play(itemBaseInfo.InitAnimation);
            IsFetched = false;
        }
        IEnumerator RandomMove()
        {
            Vector3 dir = Random.insideUnitCircle.normalized;

            float time = 0;
            while (true)
            {
                var delta = Time.deltaTime;

                transform.position += 2 * delta * dir;
                yield return null;

                time += delta;

                if (time > 0.2f)
                    yield break;
            }
        }

        public bool Fetch()
        {
            if (IsFetched)
                return false;

            IsFetched = true;
            gameObject.SetActive(false);
            UseCount = 0;

            return true;
        }
        public void ReStart()
        {
            Init();
            StartCoroutine(RandomMove());
        }

        public IEnumerator WaitCollisionDisable()
        {
            collider2D.enabled = false;
            yield return new WaitForSeconds(1);
            collider2D.enabled = true;
        }
        public int UseCount { get; private set; }
        public bool IsUsing => UseCount > 0;

        public bool IsFinishUse { protected set; get; }

        public abstract void PreviewRange(Vector2Int tilePos, Vector2Int dir, HashSet<Vector2Int> returns);
        public IEnumerator Use(Vector2Int tilePos, Vector2Int dir)
        {
            UseCount++;

            if (UseCount == 1)
            {
                GamepadVibrator.Singleton.Vibrate(GamepadVibrator.VibrationStrength.Normal, 0.1f);
                SEManager.Singleton.Play(itemBaseInfo.UseSe);
                yield return UseImpl(tilePos, dir);
                IsFinishUse = true;
                Destroy(gameObject);
            }

        }
        protected abstract IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir);


    }
}
