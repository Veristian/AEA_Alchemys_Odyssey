using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class GenericButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw normal inspector first
        DrawDefaultInspector();

        MonoBehaviour targetScript = (MonoBehaviour)target;
        MethodInfo[] methods = targetScript.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        bool hasButtons = false;

        foreach (MethodInfo method in methods)
        {
            // Only allow parameterless methods
            if (method.GetParameters().Length != 0)
                continue;

            // Skip Unity / inherited methods
            if (method.IsSpecialName)
                continue;

            if (!hasButtons)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
                hasButtons = true;
            }

            if (GUILayout.Button(method.Name))
            {
                method.Invoke(targetScript, null);
            }
        }
    }
}
