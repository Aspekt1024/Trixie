using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHandler : MonoBehaviour {

    public Transform RespawnPoint;

	public void SetObjectToPoint(Transform obj)
    {
        obj.position = RespawnPoint.position;
    }
}
