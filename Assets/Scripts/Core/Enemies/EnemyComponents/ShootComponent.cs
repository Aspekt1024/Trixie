﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootComponent : MonoBehaviour {

    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 10f;
    public GameObject ShootPoint;

    public Projectile.ProjectileSettings ProjectileSettings;

    protected GameObject target;
    protected Action shootCallback;

    public abstract void Shoot();
    public abstract void Shoot(GameObject target);

}
