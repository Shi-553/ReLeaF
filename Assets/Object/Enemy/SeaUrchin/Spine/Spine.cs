using System.Collections;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class Spine : MonoBehaviour
    {
        [SerializeField]
        Transform sprite;
        [SerializeField]
        Transform shadow;

        Vector2Int dir;
        Rigidbody2DMover mover;
        bool isSooting = false;

        [SerializeField]
        SpineInfo info;

        [SerializeField]
        Vision targetVision;

        public void Init(Vector2Int dir)
        {
            this.dir = dir;
            sprite.rotation = Quaternion.Euler(0, 0, 0) * dir.GetRotation();
            shadow.rotation = dir.GetRotation();
            TryGetComponent(out mover);
            StartCoroutine(InitAnimation());
        }
        IEnumerator InitAnimation()
        {
            float time = 0;
            yield return new WaitForSeconds(info.InitAnimationDelay);
            if (this == null)
                yield break;
            while (true)
            {
                mover.MoveDelta(DungeonManager.CELL_SIZE * info.InitAnimationSpeed * (Vector2)dir);

                if (time > info.InitAnimationTime)
                    yield break;
                time += Time.deltaTime;
                yield return null;

                if (this == null)
                    yield break;
            }
        }

        public void ShotStart()
        {
            isSooting = true;
        }
        private void Update()
        {
            if (!isSooting)
                return;
            mover.MoveDelta(DungeonManager.CELL_SIZE * info.Speed * (Vector2)dir);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isSooting || !collision.gameObject.activeSelf)
                return;

            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Plant") || collision.gameObject.CompareTag("Player"))
            {
                if (targetVision.UpdateTarget())
                {
                    foreach (var target in targetVision.Targets())
                    {
                        if (target.TryGetComponent(out Plant plant))
                        {
                            plant.Damaged(info.ATK, DamageType.Shooting);
                            continue;
                        }
                        if (target.TryGetComponent(out PlayerCore player))
                        {
                            player.Damaged(info.ATK, (transform.position - target.position).normalized * info.KnockBackPower);
                            continue;
                        }
                    }
                }
                isSooting = false;
                Destroy(gameObject);
            }
        }
    }
}
