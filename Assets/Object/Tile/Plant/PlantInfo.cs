using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    [ClassSummary("�A��({asset.dirname})�̃p�����[�^")]
    [CreateAssetMenu]
    public class PlantInfo : ScriptableObject
    {
        [SerializeField, Rename("�A���^�C�v")]
        PlantType plantType;
        public PlantType PlantType => plantType;

        [SerializeField, Rename("�ő�HP")]
        float hpMax;
        public float HpMax => hpMax;

        [SerializeField, Rename("�����ɂ����鎞��(�b)")]
        float growTime = 10.0f;
        public float GrowTime => growTime;


        [SerializeField, Rename("�_���[�W�^�C�v���Ƃ̔�_���[�W�{��")]
        DamageMagnification[] damageMagnifications;
        public IReadOnlyCollection<DamageMagnification> DamageMagnifications => damageMagnifications;
    }
}