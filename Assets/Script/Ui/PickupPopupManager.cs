using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPopupManager : Singleton<PickupPopupManager>
{
    //public static PickupPopupManager Instance;

    [Header("References")]
    [SerializeField] private RectTransform popupContainer;
    [SerializeField] private PickupPopupUI popupPrefab;

    [Header("Settings")]
    [SerializeField] private int maxPopups = 3;
    [SerializeField] private float popupDuration = 3f;
    [SerializeField] private float moveSpeed = 12f;

    [Header("Position")]
    [SerializeField] private float startY = 0f;
    [SerializeField] private float spacingY = 70f;
    [SerializeField] private float overlapAmount = 20f;

    private readonly List<PickupPopupUI> activePopups = new();
    //[SerializeField] private IngredientData ingredientData;

    //private void Awake()
    //{
    //    Instance = this;
    //}

    public void ShowPickup(IngredientData IngData)
    {
        PickupPopupUI newPopup = Instantiate(popupPrefab, popupContainer);

        newPopup.Setup(IngData.ingredientSprite, IngData.name);

        activePopups.Insert(0, newPopup);  //this make the new stack infront of old

        if (activePopups.Count > maxPopups)
        {
            PickupPopupUI lastPopup = activePopups[^1];
            activePopups.RemoveAt(activePopups.Count - 1);
            lastPopup.Close();
        }

        UpdatePopupPositions();
        StartCoroutine(RemoveAfterDelay(newPopup));
    }

    public void ShowPickup2()
    {
        PickupPopupUI newPopup = Instantiate(popupPrefab, popupContainer);

        newPopup.Setup();

        activePopups.Insert(0, newPopup);

        if (activePopups.Count > maxPopups)
        {
            PickupPopupUI lastPopup = activePopups[^1];
            activePopups.RemoveAt(activePopups.Count - 1);
            lastPopup.Close();
        }

        UpdatePopupPositions();
        StartCoroutine(RemoveAfterDelay(newPopup));
    }

    private IEnumerator RemoveAfterDelay(PickupPopupUI popup)
    {
        yield return new WaitForSeconds(popupDuration);

        if (activePopups.Contains(popup))
        {
            activePopups.Remove(popup);
            popup.Close();
            UpdatePopupPositions();
        }
    }

    private void UpdatePopupPositions()
    {
        for (int i = 0; i < activePopups.Count; i++)
        {
            //float yPos = startY + i * (spacingY - overlapAmount);
            float yPos = startY - i * (spacingY - overlapAmount);
            activePopups[i].MoveTo(new Vector2(0, yPos), moveSpeed);
        }
    }
}
