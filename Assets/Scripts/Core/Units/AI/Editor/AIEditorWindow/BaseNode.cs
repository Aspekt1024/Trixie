using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Aspekt.AI.Editor
{
    public abstract class BaseNode
    {
        private string name = "Untitled";
        private int id;
        private List<NodeInterface> interfaces = new List<NodeInterface>();

        private NodeWindow nodeWindow;
        private BaseEditor parentEditor;
        
        public string Name { get { return name; } }
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
            GUI.Box(nodeRect, name);

            GUI.BeginGroup(nodeRect);
            AIGUI.SetWindow(nodeRect.size);
            DrawInterfaces();
            DrawContent();
            GUI.EndGroup();
        }

        protected abstract void DrawContent();

        private void DrawInterfaces()
        {
            foreach (NodeInterface iface in interfaces)
            {
                iface.Draw();
            }
        }

        private Texture2D GetWindowTexture()
        {
            string textureName = string.Format("AspektAI/{0}WindowSelected.png", GetNodeType());
            return (Texture2D)EditorGUIUtility.Load(textureName);
        }


    }
}
