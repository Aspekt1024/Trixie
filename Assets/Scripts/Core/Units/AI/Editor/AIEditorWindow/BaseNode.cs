using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Aspekt.AI.Editor
{
    public abstract class BaseNode
    {
        protected enum NodeTypes
        {
            Action, Goal, Unit
        }
        protected NodeTypes nodeType;

        private string name = "Untitled";
        private int id;
        private List<NodeInterface> interfaces = new List<NodeInterface>();
        private Rect windowRect;

        //TODO private NodeTransform transform;
        private BaseEditor parentEditor;

        private Rect nodeRect;

        public string Name { get { return name; } }
        public int ID { get { return id; } }
        public List<NodeInterface> GetInterfaces() { return interfaces; }

        public virtual void SetupNode()
        {

        }

        public void SetWindowRect(Rect rect)
        {
            windowRect = rect;
            // TODO set what needs to be
        }

        public void Draw(Vector2 canvasOffset)
        {
            // TODO nodeRuntimeIndicator (border etc)
            // TODO nodeRect = transform.GetWindow(canvasOffset);

            // TODO setup skin
            GUI.skin = (GUISkin)EditorGUIUtility.Load("NodeEditorWindowSkin.guiskin");
            GUI.skin.box.normal.background = GetWindowTexture();
            GUI.Box(nodeRect, name);

            GUI.BeginGroup(nodeRect);
            // TODO setup nodeGUI NodeGUI.SetWindow(nodeRect.size);
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
            string textureName = string.Format("{0}Window.png", nodeType.ToString());
            return (Texture2D)EditorGUIUtility.Load(textureName);
        }


    }
}
