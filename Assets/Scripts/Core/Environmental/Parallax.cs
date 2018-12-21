using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Parallax : MonoBehaviour {

    public Camera MainCamera;
    public float Distance = 1f;
    public Vector2 Offset;

    protected void Update()
    {
        if (MainCamera == null) return;
        Vector2 pos = MainCamera.transform.position * Distance;
        transform.position = (Vector3)Offset + new Vector3(pos.x, pos.y, transform.position.z);
    }
}
