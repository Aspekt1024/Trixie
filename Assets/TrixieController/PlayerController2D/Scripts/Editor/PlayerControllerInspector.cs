using System.Linq;

using UnityEngine;
using UnityEditor;
using Aspekt.PlayerController;
using static Aspekt.IO.ControllerInputHandler;
using System.Reflection;
using System;

namespace Aspekt.IO.Edit
{
    [CustomEditor(typeof(PlayerController))]
    public class Aspekt : Editor
    {
        private PlayerController controller;

        string[] classStrings;
        private bool isAddingButton = false;
        private Keybinding newKeybinding;
        private int bindingIndex;

        private int getKeycodeIndex = -1;
        
        public override void OnInspectorGUI()
        {
            controller = (PlayerController)target;

            CheckKeypresses(Event.current);
            ShowExistingBindings();
            EditorGUILayout.Space();
            ShowAddUI();
        }

        private void CheckKeypresses(Event e)
        {
            if (getKeycodeIndex >= 0)
            {
                if (e.isKey || e.isMouse)
                {
                    bool isValidKey = false;
                    bool isInvalidKey = false;
                    KeyCode code = KeyCode.Escape;
                    switch (e.type)
                    {
                        case EventType.MouseDown:
                            isValidKey = e.button == 0 || e.button == 1;
                            if (e.button == 0) code = KeyCode.Mouse0;
                            if (e.button == 1) code = KeyCode.Mouse1;
                            break;
                        case EventType.KeyDown:
                            code = e.keyCode;
                            isInvalidKey = code == KeyCode.Escape;
                            isValidKey = !isInvalidKey;
                            break;
                    }

                    if (isValidKey)
                    {
                        var binding = controller.KeyBindings[getKeycodeIndex];
                        binding.KeyboardBinding = code;
                        controller.KeyBindings[getKeycodeIndex] = binding;
                    }

                    if (isValidKey || isInvalidKey)
                    {
                        getKeycodeIndex = -1;
                        EditorUtility.SetDirty(controller);
                    }
                }
            }
        }

        private void ShowExistingBindings()
        {
            for (int i = 0; i < controller.KeyBindings.Count; i++)
            {
                var binding = controller.KeyBindings[i];

                EditorGUILayout.BeginHorizontal();

                string className = binding.HandlerTypeString.Split('.').Last();
                EditorGUILayout.LabelField(className);

                if (getKeycodeIndex == i)
                {
                    EditorGUILayout.LabelField("press a key");
                }
                else
                {
                    if (GUILayout.Button(binding.KeyboardBinding.ToString()))
                    {
                        getKeycodeIndex = i;
                    }
                }

                var selectedBindings = controller.KeyBindings.Select(b => b.ControllerBinding.ToString()).ToList();
                var availableBindings = Enum.GetNames(typeof(BindableButtons)).Where(b => controller.KeyBindings[i].ControllerBinding.ToString().Equals(b) || !selectedBindings.Contains(b)).ToArray();

                int currentIndex = Array.IndexOf(availableBindings, controller.KeyBindings[i].ControllerBinding.ToString());
                if (currentIndex < 0) currentIndex = 0;

                var bindingIndex = EditorGUILayout.Popup(currentIndex, availableBindings);
                if (bindingIndex != currentIndex)
                {
                    binding.ControllerBinding = (BindableButtons)Enum.Parse(typeof(BindableButtons), availableBindings[bindingIndex]);
                    EditorUtility.SetDirty(controller);
                }

                controller.KeyBindings[i] = binding;

                EditorGUILayout.EndHorizontal();
            }
        }

        private void ShowAddUI()
        {
            if (isAddingButton)
            {
                DisplayNewBinding();
            }
            else if (GUILayout.Button("Add Button"))
            {
                isAddingButton = true;
                newKeybinding = new Keybinding();
                bindingIndex = 0;

                var classes = Assembly.GetAssembly(typeof(PlayerControllerButtonHandler)).GetTypes()
                    .Where(myType => myType.IsClass && myType.IsSubclassOf(typeof(PlayerControllerButtonHandler))).ToArray();

                classStrings = classes.Select(c => c.ToString()).ToArray();
            }
        }

        private void DisplayNewBinding()
        {
            var displayArray = classStrings.Select(c => c.Split('.').Last()).ToArray(); // want to keep full path for instantiation
            bindingIndex = EditorGUILayout.Popup(bindingIndex, displayArray);
            newKeybinding.HandlerTypeString = classStrings[bindingIndex];
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                controller.KeyBindings.Add(newKeybinding);
                isAddingButton = false;
            }
            else if (GUILayout.Button("Cancel"))
            {
                isAddingButton = false;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
