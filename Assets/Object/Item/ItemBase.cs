using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public abstract class ItemBase : MonoBehaviour
    {
        public Sprite Icon => GetComponentInChildren<SpriteRenderer>().sprite;

        [SerializeField]
        AudioInfo useSe;

        [SerializeField]
        bool isImmediate = false;
        public bool IsImmediate => isImmediate;

        public bool IsFetched { get; private set; }

        [SerializeField]
        AnimationClip initAnimation;

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
            }

            animancer.Stop();
            animancer.Play(initAnimation);
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

        public int UseCount { get; private set; }
        public bool IsUsing => UseCount > 0;

        public bool IsFinishUse { protected set; get; }

        public abstract void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns);
        public IEnumerator Use(Vector2Int tilePos, Vector2Int dir)
        {
            UseCount++;

            if (UseCount == 1)
            {
                SEManager.Singleton.Play(useSe);
                yield return UseImpl(tilePos, dir);
                IsFinishUse = true;
                Destroy(gameObject);
            }

        }
        protected abstract IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir);


    }
}
