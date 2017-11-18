using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectilePoolItem : ObjectPoolItem
{
    public EnergyTypes.Colours Colour;

    protected override void SetPoolSpecificAttributes(GameObject obj)
    {
        obj.GetComponent<Projectile>().SetType(Colour);
    }
}
