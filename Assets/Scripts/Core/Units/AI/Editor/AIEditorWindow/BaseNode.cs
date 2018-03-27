using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Aspekt.AI.Editor
{
    public abstract class BaseNode
    {
        private int id;
        private List<NodeInterface> interfaces = new List<NodeInterface>();

        private NodeWindow nodeWindow;
        private BaseEditor parentEditor;

        protected bool isActive;
        protected string title = "Untitled";

        public string Name { get { return title; } }
        public int ID { get { return id; } }
        public List<NodeInterface> GetInterfaces() { return interfaces; }

        public BaseNode()
        {
            nodeWindow = new NodeWindow();
            SetupNode();
        }

        protected abstract void SetupNode();
        protected abstract string GetNodeType();

        public void SetPosition(Vector2 position)
        {
            nodeWindow.SetPosition(position);
        }

        public void SetSize(Vector2 size)
        {
            nodeWindow.Size = size;
        }

        public Vector2 GetSize() { return nodeWindow.Size; }

        public void Draw(Vector2 canvasOffset)
        {
            // TODO nodeRuntimeIndicator (border etc)
            Rect nodeRect = nodeWindow.GetNodeRect(canvasOffset);
            
            GUI.skin = (GUISkin)EditorGUIUtility.Load("NodeEditorWindowSkin.guiskin");
            GUI.skin.box.normal.background = GetWindowTexture();
            GUI.skin.box.border = new RectOffset(8, 8, 22, 8);
            GUI.skin.box.normal.textColor = new Color(.9f, .9f, .9f, 1f);
            GUI.skin.box.fontStyle = FontStyle.Bold;
            GUI.Box(nodeRect, title);

            GUI.BeginGroup(nodeRect);
            AIGUI.SetWindow(nodeRect.size);
            DrawInterfaces();
            DrawContent();
            GUI.EndGroup();
        }

        protected abstract void DrawContent();

        protected Vector2 position
        {
            get { return nodeWindow.Position; }
        }

        private void DrawInterfaces()
        {
            foreach (NodeInterface iface in interfaces)
            {
                iface.Draw();
            }
        }

        private Texture2D GetWindowTexture()
        {
            string textureName = string.Format("AspektAI/{0}Window{1}.png", GetNodeType(), isActive ? "Selected" : "Normal");
            return (Texture2D)EditorGUIUtility.Load(textureName);
        }


    }
}
