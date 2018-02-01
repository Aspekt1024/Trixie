﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrallax : MonoBehaviour {

    public Camera MainCamera;
    public float Distance = 1f;
    public Vector2 Offset;

    private Vector3 startingPos;

    protected void Start()
    {
        startingPos = transform.position;
    }

    protected void Update()
    {
        if (MainCamera == null) return;

        Vector2 pos = startingPos + MainCamera.transform.position * Distance;
        transform.position = (Vector3)Offset + new Vector3(pos.x, pos.y, transform.position.z);
    }
}
