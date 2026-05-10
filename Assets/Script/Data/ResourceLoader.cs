using UnityEngine;
using System;
using System.Collections.Generic;
public enum LoadState
{
    Enabled,
    Disabled,
    DebugOnly
}
public interface ILoadable
{
    LoadState LoadState { get; }
}

public static class ResourceLoader
{
    private static Dictionary<Type, List<ScriptableObject>> database
        = new Dictionary<Type, List<ScriptableObject>>();

    private static HashSet<Type> loadedTypes = new HashSet<Type>();

    public static void Load<T>(string path) where T : ScriptableObject
    {
        Type type = typeof(T);

        if (loadedTypes.Contains(type))
            return;

        var assets = Resources.LoadAll<T>(path);

        var list = new List<ScriptableObject>();

        foreach (var asset in assets)
        {
            if (asset is ILoadable loadable)
            {
                if (!ShouldLoad(loadable.LoadState))
                    continue;
            }

            list.Add(asset);
        }

        database[type] = list;
        loadedTypes.Add(type);
    }

    public static IReadOnlyList<T> GetAll<T>(string path) where T : ScriptableObject
    {
        Load<T>(path);

        var type = typeof(T);
        var rawList = database[type];

        List<T> result = new List<T>();

        foreach (var obj in rawList)
            result.Add(obj as T);

        return result;
    }

    private static bool ShouldLoad(LoadState state)
    {
        switch (state)
        {
            case LoadState.Enabled:
                return true;

            case LoadState.Disabled:
                return false;

            case LoadState.DebugOnly:
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            default:
                return false;
        }
    }
}