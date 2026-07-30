using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private MeshRenderer visual;
    public enum MarkerType
    {
        NPC,
        Teleport
    }

    public MarkerType type;
    private bool isTracking;
    private Vector3 startPos;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material trackedMaterial;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        if (isTracking)
        {
            float wave = Mathf.Sin(Time.time * 6f);

            transform.localPosition = startPos + Vector3.up * wave * 0.7f;

            float scale = 1.5f + wave * 0.08f;
            transform.localScale = Vector3.one * scale;
        }
    }
    public void SetInteractable(bool value)
    {
        visual = GetComponent<MeshRenderer>();
        if (type == MarkerType.NPC)
            visual.enabled = value;
    }

    public void SetTracking(bool tracking)
    {

        visual = GetComponent<MeshRenderer>();
        isTracking = tracking;
        transform.localScale = tracking
            ? Vector3.one * 1.5f
            : Vector3.one;

        if (!tracking)
            transform.localPosition = startPos;

        visual.material = tracking ? trackedMaterial : normalMaterial;
    }
}