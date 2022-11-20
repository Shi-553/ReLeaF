using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ReLeaf
{
    public enum DamageType
    {
        Direct,
        Shooting,
        Explosion
    }
    [Serializable]
    public struct DamageMagnification
    {
        [SerializeField, Rename("ダメージタイプ")]
        public DamageType damageType;
        [SerializeField, Rename("倍率")]
        public float magnification;
    }

    public abstract class Plant : MonoBehaviour
    {
        [SerializeField]
        PlantInfo plantInfo;

        [SerializeField, ReadOnly]
        float hp;
        public float Hp => hp;

        [SerializeField]
        protected GameObject seedObjRoot;
        [SerializeField]
        protected GameObject plantObjRoot;

        public bool IsFullGrowth { get; private set; }

        public Vector2Int TilePos { get; private set; }

        Coroutine growCo;
        public bool IsFouceGrowing { get; private set; }

        protected void Init()
        {
            IsFullGrowth = false;
            hp = plantInfo.HpMax;
            TilePos = DungeonManager.Instance.WorldToTilePos(transform.position);

            growCo= StartCoroutine(Growing());
        }
        IEnumerator Growing()
        {
            if (!IsFouceGrowing)
            {
                // 完全に成長するまで
                yield return new WaitForSeconds(plantInfo.GrowTime);
            }
            growCo = null;
            IsFullGrowth = true;
            FullGrowed();
        }

        virtual protected void FullGrowed()
        {
            seedObjRoot.SetActive(false);
            plantObjRoot.SetActive(true);
        }

        public virtual void Damaged(float damage, DamageType type)
        {
            if (!IsFullGrowth)
            {
                hp = 0;
                Withered();
                return;
            }

            foreach (var magnification in plantInfo.DamageMagnifications)
            {
                if (magnification.damageType == type)
                {
                    damage *= magnification.magnification;
                }
            }
            hp -= damage;
            if (hp <= 0)
            {
                Withered();
            }
        }
        protected virtual void Withered()
        {
            DungeonManager.Instance.Messy(this);
            Destroy(gameObject);
        }

        public void FouceGrowing()
        {
                IsFouceGrowing = true;
            if (growCo != null)
            {
                Debug.Log("StopAndFullGrowed");
                StopCoroutine(growCo);
                growCo = null;
                IsFullGrowth = true;
                FullGrowed();
            }

        }

    }
}