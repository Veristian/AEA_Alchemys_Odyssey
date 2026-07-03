using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalUiManager : MonoBehaviour
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

    [System.Serializable]
    public class Page
    {
        public Button pageButton;
        public GameObject pageContainer;
        public Image pageImage;
        public RectTransform RootLayout;
        public bool isJournal;
    }

    public Page[] pages;
    private int currentPage = -1;
    private void OnEnable()
    {
        JournalPageSetup();
    }

    private void JournalTabSetup()
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
    public void OpenPage(int index)
    {
        if (index < 0 || index >= pages.Length || pages == null || pages.Length == 0)
            return;

        for (int i = 0; i < pages.Length; i++)
        {
            //if (tabs[i].IsUnityNull() || tabs[i].container.IsUnityNull()) continue;
            //tabs[i].container.SetActive(i == index);

            if (pages[i] == null) continue;

            // Show/hide container
            if (pages[i].pageContainer != null)
            {
                pages[i].pageContainer.SetActive(i == index);
                
            }
            if (pages[i].RootLayout != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(pages[i].RootLayout);
            }
                //pages[i].pageContainer.SetActive(i == index);

            // Change tab color
            if (pages[i].pageImage != null)
                pages[i].pageImage.color = (i == index)
                    ? Color.white
                    : Color.gray;
            if (pages[i].isJournal)
            {
                JournalItemSetup();
                JournalTabSetup();
            }

            //StartCoroutine(RebuildLayout(pages[i].pageContainer));
            //else
            //{
            //    RectTransform rootObject = pages[i].pageContainer.GetComponent<RectTransform>();
            //    Debug.Log("Rebuilding Layout");
            //    LayoutRebuilder.ForceRebuildLayoutImmediate(rootObject);
            //}
        }

        currentPage = index;
        //StartCoroutine(RebuildLayout(pages[index].pageContainer));
    }

    private void JournalPageSetup()
    {
        if (pages == null || pages.Length == 0)
        {
            Debug.LogWarning("No pages");
            return;
        }

        for (int i = 0; i < pages.Length; i++)
        {
            int index = i;
            if (pages[i].pageButton != null)
            {
                pages[i].pageButton.onClick.AddListener(() => OpenPage(index));
            }
            else
            {
                Debug.LogWarning($"Tab button for index {index} is not assigned. Please assign a Button reference to tabButton in the inspector for this tab.");
            }
        }
        OpenPage(0);
    }

    public void JournalItemSetup()
    {
        UiLoader.Instance.CallOpenIndexEvent();
        UiLoader.Instance.CallOpenQuestEvent();
    }

    
}
