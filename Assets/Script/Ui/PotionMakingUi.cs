using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionMakingUi : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button tabButton;
        public GameObject container;
    }

    public Tab[] tabs;

    private int currentTabIndex = -1;

    [Header("Recipe Panel")]
    public GameObject RecipePanel;
    public Button RecipeOpenBtn;
    public Button RecipeCloseBtn;

    private void Awake()
    {
        if (RecipeOpenBtn != null)
        {
            RecipeOpenBtn.onClick.AddListener(OpenRecipePanel);

        }
        if (RecipeCloseBtn != null)
        {
            RecipeCloseBtn.onClick.AddListener(CloseRecipePanel);
        }
    }

    private void Start()
    {
        // Add button click events
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            tabs[i].tabButton.onClick.AddListener(() => OpenTab(index));
        }

        // Open first tab by default
        OpenTab(0);
    }

    // Function 1: Open selected tab
    public void OpenTab(int index)
    {
        if (index < 0 || index >= tabs.Length)
            return;

        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].container.SetActive(i == index);
        }

        currentTabIndex = index;
    }
    
    public void OpenRecipePanel()
    {
        if (RecipePanel!=null)
            RecipePanel.SetActive(true);
    }

    public void CloseRecipePanel()
    {
        if (RecipePanel!=null)
            RecipePanel.SetActive(false);
    }
}
