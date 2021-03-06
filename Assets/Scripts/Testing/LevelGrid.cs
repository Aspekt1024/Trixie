﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelGrid : MonoBehaviour
{
    public Color GridColor;
    public float GridSpacing;

    public static LevelGrid Instance;

    private Texture2D texture;

    private Vector2 worldCenter;

    public static float GetGridSpacing()
    {
        return Instance.GridSpacing;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple LevelGrids found in scene. There should only be one!");
            Destroy(gameObject);
        }

        LevelConfigData levelConfig = ConfigHandler.LoadConfig();
        if (levelConfig != null)
        {
            GridSpacing = levelConfig.GridSize;
        }

        SetTexture();
    }
    
    private void OnGUI()
    {
        CheckGridSpacing();

        int gridNumX = Mathf.RoundToInt(Camera.main.ViewportToWorldPoint(Vector2.zero).x / GridSpacing);
        int gridNumY = Mathf.RoundToInt(Camera.main.ViewportToWorldPoint(Vector2.zero).y / GridSpacing);
        
        while (Camera.main.WorldToViewportPoint(Vector2.right * GridSpacing * gridNumX).x < 1f)
        {
            Vector2 pos = new Vector2(GridSpacing * gridNumX, 0f);
            pos.x = Camera.main.WorldToScreenPoint(pos).x;
            pos.y = 0f;

            Vector2 size = new Vector2(1, Screen.height);

            Rect line = new Rect(pos, size);

            GUI.DrawTexture(line, texture);
            gridNumX++;
        }
        
        while (Camera.main.WorldToViewportPoint(Vector2.up * GridSpacing * gridNumY).y < 1f)
        {
            Vector2 pos = new Vector2(0f, GridSpacing * gridNumY);
            pos.y = Screen.height - Camera.main.WorldToScreenPoint(pos).y;

            Vector2 size = new Vector2(Screen.width, 1);

            Rect line = new Rect(pos, size);

            GUI.DrawTexture(line, texture);
            gridNumY++;
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        CheckGridSpacing();

        Gizmos.color = GridColor;
        Camera sceneCam = UnityEditor.SceneView.currentDrawingSceneView.camera;

        int gridNumX = Mathf.RoundToInt(sceneCam.ViewportToWorldPoint(Vector2.zero).x / GridSpacing);
        int gridNumY = Mathf.RoundToInt(sceneCam.ViewportToWorldPoint(Vector2.zero).y / GridSpacing);

        while (sceneCam.WorldToViewportPoint(Vector2.right * GridSpacing * gridNumX).x < 1f)
        {
            Vector3 startPos = new Vector3(GridSpacing * gridNumX, sceneCam.transform.position.y - Screen.height * sceneCam.orthographicSize / 2);
            Vector3 endPos = startPos + Vector3.up * Screen.height * sceneCam.orthographicSize;

            Gizmos.DrawLine(startPos, endPos);
            gridNumX++;
        }
        
        while (sceneCam.WorldToViewportPoint(Vector2.up * GridSpacing * gridNumY).y < 1f)
        {
            Vector3 startPos = new Vector3(sceneCam.transform.position.x - Screen.width * sceneCam.orthographicSize / 2, GridSpacing * gridNumY);
            Vector3 endPos = startPos + Vector3.right * Screen.width * sceneCam.orthographicSize;

            Gizmos.DrawLine(startPos, endPos);
            gridNumY++;
        }
    }

    public void GetCellCenter(Vector3 pos)
    {

    }
#endif


    private void SetTexture()
    {
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, GridColor);
        texture.Apply();
    }

    private void CheckGridSpacing()
    {
        if (GridSpacing < 0.5f)
        {
            GridSpacing = 0.5f;
        }
    }
}
