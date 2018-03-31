using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderHandler : MonoBehaviour {

    private static PathfinderHandler pathfinderHandler;

    private void Awake()
    {
        if (pathfinderHandler ==  null)
        {
            pathfinderHandler = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static Transform GetPathfinderWaypointsTransform()
    {
        return pathfinderHandler.transform;
    }
}
