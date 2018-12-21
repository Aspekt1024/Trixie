using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TrixiePrototypes.Edit
{
    [CustomEditor(typeof(MovingPlatform))]
    public class MovingPlatformInspector : Editor
    {
        private MovingPlatform platform;
        
        private void OnSceneGUI()
        {
            platform = (MovingPlatform)target;

            // Force Starting Point within limits
            if (platform.StartingPoint >= platform.MovementPoints.Length)
            {
                platform.StartingPoint = platform.MovementPoints.Length - 1;
            }
            if (platform.StartingPoint < 0)
            {
                platform.StartingPoint = 0;
            }
            if (platform.MovementPoints == null || platform.MovementPoints.Length == 0) return;

            platform.transform.position = platform.MovementPoints[platform.StartingPoint];
            
            for (int i = 0; i < platform.MovementPoints.Length; i++)
            {
                Handles.color = new Color(0f, 0.8f, 0.8f);
                // Visualise movement path
                if (i > 0)
                {
                    Handles.DrawLine(platform.MovementPoints[i - 1], platform.MovementPoints[i]);

                    Handles.ArrowHandleCap(0, 
                        platform.MovementPoints[i - 1],
                        Quaternion.LookRotation(platform.MovementPoints[i] - platform.MovementPoints[i - 1], 
                        Vector3.forward),
                        1.2f, 
                        EventType.Repaint);
                }
                else if (platform.ClosedLoop)
                {
                    Handles.DrawLine(platform.MovementPoints[i], platform.MovementPoints[platform.MovementPoints.Length - 1]);

                    Handles.ArrowHandleCap(0,
                        platform.MovementPoints[platform.MovementPoints.Length - 1],
                        Quaternion.LookRotation(platform.MovementPoints[i] - platform.MovementPoints[platform.MovementPoints.Length - 1],
                        Vector3.forward),
                        1.2f,
                        EventType.Repaint);
                }
                Handles.DrawSolidDisc(platform.MovementPoints[i], Vector3.back, 0.2f);

                // Handle path movement in the editor scene view
                Vector2 newPoint = Handles.PositionHandle(platform.MovementPoints[i], Quaternion.identity);

                if (Event.current.shift)
                {
                    if (Vector2.Distance(newPoint, platform.MovementPoints[i]) > 0f)
                    {
                        int nearestXIndex = -1;
                        int nearestYIndex = -1;
                        float nearestX = float.MaxValue;
                        float nearestY = float.MaxValue;
                        for (int index = 0; index < platform.MovementPoints.Length; index++)
                        {
                            if (index == i) continue;
                            var dist = newPoint - platform.MovementPoints[index];
                            if (Mathf.Abs(dist.x) < Mathf.Abs(nearestX))
                            {
                                nearestX = dist.x;
                                nearestXIndex = index;
                            }
                            if (Mathf.Abs(dist.y) < Mathf.Abs(nearestY))
                            {
                                nearestY = dist.y;
                                nearestYIndex = index;
                            }
                        }

                        float threshold = 0.2f;
                        if (Mathf.Abs(nearestX) < threshold && Mathf.Abs(nearestY) < threshold && nearestXIndex >=0 && nearestYIndex >= 0)
                        {
                            newPoint.x = platform.MovementPoints[nearestXIndex].x;
                            newPoint.y = platform.MovementPoints[nearestYIndex].y;
                        }
                        if (Mathf.Abs(nearestY) < Mathf.Abs(nearestX) && nearestYIndex >= 0)
                        {
                            newPoint.y = platform.MovementPoints[nearestYIndex].y;
                        }
                        else if (nearestXIndex >= 0)
                        {
                            newPoint.x = platform.MovementPoints[nearestXIndex].x;
                        }
                    }
                }

                platform.MovementPoints[i] = newPoint;
            }
        }
    }
}
