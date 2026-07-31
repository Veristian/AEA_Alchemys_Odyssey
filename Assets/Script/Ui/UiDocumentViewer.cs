using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UIDocumentViewer : MonoBehaviour
{
    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 3f;

    [Header("Dragging")]
    [SerializeField] private float dragMultiplier = 1f;

    [Header("Bounds")]
    [SerializeField] private bool clampToScreen = true;
    [SerializeField] private float screenPadding = 50f;

    [Header("References")]
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private Camera uiCamera;

    private float currentZoom = 1f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCamera = canvas.worldCamera;

        currentZoom = rectTransform.localScale.x;
    }

    private void Update()
    {
        if (InputManager.Instance == null)
            return;

        HandleZoom();
        HandleDrag();

        if (clampToScreen)
            ClampToScreen();
    }

    private void HandleZoom()
    {
        float scroll = InputManager.Instance.Scroll;

        if (Mathf.Approximately(scroll, 0f))
            return;

        Vector2 mousePosition = InputManager.Instance.MousePosition;

        // Find the position of the mouse relative to the UI element
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            mousePosition,
            uiCamera,
            out Vector2 localMousePosition
        );

        float oldZoom = currentZoom;

        currentZoom += scroll * zoomSpeed * Time.deltaTime;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        if (Mathf.Approximately(oldZoom, currentZoom))
            return;

        // Position before zoom
        Vector3 worldMousePositionBeforeZoom =
            rectTransform.TransformPoint(localMousePosition);

        // Apply zoom
        rectTransform.localScale = Vector3.one * currentZoom;

        // Position after zoom
        Vector3 worldMousePositionAfterZoom =
            rectTransform.TransformPoint(localMousePosition);

        // Move the document so the point underneath
        // the mouse remains underneath the mouse.
        Vector3 offset =
            worldMousePositionBeforeZoom - worldMousePositionAfterZoom;

        rectTransform.position += offset;
    }

    private void HandleDrag()
    {
        if (!InputManager.Instance.DocumentDragIsHeld)
            return;

        Vector2 mouseDelta = InputManager.Instance.MouseDelta;

        if (mouseDelta.sqrMagnitude <= 0f)
            return;

        Vector2 movement = mouseDelta * dragMultiplier;

        // Convert screen-space movement into canvas-space movement.
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                InputManager.Instance.MousePosition,
                uiCamera,
                out Vector2 currentMousePosition
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                InputManager.Instance.MousePosition - mouseDelta,
                uiCamera,
                out Vector2 previousMousePosition
            );

            movement = currentMousePosition - previousMousePosition;
        }

        // Compensate for zoom.
        movement /= rectTransform.localScale.x;

        rectTransform.anchoredPosition += movement;
    }
    private void ClampToScreen()
    {
        if (canvas == null)
            return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        float minX = canvasCorners[0].x + screenPadding;
        float maxX = canvasCorners[2].x - screenPadding;

        float minY = canvasCorners[0].y + screenPadding;
        float maxY = canvasCorners[2].y - screenPadding;

        Vector3 correction = Vector3.zero;

        // Left
        if (corners[0].x > maxX)
            correction.x = maxX - corners[0].x;

        // Right
        if (corners[2].x < minX)
            correction.x = minX - corners[2].x;

        // Bottom
        if (corners[0].y > maxY)
            correction.y = maxY - corners[0].y;

        // Top
        if (corners[2].y < minY)
            correction.y = minY - corners[2].y;

        rectTransform.position += correction;
    }

    public void ResetView()
    {
        currentZoom = 1f;
        rectTransform.localScale = Vector3.one;

        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void SetZoom(float zoom)
    {
        currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        rectTransform.localScale = Vector3.one * currentZoom;
    }

    public float GetZoom()
    {
        return currentZoom;
    }
}

