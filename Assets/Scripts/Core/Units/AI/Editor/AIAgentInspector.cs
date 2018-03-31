using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using Aspekt.AI;

[CustomEditor(typeof(AIAgent))]
public class AIAgentInspector : Editor {

    private AIAgent agent;

    private int selectedGoalIndex;
    private int selectedActionIndex;

    public override void OnInspectorGUI()
    {
        agent = (AIAgent)target;

        SetSensorsObject();

        if (EditorGUILayout.Foldout(true, "Agent Profile"))
        {
            DisplayGoals();
            EditorGUILayout.Space();
            DisplayActions();
        }

        EditorGUILayout.Separator();
        EditorGUIUtility.labelWidth = 0;
        agent.LoggingEnabled = EditorGUILayout.Toggle(new GUIContent("Logging Enabled", "Enable to show logging of the AI"), agent.LoggingEnabled);
    }

    private void DisplayGoals()
    {
        if (agent.GoalsObject == null)
        {
            agent.GoalsObject = ObtainChildObject("Goals");
        }

        AIGoal[] goals = agent.GoalsObject.GetComponents<AIGoal>();

        EditorGUILayout.LabelField("Goals:", EditorStyles.boldLabel);
        if (goals == null || goals.Length == 0)
        {
            EditorGUILayout.HelpBox("You should select at least one goal in order for the AIAgent to do anything!", MessageType.Warning);
        }
        else
        {
            AIGoal goalToRemove = null;
            for (int i = 0; i < goals.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 50f;
                EditorGUILayout.LabelField(goals[i].ToString());

                goals[i].Priority = Mathf.Clamp(EditorGUILayout.FloatField("Priority", goals[i].Priority), 0, float.MaxValue);

                if (GUILayout.Button("Remove"))
                {
                    goalToRemove = goals[i];
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            if (goalToRemove != null)
            {
                DestroyImmediate(goalToRemove);
            }
        }

        EditorGUILayout.BeginHorizontal();
        
        System.Type[] allTypes = (from System.Type type in Assembly.GetAssembly(typeof(AIGoal)).GetTypes() where type.IsSubclassOf(typeof(AIGoal)) select type).ToArray();
        string[] allTypesStringArray = (from System.Type type in allTypes select type.ToString()).ToArray();

        selectedGoalIndex = EditorGUILayout.Popup(selectedGoalIndex, allTypesStringArray);
        if (GUILayout.Button("Add selected goal"))
        {
            if (!ComponentExists(goals, allTypes[selectedGoalIndex]))
            {
                AIGoal newGoal = (AIGoal)agent.GoalsObject.AddComponent(allTypes[selectedGoalIndex]);
                newGoal.Priority = 1;
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void DisplayActions()
    {
        if (agent.ActionsObject == null)
        {
            agent.ActionsObject = ObtainChildObject("Actions");
        }

        AIAction[] actions = agent.ActionsObject.GetComponents<AIAction>();

        EditorGUILayout.LabelField("Actions:", EditorStyles.boldLabel);
        if (actions == null || actions.Length == 0)
        {
            EditorGUILayout.HelpBox("Without any actions, the AIAgent won't do anything!", MessageType.Warning);
        }
        else
        {
            AIAction actionToRemove = null;
            for (int i = 0; i < actions.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 50f;
                EditorGUILayout.LabelField(actions[i].ToString());

                actions[i].Cost = Mathf.Clamp(EditorGUILayout.FloatField("Cost", actions[i].Cost), 0, float.MaxValue);

                if (GUILayout.Button("Remove"))
                {
                    actionToRemove = actions[i];
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            if (actionToRemove != null)
            {
                DestroyImmediate(actionToRemove);
            }
        }

        EditorGUILayout.BeginHorizontal();

        System.Type[] allTypes = (from System.Type type in Assembly.GetAssembly(typeof(AIAction)).GetTypes() where type.IsSubclassOf(typeof(AIAction)) select type).ToArray();
        string[] allTypesStringArray = (from System.Type type in allTypes select type.ToString()).ToArray();

        selectedActionIndex = EditorGUILayout.Popup(selectedActionIndex, allTypesStringArray);
        if (GUILayout.Button("Add selected action"))
        {
            if (!ComponentExists(actions, allTypes[selectedActionIndex]))
            {
                AIAction newAction = (AIAction)agent.ActionsObject.AddComponent(allTypes[selectedActionIndex]);
                newAction.Cost = 1;
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private GameObject ObtainChildObject(string objectName)
    {
        Transform[] children = agent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == objectName)
            {
                return children[i].gameObject;
            }
        }

        GameObject childObject = new GameObject(objectName);
        childObject.transform.SetParent(agent.transform);

        return childObject;
    }
    
    private bool ComponentExists(object[] existingObjects, System.Type newType)
    {
        bool objectExists = false;
        for (int i = 0; i < existingObjects.Length; i++)
        {
            if (existingObjects[i].GetType() == newType)
            {
                objectExists = true;
                break;
            }
        }
        return objectExists;
    }

    private void SetSensorsObject()
    {
        if (agent.SensorsObject == null)
        {
            agent.SensorsObject = ObtainChildObject("Sensors");
        }
    }
}
