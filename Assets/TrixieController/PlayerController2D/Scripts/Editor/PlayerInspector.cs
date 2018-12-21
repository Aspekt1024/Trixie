using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using Aspekt.PlayerController;

namespace Aspekt.PlayerController2D.Edit
{
    [CustomEditor(typeof(Player))]
    public class PlayerInspector : Editor
    {
        private PlayerState state;

        public override void OnInspectorGUI()
        {
            state = ((Player)target).GetPlayerState();
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
