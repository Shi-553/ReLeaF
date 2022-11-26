using Pickle;
using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField, Rename("�_���[�W�^�C�v")]
        public DamageType damageType;
        [SerializeField, Rename("�{��")]
        public float magnification;
    }

    public abstract class Plant : TileObject, IMultipleVisual
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
        [SerializeField,Pickle(LookupType =ObjectProviderType.Assets)]
        protected TileObject messyObj;

        public bool IsFullGrowth { get; private set; }


        Coroutine growCo;
        public bool IsFouceGrowing { get; private set; }

        public override void Init(bool isCreated)
        {
            base.Init(isCreated);

            hp = plantInfo.HpMax;
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
                // ���S�ɐ�������܂�
                yield return new WaitForSeconds(plantInfo.GrowTime);
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
            Max
        }

        [SerializeField, Rename("������")]
        VisualType visualType;

        public int VisualTypeMax => (int)VisualType.Max;
        int IMultipleVisual.VisualType => (int)visualType;

        virtual protected void FullGrowed()
        {
            seedObjRoot.SetActive(false);
            plantObjRoot.SetActive(true);
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
            DungeonManager.Singleton.Messy(TilePos, this);
        }

        }
    }