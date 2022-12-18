using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField]
        Sprite icon;
        public Sprite Icon => icon;

        [SerializeField]
        bool isImmediate = false;
        public bool IsImmediate => isImmediate;

        public bool IsFetched { get; private set; }

        [SerializeField]
        AnimationClip initAnimation;

        AnimancerComponent animancer;
        bool isFirst = true;

        protected void Awake()
        {
            Init(isFirst);
            isFirst = false;
        }
        public virtual void Init(bool isFirst)
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

            return true;
        }
        public void ReStart()
        {
            Init(isFirst);
            StartCoroutine(RandomMove());
        }


        public abstract void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns);
        public abstract IEnumerator Use(Vector2Int tilePos, Vector2Int dir);
    }
}
