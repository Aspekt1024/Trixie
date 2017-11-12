using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform ObjectToFollow;

    private Rigidbody2D followBody;

    private float xMin = 0f;
    private float xMax = 100f;
    private float yMin = 0f;
    private float yMax = 30f;


    private float xOffset = 0f;
    private float yOffset = 0f;

    private void Start ()
    {
        followBody = ObjectToFollow.GetComponent<Rigidbody2D>();
    }
	
	private void Update ()
    {
        if (followBody != null)
        {
            xOffset = Mathf.Lerp(xOffset, followBody.velocity.x, Time.deltaTime * 2f);
            yOffset = Mathf.Lerp(yOffset, followBody.velocity.y, Time.deltaTime * 2f);
        }

        Vector2 targetPos = new Vector2();
        targetPos.x = Mathf.Clamp(ObjectToFollow.position.x + xOffset, xMin, xMax);
        targetPos.y = Mathf.Clamp(ObjectToFollow.position.y + yOffset, yMin, yMax);
        
        Vector2 newPos = Vector2.Lerp(transform.position, targetPos, 2 * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
	}
}
