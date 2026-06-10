// using UnityEditor;
// using UnityEngine;
// using System.Reflection;

// [CustomEditor(typeof(MonoBehaviour), true)]
// public class GenericButtonEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         // Draw normal inspector first
//         DrawDefaultInspector();

//         MonoBehaviour targetScript = (MonoBehaviour)target;
//         MethodInfo[] methods = targetScript.GetType()
//             .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

//         bool hasButtons = false;

//         foreach (MethodInfo method in methods)
//         {
//             // Only allow parameterless methods
//             if (method.GetParameters().Length != 0)
//                 continue;

//             // Skip Unity / inherited methods
//             if (method.IsSpecialName)
//                 continue;

//             if (!hasButtons)
//             {
//                 EditorGUILayout.Space();
//                 EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
//                 hasButtons = true;
//             }

//             if (GUILayout.Button(method.Name))
//             {
//                 method.Invoke(targetScript, null);
//             }
//         }
//     }
// }
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(MonoBehaviour), true)]
public class GenericButtonEditor : Editor
{
    private Dictionary<string, object[]> methodParams = new Dictionary<string, object[]>();
    private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MonoBehaviour targetScript = (MonoBehaviour)target;

        MethodInfo[] methods = targetScript.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        bool hasButtons = false;

        foreach (MethodInfo method in methods)
        {
            ParameterInfo[] parameters = method.GetParameters();

            if (method.IsSpecialName)
                continue;

            if (!AreAllParametersSupported(parameters))
                continue;

            string key = GetMethodKey(method);

            if (!foldouts.ContainsKey(key))
                foldouts[key] = false;

            if (!hasButtons)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
                hasButtons = true;
            }

            // Foldout header
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };

            foldouts[key] = EditorGUILayout.Foldout(foldouts[key], method.Name, true, foldoutStyle);

            if (foldouts[key])
            {
                EditorGUI.indentLevel++;

                // Initialize parameter storage
                if (!methodParams.ContainsKey(key))
                {
                    object[] defaults = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                        defaults[i] = GetDefault(parameters[i].ParameterType);

                    methodParams[key] = defaults;
                }

                object[] paramValues = methodParams[key];

                // Draw parameters
                for (int i = 0; i < parameters.Length; i++)
                {
                    paramValues[i] = DrawField(
                        parameters[i].Name,
                        paramValues[i],
                        parameters[i].ParameterType
                    );
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Invoke"))
                {
                    method.Invoke(targetScript, paramValues);
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
    }

    // ---------- Helpers ----------

    private string GetMethodKey(MethodInfo method)
    {
        return method.DeclaringType.FullName + "." + method.ToString();
    }

    private bool AreAllParametersSupported(ParameterInfo[] parameters)
    {
        foreach (var param in parameters)
        {
            if (!IsSupportedType(param.ParameterType))
                return false;
        }
        return true;
    }

    private bool IsSupportedType(Type type)
    {
        return
            type == typeof(int) ||
            type == typeof(float) ||
            type == typeof(string) ||
            type == typeof(bool) ||
            type == typeof(Vector3) ||
            type.IsEnum ||
            typeof(UnityEngine.Object).IsAssignableFrom(type);
    }

    private object DrawField(string label, object value, Type type)
    {
        if (type == typeof(int))
            return EditorGUILayout.IntField(label, (int)value);

        if (type == typeof(float))
            return EditorGUILayout.FloatField(label, (float)value);

        if (type == typeof(string))
            return EditorGUILayout.TextField(label, (string)value);

        if (type == typeof(bool))
            return EditorGUILayout.Toggle(label, (bool)value);

        if (type == typeof(Vector3))
            return EditorGUILayout.Vector3Field(label, (Vector3)value);

        // Enum support
        if (type.IsEnum)
        {
            if (value == null)
                value = GetDefault(type);

            // Flags enum support
            if (type.GetCustomAttribute<FlagsAttribute>() != null)
                return EditorGUILayout.EnumFlagsField(label, (Enum)value);

            return EditorGUILayout.EnumPopup(label, (Enum)value);
        }

        // Unity Object reference
        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            return EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, type, true);

        return value;
    }

    private object GetDefault(Type type)
    {
        if (type.IsEnum)
            return Enum.GetValues(type).GetValue(0);

        if (type.IsValueType)
            return Activator.CreateInstance(type);

        return null;
    }
}