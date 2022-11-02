using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum DamageType
{
    Direct,
    Shooting,
    Explosion
}
[Serializable]
public struct DamageMagnification
{
    public DamageType damageType;
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

    public bool IsFullGrowth  { get; private set; }

    public Vector3Int TilePos { get; private set; }

    protected void Init()
    {
        IsFullGrowth = false;
        hp = plantInfo.HpMax;
        TilePos = DungeonManager.Instance.WorldToTilePos(transform.position);

        StartCoroutine(Growing());
    }
    IEnumerator Growing()
    {
        // äÆëSÇ…ê¨í∑Ç∑ÇÈÇ‹Ç≈
        yield return new WaitForSeconds(plantInfo.GrowTime);

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

}
