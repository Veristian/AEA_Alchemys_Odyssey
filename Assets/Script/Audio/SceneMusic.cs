using JetBrains.Annotations;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Header("Audio Setting")]
    [Range(0.0f, 1.0f)]
    public float defaultVolume = 1f;

    [Header("First BGM")]
    public AudioClip bgm01;

    [Header("Second BGM")]
    public AudioClip bgm02;
    
}