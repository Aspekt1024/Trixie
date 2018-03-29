using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrixieCore;

public class LevelGenerator : MonoBehaviour {

    [System.Serializable]
    public struct GroundPrefabs
    {
        public GroundTypes GroundType;
        public GameObject Prefab;
    }

    public enum GroundTypes
    {
        Surface, Roof, Middle, Island
    }

    public Texture2D LevelMap;
    public ColourToPrefab[] PrefabMap;
    public GroundPrefabs[] GroundMap;

    private bool playerPlaced;
    private Vector2 startPos;
    private Color currentPixelColor;

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        if (LevelMap == null) return;

        startPos = new Vector2(-17.5f, -9.5f) * LevelGrid.GetGridSpacing();

        for (int x = 0; x < LevelMap.width; x++)
        {
            for (int y = 0; y < LevelMap.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    private void GenerateTile(int x, int y)
    {
        currentPixelColor = LevelMap.GetPixel(x, y);
        if (currentPixelColor == Color.white) return;
        
        if (currentPixelColor == Color.blue && !playerPlaced)
        {
            playerPlaced = true;
            Player.Instance.transform.position = startPos + new Vector2(x + 0.5f, y + 1) * LevelGrid.GetGridSpacing();
        }
        else if (currentPixelColor == Color.black)
        {
            if (LevelMap.GetPixel(x, y + 1) != Color.black && LevelMap.GetPixel(x, y - 1) != Color.black)
            {
                GenerateGroundPrefab(x, y, GroundTypes.Island);
            }
            else if (LevelMap.GetPixel(x, y + 1) != Color.black)
            {
                GenerateGroundPrefab(x, y, GroundTypes.Surface);
            }
            else if (LevelMap.GetPixel(x, y - 1) != Color.black)
            {
                GenerateGroundPrefab(x, y, GroundTypes.Roof);
            }
            else
            {
                GenerateGroundPrefab(x, y, GroundTypes.Middle);
            }
        }
        else if (currentPixelColor == Color.green)
        {

        }
        else
        {
            foreach (ColourToPrefab c2p in PrefabMap)
            {
                if (c2p.Colour.Equals(currentPixelColor))
                {
                    InstantiatePrefab(c2p.Prefab, x, y);
                }
            }
        }


    }

    private void GenerateGroundPrefab(int x, int y, GroundTypes type)
    {
        foreach (GroundPrefabs groundPrefab in GroundMap)
        {
            if (groundPrefab.GroundType == type)
            {
                InstantiatePrefab(groundPrefab.Prefab, x, y);
            }
        }
    }

    private void InstantiatePrefab(GameObject prefab, int x, int y)
    {
        Instantiate(prefab, startPos + new Vector2(x, y) * LevelGrid.GetGridSpacing(), Quaternion.identity);
    }


}
