using System;
using System.Collections;
using UnityEngine;
using Utility;

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

    public abstract class Plant : TileObject, IMultipleVisual
    {
        PlantInfo PlantInfo => Info as PlantInfo;

        [SerializeField, ReadOnly]
        float hp;
        public float Hp => hp;

        [SerializeField]
        protected GameObject seedObjRoot;
        [SerializeField]
        protected GameObject plantObjRoot;

        protected MeshRenderer plantMeshRenderer;

        public bool IsFullGrowth { get; private set; }


        Coroutine growCo;
        public bool IsFouceGrowing { get; private set; }

        protected void Awake()
        {
            plantMeshRenderer = plantObjRoot.GetComponentInChildren<MeshRenderer>();
        }

        protected override void InitImpl()
        {
            isSetWeakMarker = false;

            if (!gameObject.activeInHierarchy)
                return;
            base.InitImpl();

            hp = PlantInfo.HpMax;
            if (IsFullGrowth)
            {
                IsFullGrowth = false;

                seedObjRoot.SetActive(true);
                plantObjRoot.SetActive(false);
            }

            growCo = StartCoroutine(Growing());
        }
        IEnumerator Growing()
        {
            if (!IsFouceGrowing)
            {
                // 完全に成長するまで
                yield return new WaitForSeconds(PlantInfo.GrowTime);
            }
            growCo = null;
            IsFullGrowth = true;
            FullGrowed();
        }

        public enum VisualType
        {
            Normal,
            BlackGrass,
            BlockCover,
            DokudamiLeaf,
            DokudamiFlower,
            Max
        }

        [SerializeField, Rename("見た目")]
        VisualType visualType;

        int IMultipleVisual.VisualType => (int)visualType;

        public int VisualMax => VisualType.Max.ToInt32();

        virtual protected void FullGrowed()
        {
            seedObjRoot.SetActive(false);
            plantObjRoot.SetActive(true);
            SetWeakMarker();
        }

        public virtual void Damaged(float damage, DamageType type)
        {
            if (IsInvincible)
                return;

            if (!IsFullGrowth)
            {
                hp = 0;
                Withered();
                return;
            }

            foreach (var magnification in PlantInfo.DamageMagnifications)
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
            DungeonManager.Singleton.Messy(TilePos, this);
        }

        bool isSetWeakMarker;
        public bool IsSetWeakMarker
        {
            get => isSetWeakMarker;
            set
            {
                isSetWeakMarker = value;
                if (hp > 0)
                {
                    SetWeakMarker();
                }
            }
        }

        void SetWeakMarker()
        {
            plantMeshRenderer.material = IsSetWeakMarker ? PlantInfo.WeakMarkerdMaterial : PlantInfo.Material;
        }
    }
}