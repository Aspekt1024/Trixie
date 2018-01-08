using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootComponent : MonoBehaviour {

    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 10f;
    public GameObject Target;
    public GameObject ShootPoint;

    public EnergyTypes.Colours Colour;

    public abstract void Shoot();
    public abstract void Shoot(GameObject target);
}
