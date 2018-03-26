using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Aspekt.AI.Editor
{
    public class NodeWindow
    {
        private Rect windowRect;
        
        public Vector2 Size
        {
            get { return windowRect.size; }
            set { windowRect = new Rect(windowRect.position, value); }
        }

        public Rect GetNodeRect(Vector2 canvasOffset)
        {
            windowRect.position = SnapToGrid(windowRect.position);

            return windowRect;
        }
        
        public void SetPosition(Vector2 position)
        {
            windowRect = new Rect(position, windowRect.size);
        }
        
        private Vector2 SnapToGrid(Vector2 position)
        {
            return new Vector2
            {
                x = AIGUI.GRID_SPACING * Mathf.Round(position.x / AIGUI.GRID_SPACING),
                y = AIGUI.GRID_SPACING * Mathf.Round(position.y / AIGUI.GRID_SPACING)
            };
        }
    }


}
