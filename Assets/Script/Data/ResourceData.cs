using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResourceData : ScriptableObject, ILoadable
{
    [Header("Load Setting")]
    public LoadState loadState;

    public LoadState LoadState => loadState;


}
