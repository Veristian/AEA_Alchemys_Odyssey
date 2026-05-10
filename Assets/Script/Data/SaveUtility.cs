using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
[Serializable]
public class DataWrapper
{
    public string type;
    public string data;
}
public static class SaveUtility
{
    private static Dictionary<string, Type> cachedTypes = new Dictionary<string, Type>();

    static SaveUtility()
    {
        CacheAllMetadataTypes();
    }

    private static void CacheAllMetadataTypes()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<MetadataAttribute>();
                if (attr != null)
                {
                    cachedTypes[attr.typeId] = type;
                }
            }
        }
    }

    // 🔹 Serialize ANY object
    public static string Serialize(object obj)
    {
        Type type = obj.GetType();

        string typeId = GetTypeId(type);

        DataWrapper wrapper = new DataWrapper
        {
            type = typeId,
            data = JsonUtility.ToJson(obj)
        };

        return JsonUtility.ToJson(wrapper);
    }

    // 🔹 Deserialize back into correct type
    public static object Deserialize(string json)
    {
        DataWrapper wrapper = JsonUtility.FromJson<DataWrapper>(json);

        Type type = ResolveType(wrapper.type);
        if (type == null)
        {
            Debug.LogError($"Unknown type: {wrapper.type}");
            return null;
        }
        
        return JsonUtility.FromJson(wrapper.data, type);
    }

    // 🔹 Generic version (safe cast)
    public static T Deserialize<T>(string json)
    {
        return (T)Deserialize(json);
    }

    private static string GetTypeId(Type type)
    {
        var attr = type.GetCustomAttribute<MetadataAttribute>();
        if (attr != null)
            return attr.typeId;

        return type.FullName; // fallback
    }

    private static Type ResolveType(string typeId)
    {
        if (cachedTypes.TryGetValue(typeId, out var type))
            return type;

        // fallback if no metadata
        return Type.GetType(typeId);
    }
}