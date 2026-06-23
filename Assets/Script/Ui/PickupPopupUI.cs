using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupPopupUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemText;
    //[SerializeField] private CanvasGroup canvasGroup;

    private RectTransform rectTransform;
    private Vector2 targetPos;
    private float moveSpeed = 12f;
    private bool isClosing;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(Sprite icon, string text)
    {
        itemIcon.sprite = icon;
        itemText.text = text;

        //canvasGroup.alpha = 0f;
        //rectTransform.localScale = Vector3.one * 0.9f;

        UITransitionManager.Instance.FadeIn(gameObject);
    }

    public void Setup()
    {
        UITransitionManager.Instance.FadeIn(gameObject);
    }

    public void MoveTo(Vector2 position, float speed)
    {
        targetPos = position;
        moveSpeed = speed;
    }

    private void Update()
    {
        if (!isClosing)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPos,
                Time.deltaTime * moveSpeed
            );
        }
    }

    public void Close()
    {
        if (isClosing) return;
        isClosing = true;

        UITransitionManager.Instance.FadeOut(gameObject, () =>
        {
        Destroy(gameObject);
        });
    }
}
