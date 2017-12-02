using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GravityField : MonoBehaviour {

    public float Strength;

    private SpriteRenderer r;

    private void Start()
    {
        r = GetComponent<SpriteRenderer>();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Strength > 0)
        {
            r.color = new Color(0.4f, 1f, .4f, 0.5f);
        }
        else
        {
            r.color = new Color(0.6f, 0.7f, 1f, 0.4f);
        }
    }
#endif

}
