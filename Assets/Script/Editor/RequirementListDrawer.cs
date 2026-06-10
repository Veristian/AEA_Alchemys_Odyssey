using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(RequirementList))]
public class RequirementListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty listProp = property.FindPropertyRelative("requirements");

        EditorGUI.PropertyField(position, listProp, label, true);

        // Detect right click
        Event e = Event.current;
        if (e.type == EventType.ContextClick && position.Contains(e.mousePosition))
        {
            ShowMenu(listProp);
            e.Use();
        }
    }

    private void ShowMenu(SerializedProperty listProp)
    {
        GenericMenu menu = new GenericMenu();

        var types = TypeCache.GetTypesDerivedFrom<Requirements>();

        foreach (var type in types)
        {
            // Skip abstract / generic types
            if (type.IsAbstract || type.IsGenericType)
                continue;

            string path = $"Add/{type.Name}";

            menu.AddItem(new GUIContent(path), false, () =>
            {
                var instance = Activator.CreateInstance(type) as Requirements;

                // Optional: set defaults
                ApplyDefaults(instance);

                Add(listProp, instance);
            });
        }

        menu.ShowAsContext();
    }

    private void Add(SerializedProperty list, Requirements instance)
    {
        list.arraySize++;
        var element = list.GetArrayElementAtIndex(list.arraySize - 1);

        element.managedReferenceValue = instance;
        list.serializedObject.ApplyModifiedProperties();
    }

    // 🔥 Optional: handle default values per type
    private void ApplyDefaults(Requirements instance)
    {
        switch (instance)
        {
            case DaysRequirement d:
                d.minimumDaysPassed = 1;
                break;

            case QuestCompletionRequirement q:
                q.requiredQuestId = null;
                break;

            case QuestPotionsRequirement p:
                p.requiredPotion = null;
                p.requiredIngredient = null;
                break;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(
            property.FindPropertyRelative("requirements"),
            label,
            true
        );
    }
}