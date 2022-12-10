using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("�A��({asset.dirname})�̃p�����[�^")]
    [CreateAssetMenu(menuName = "Tile/PlantInfo")]
    public class PlantInfo : ScriptableObject
    {
        [SerializeField, Rename("�ő�HP")]
        float hpMax;
        public float HpMax => hpMax;

        [SerializeField, Rename("�����ɂ����鎞��(�b)")]
        float growTime = 10.0f;
        public float GrowTime => growTime;


        [SerializeField, Rename("�_���[�W�^�C�v���Ƃ̔�_���[�W�{��")]
        DamageMagnification[] damageMagnifications;
        public IReadOnlyCollection<DamageMagnification> DamageMagnifications => damageMagnifications;

        [SerializeField, Rename("�Ή����̃G�t�F�N�g")]
        ToLeafEffect toLeafEffect;
        public ToLeafEffect ToLeafEffect => toLeafEffect;
    }
}