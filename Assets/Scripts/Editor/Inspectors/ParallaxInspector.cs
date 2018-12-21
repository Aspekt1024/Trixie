using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Parallax))]
public class ParallaxInspector : Editor {

    private Parallax parallax;

    private void OnEnable()
    {
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Tools.hidden = false;
    }

    private void OnSceneGUI()
    {
        parallax = (Parallax)target;
        Vector2 pos = (Vector2)parallax.MainCamera.transform.position * parallax.Distance;

        parallax.Offset = Handles.PositionHandle(parallax.transform.position - (Vector3)pos, Quaternion.identity);
        parallax.transform.position = parallax.Offset + pos;
    }


}
