using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using Aspekt.PlayerController;

namespace TrixieCore.Edit
{
    [CustomEditor(typeof(Trixie))]
    public class TrixieInspector : Editor
    {
        private PlayerState state;

        public override void OnInspectorGUI()
        {
            state = ((Trixie)target).GetPlayerState();
            if (state == null) return;

            var labels = (StateLabels[])Enum.GetValues(typeof(StateLabels));
            foreach (var label in labels)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label.ToString());
                EditorGUILayout.LabelField(state.GetValue(label)?.ToString());
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
