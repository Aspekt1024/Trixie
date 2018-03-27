using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Aspekt.AI.Editor
{
    public abstract class BaseEditor : EditorWindow
    {
        private Texture2D background;
        private List<BaseNode> nodes = new List<BaseNode>();


        public virtual void LoadEditor()
        {
            SetTheme();
        }

        protected abstract void SetTheme();
        protected abstract void GUIUpdate();

        private void OnGUI()
        {
            DrawBackground();
            DrawNodes();
            GUIUpdate();
        }

        private void Update()
        {
            Repaint();
        }

        public void AddNode(BaseNode node)
        {
            nodes.Add(node);
        }

        public void RemoveNode(BaseNode node)
        {
            nodes.Remove(node);
        }

        public List<BaseNode> GetNodes()
        {
            return nodes;
        }

        private void DrawBackground()
        {
            if (background == null)
            {
                background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                background.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f, 1f));
                background.Apply();
            }
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), background, ScaleMode.StretchToFill);
            DrawGrid(250, new Color(1f, 1f, 1f, 0.4f));
            DrawGrid(50, new Color(1f, 1f, 1f, 0.1f));
            DrawGrid(10, new Color(1f, 1f, 1f, 0.03f));
        }

        private void DrawNodes()
        {
            foreach (var node in nodes)
            {
                node.Draw(Vector2.zero);
            }
        }

        private void DrawGrid(float gridSpacing, Color gridColour)
        {
            Handles.BeginGUI();
            Handles.color = gridColour;

            Vector2 CanvasOffset = Vector2.zero;
            Vector2 CanvasDrag = Vector2.zero;

            float hPos = (CanvasOffset.x + CanvasDrag.x) % gridSpacing;
            float vPos = (CanvasOffset.y + CanvasDrag.y) % gridSpacing;

            while (hPos < position.width)
            {
                Handles.DrawLine(new Vector3(hPos, 0, 0), new Vector3(hPos, position.height, 0));
                hPos += gridSpacing;
            }

            while (vPos < position.height)
            {
                Handles.DrawLine(new Vector3(0, vPos, 0), new Vector3(position.width, vPos, 0));
                vPos += gridSpacing;
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

    }
}

