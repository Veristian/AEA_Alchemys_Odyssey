using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUiManager : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button tabButton;
        public GameObject container;
        public Image tabImage;
    }

    public Tab[] tabs;
    private int currentTabIndex = -1;

    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI CoinText;

    private void OnEnable()
    {
        InventoryItemSetup();
        InventoryTabSetup();
    }
 
    private void InventoryTabSetup()
    {
        if (tabs == null || tabs.Length == 0)
        {
            Debug.LogWarning("Tabs are not assigned. Please assign tabs in the inspector.");
            return;
        }
        // Add button click events
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            if (tabs[i].tabButton != null)
            {
                tabs[i].tabButton.onClick.AddListener(() => OpenTab(index));
            }
            else
            {
                Debug.LogWarning($"Tab button for index {index} is not assigned. Please assign a Button reference to tabButton in the inspector for this tab.");
            }
        }
        OpenTab(0);
    }

    public void OpenTab(int index)
    {
        if (index < 0 || index >= tabs.Length || tabs == null || tabs.Length == 0)
            return;

        for (int i = 0; i < tabs.Length; i++)
        {
            //if (tabs[i].IsUnityNull() || tabs[i].container.IsUnityNull()) continue;
            //tabs[i].container.SetActive(i == index);

            if (tabs[i] == null) continue;

            // Show/hide container
            if (tabs[i].container != null)
                tabs[i].container.SetActive(i == index);
            // Change tab color
            if (tabs[i].tabImage != null)
                tabs[i].tabImage.color = (i == index)
                    ? Color.white
                    : Color.gray;
        }

        currentTabIndex = index;
    } 

    public void InventoryItemSetup()
    {
        CoinText.text = PlayerDataManager.Instance.gold.ToString();
        UiLoader.Instance.CallOpenInventoryEvent();

    }
}
