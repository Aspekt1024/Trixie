using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public enum FollowTypes
    {
        PlayerOnly, PlayerAndAggro
    }
    public FollowTypes FollowType = FollowTypes.PlayerAndAggro;
    public float FollowSpeed = 5f;
    public float TargetRange = 20f;
    
    private float xMin = 0f;
    private float xMax = 1000f;
    private float yMin = 0f;
    private float yMax = 40f;
    
    private float xOffset = 0f;
    private float yOffset = 0f;
    private float sizeOffset = 0f;

    private bool focusOverrideMode;

    private Transform playerTf;
    private Rigidbody2D playerBody;
    private HashSet<Transform> objectsToFollow;
    private Vector2 focusPoint;

    private void Start ()
    {
        playerTf = Player.Instance.transform;
        playerBody = playerTf.GetComponent<Rigidbody2D>();
        objectsToFollow = new HashSet<Transform>();
    }
	
	private void FixedUpdate ()
    {
        if (focusOverrideMode)
        {
            GotoFocusPoint();
        }
        else if (FollowType == FollowTypes.PlayerOnly || objectsToFollow == null || objectsToFollow.Count == 0)
        {
            FollowPlayer();
        }
        else
        {
            FocusAllBodies();
        }
    }

    public void SetCameraFocus(Transform focusTf)
    {
        focusOverrideMode = true;
        focusPoint = focusTf.position;
    }

    public void SetCameraFollow(Transform newObjectToFollow = null)
    {
        if (newObjectToFollow)
        {
            playerTf = newObjectToFollow;
            playerBody = playerTf.GetComponent<Rigidbody2D>();
        }
        focusOverrideMode = false;
    }

    public void AddObjectToFollow(Transform newObject)
    {
        objectsToFollow.Add(newObject);
    }

    public void StopFollowingObject(Transform objectToRemove)
    {
        objectsToFollow.Remove(objectToRemove);
    }

    private void GotoFocusPoint()
    {
        Vector2 newPos = Vector2.Lerp(transform.position, focusPoint, 2 * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    private void FollowPlayer()
    {
        if (playerBody != null)
        {
            xOffset = Mathf.Lerp(xOffset, playerBody.velocity.x, Time.deltaTime * FollowSpeed);
            yOffset = Mathf.Lerp(yOffset, playerBody.velocity.y * 0.75f, Time.deltaTime * FollowSpeed);
        }

        Vector2 targetPos = new Vector2()
        {
            x = Mathf.Clamp(playerTf.position.x + xOffset, xMin, xMax),
            y = Mathf.Clamp(playerTf.position.y + yOffset, yMin, yMax)
        };

        LerpCameraPosition(targetPos);
    }

    private void FocusAllBodies()
    {
        Vector2 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + new Vector2(xOffset, yOffset);

        LerpCameraPosition(centerPoint);
    }

    private Vector2 GetCenterPoint()
    {
        var bounds = new Bounds(playerTf.position, Vector3.zero);

        foreach (Transform tf in objectsToFollow)
        {
            if (Vector2.Distance(tf.position, playerTf.position) <= TargetRange)
            {
                bounds.Encapsulate(tf.position);
            }
        }

        return bounds.center;
    }

    private void LerpCameraPosition(Vector2 targetPos)
    {
        float cameraSpeed = 2f;

        float maxDistFromCenter = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height / 2f)).y - Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height / 4f)).y; // TODO there is a better way to do this
        float playerDistFromCenter = Mathf.Abs(playerTf.position.y - Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height / 2f)).y);

        targetPos.y = Mathf.Lerp(targetPos.y, Player.Instance.transform.position.y, playerDistFromCenter / maxDistFromCenter);
        Vector2 newPos = Vector2.Lerp(transform.position, targetPos, cameraSpeed * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }
}
