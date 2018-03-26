using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Aspekt.AI.Editor
{
    public class AIEditorWindow : BaseEditor
    {
        [MenuItem("Window/Aspekt AI")]
        private static void ShowEditor()
        {
            AIEditorWindow editor = GetWindow<AIEditorWindow>();
            editor.LoadEditor();
            editor.CreateNodes(editor);
        }

        protected override void SetTheme()
        {
            // TODO set image titleContent.image = (Texture)Resournces.Load("imagelocation");
            titleContent.text = "Aspekt AI";

        }

        private void CreateNodes(AIEditorWindow editor)
        {
            AINode newNode = new AINode();
            newNode.SetPosition(new Vector2(10, 10));
            editor.AddNode(newNode);
        }

    }
}
