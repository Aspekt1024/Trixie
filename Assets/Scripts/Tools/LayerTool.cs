using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using TrixieCore;

/// <summary>
/// Handler for auto sorting layers and reporting which objects are not assigned a layer
/// </summary>
public class LayerTool : MonoBehaviour
{
}

[CustomEditor(typeof(LayerTool))]
public class LayerToolInspector : Editor
{
    private readonly Dictionary<SortingLayers, float> zLayers = new Dictionary<SortingLayers, float>();
    private List<string> errors = new List<string>();

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Sort"))
        {
            SetupLayerDict();
            Sort();
        }

        if (errors.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sorting Layer Issues:");
            foreach (var error in errors)
            {
                EditorGUILayout.LabelField(error);
            }
        }
    }

    private void Sort()
    {
        errors = new List<string>();

        float currentIndex = 0f;
        int highestOrderInLayer = 0;
        SortingLayers currentLayer = SortingLayers.UI;
        
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        Renderer[] validRenderers = allRenderers.Where(x => System.Enum.TryParse(x.sortingLayerName, out SortingLayers l)).ToArray();
        Renderer[] invalidRenderers = allRenderers.Where(x => !System.Enum.TryParse(x.sortingLayerName, out SortingLayers l)).ToArray();

        foreach (var renderer in invalidRenderers)
        {
            errors.Add("Invalid layer '" + renderer.sortingLayerName + "': " + GetObjectPath(renderer.gameObject));
        }

        Renderer[] renderers = validRenderers
            .OrderByDescending(x => x.sortingOrder)
            .OrderBy(x => zLayers[(SortingLayers)System.Enum.Parse(typeof(SortingLayers), x.sortingLayerName)])
            .ToArray();

        foreach (var renderer in renderers)
        {
            if (renderer.sortingLayerName == SortingLayers.Default.ToString())
            {
                errors.Add("Invalid layer 'Default': " + GetObjectPath(renderer.gameObject));
            }
            else
            {
                SortingLayers layer = (SortingLayers)System.Enum.Parse(typeof(SortingLayers), renderer.sortingLayerName);
                if (layer != currentLayer)
                {
                    currentLayer = layer;
                    currentIndex += highestOrderInLayer + 2f;
                    highestOrderInLayer = renderer.sortingOrder;
                }
                var pos = renderer.transform.position;
                pos.z = currentIndex + highestOrderInLayer - renderer.sortingOrder;
                renderer.transform.position = pos;
            }
        }
    }

    private string GetObjectPath(GameObject obj)
    {
        string path = obj.name;
        var parent = obj.transform.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.transform.parent;
        }
        return path;
    }

    private void SetupLayerDict()
    {
        SortingLayers[] layers = (SortingLayers[])System.Enum.GetValues(typeof(SortingLayers));

        float zValue = 0f;
        const float spacing = 2f;

        for (int i = layers.Length - 1; i >= 0; i--)
        {
            if (zLayers.ContainsKey(layers[i]))
            {
                zLayers[layers[i]] = zValue;
            }
            else
            {
                zLayers.Add(layers[i], zValue);
            }
            zValue += spacing;
        }
    }
}
