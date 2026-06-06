using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[Serializable]
public class ResourceData : ScriptableObject, ILoadable
{
    [Header("Load Setting")]
    public LoadState loadState;

    public LoadState LoadState => loadState;
    [Header("Quest")]
    public string questId;
}
