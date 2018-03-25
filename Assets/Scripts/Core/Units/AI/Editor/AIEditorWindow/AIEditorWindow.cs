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
        }

        protected override void SetTheme()
        {
            // TODO set image titleContent.image = (Texture)Resournces.Load("imagelocation");
            titleContent.text = "Aspekt AI";

        }

    }
}
