using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class TilingBackground : MonoBehaviour {

    public float ScaleMultiplier = 1f;
    public Sprite BackgroundSprite;
    public bool RequiresMirror = false;
    public int NumTiles = 3;

    private Transform[] backgrounds;
    private float sizeX;
    
    private void Start ()
    {
        sizeX = BackgroundSprite.bounds.size.x;
        
        backgrounds = new Transform[NumTiles];
        bool flip = false;
        for (int i = 0; i < NumTiles; i++)
        {
            backgrounds[i] = CreateNewBackground((i - 1) * sizeX * ScaleMultiplier);

            if (RequiresMirror)
            {
                if (flip)
                {
                    backgrounds[i].localScale = new Vector3(-backgrounds[i].localScale.x, backgrounds[i].localScale.y, 1f);
                }
                flip = !flip;
            }

        }
    }

    private void Update()
    {
        if (backgrounds[0].position.x < Camera.main.transform.position.x - (NumTiles / 2f + 0.1f) * sizeX * ScaleMultiplier)
        {
            MoveLeftToRight();
        }
        else if (backgrounds[NumTiles - 1].position.x > Camera.main.transform.position.x + (NumTiles / 2f + 0.1f) * sizeX * ScaleMultiplier)
        {
            MoveRightToLeft();
        }
    }

    private Transform CreateNewBackground(float xPos)
    {
        GameObject newBg = new GameObject();
        newBg.transform.SetParent(transform);
        newBg.transform.localScale = new Vector3(ScaleMultiplier, ScaleMultiplier, 1f);
        newBg.transform.localPosition = new Vector2(xPos, 0f);

        var sr = newBg.AddComponent<SpriteRenderer>();
        sr.sprite = BackgroundSprite;
        sr.sortingLayerName = SortingLayers.Background.ToString();

        var rb = newBg.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        return newBg.transform;
    }

    private void MoveLeftToRight()
    {
        backgrounds[0].position += Vector3.right * sizeX * ScaleMultiplier * NumTiles;
        if (RequiresMirror)
        {
            backgrounds[0].localScale = new Vector3(-backgrounds[NumTiles-1].localScale.x, backgrounds[0].localScale.y, 1f);
        }

        Transform[] newBackgrounds = new Transform[NumTiles];

        newBackgrounds[NumTiles - 1] = backgrounds[0];
        for (int i = 0; i < NumTiles - 1; i++)
        {
            newBackgrounds[i] = backgrounds[i + 1];
        }
        backgrounds = newBackgrounds;
    }

    private void MoveRightToLeft()
    {
        backgrounds[NumTiles - 1].position -= Vector3.right * sizeX * ScaleMultiplier * NumTiles;
        if (RequiresMirror)
        {
            backgrounds[NumTiles - 1].localScale = new Vector3(-backgrounds[0].localScale.x, backgrounds[0].localScale.y, 1f);
        }

        Transform[] newBackgrounds = new Transform[NumTiles];

        newBackgrounds[0] = backgrounds[NumTiles - 1];
        for (int i = 1; i < NumTiles; i++)
        {
            newBackgrounds[i] = backgrounds[i - 1];
        }
        backgrounds = newBackgrounds;
    }
}
