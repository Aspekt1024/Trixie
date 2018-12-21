using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCamera : MonoBehaviour {

    public Transform Target;
    public float FollowSpeed = 100f;

    private Vector3 velocity;

    private float initialZ;

    private void Start()
    {
        initialZ = transform.position.z;
    }


    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref velocity, 0.01f, 100f, Time.deltaTime);




        transform.position = new Vector3(transform.position.x, transform.position.y, initialZ);

    }

}
