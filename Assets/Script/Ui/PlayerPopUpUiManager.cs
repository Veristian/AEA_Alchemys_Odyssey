using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerPopUpUiManager : MonoBehaviour
{
    [System.Serializable]
    public class InteractIndicator
    {
        public string name;
        public Sprite image;
    }

    public static PlayerPopUpUiManager Instance;

    [Header("Background")]
    [SerializeField] private GameObject transBG;

    [Header("Popups")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private GameObject dailyTidalsPanel;
    [SerializeField] private GameObject gameStorePanel;

    [Header("CloseBtns")]
    [SerializeField] private Button JournalCloseBtn;
    [SerializeField] private Button InventoryCloseBtn;
    [SerializeField] private Button PauseMenuCloseBtn;
    [SerializeField] private Button DailyTidalCloseBtn;
    [SerializeField] private Button GameStoreCloseBtn;

    [Header("UiCategory")]
    [SerializeField] private GameObject PlayerHUD;

    [Header("Interactable Indicator")]
    [SerializeField] private GameObject IiPanel;
    [SerializeField] private InteractIndicator[] iIndicators;

    private GameObject currentPopup;
    private bool isPopUpOpened = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        CloseAllPopups();

        ButtonAssign();
    }

    private void Update()
    {
        if (InputManager.Instance.PauseWasPressed)
        {
            OpenPauseMenu();
        }
        if (InputManager.Instance.InventoryWasPressed)
        {
            OpenInventory();
        }
        if (InputManager.Instance.JournalWasPressed)
        {
            OpenJournal();
        }

    }

    public void OpenPopup(GameObject popup)
    {
        if (currentPopup == popup)
        {
            CloseCurrentPopup();
            isPopUpOpened = false;
            InputManager.Instance.EnableInputs();
            //InputManager.Instance.canMove = true;
            //InputManager.Instance.canLook = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }
        if (isPopUpOpened)
        {
            return;
        }
        CloseCurrentPopup();

        //InputManager.Instance.canMove = false;
        //InputManager.Instance.canLook = false;
        InputManager.Instance.DisableInputs();


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentPopup = popup;
        UITransitionManager.Instance.FadeIn(currentPopup);

        if (transBG != null)
        {
            UITransitionManager.Instance.FadeIn(transBG);
            //transBG.SetActive(true);
        }
        isPopUpOpened = true;
    }


    public void CloseCurrentPopup()
    {
        if (currentPopup != null)
        {
            UITransitionManager.Instance.FadeOut(currentPopup);
            currentPopup = null;
        }

        if (transBG != null)
        {
            //UITransitionManager.Instance.FadeOut(transBG);
            transBG.SetActive(false);
        }            
    }

    public void CloseAllPopups()
    {
        journalPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        dialogPanel.SetActive(false);
        dailyTidalsPanel.SetActive(false);

        currentPopup = null;

        if (transBG != null)
            transBG.SetActive(false);
        isPopUpOpened = false;
    }

    public void OpenJournal()
    {
        OpenPopup(journalPanel);
    }

    public void OpenInventory()
    {
        OpenPopup(inventoryPanel);
    }


    public void OpenPauseMenu()
    {
        OpenPopup(pauseMenuPanel);
    }

    public void OpenDialog()
    {
        OpenPopup(dialogPanel);
    }

    public void OpenDailyTidals()
    {
        OpenPopup(dailyTidalsPanel);
    }

    public void OpenGameStore()
    {
        OpenPopup(gameStorePanel);
    }

    private void ButtonAssign()
    {
        if (JournalCloseBtn != null)
        {
            JournalCloseBtn.onClick.AddListener(OpenJournal);
        }
        if (InventoryCloseBtn != null)
        {
            InventoryCloseBtn.onClick.AddListener(OpenInventory);
        }
        if (PauseMenuCloseBtn != null)
        {
            PauseMenuCloseBtn.onClick.AddListener(OpenPauseMenu);
        }
        if (DailyTidalCloseBtn != null)
        {
            DailyTidalCloseBtn.onClick.AddListener(OpenDailyTidals);
        }
        if (GameStoreCloseBtn != null)
        {
            GameStoreCloseBtn.onClick.AddListener(OpenGameStore);
        }

    }

    public void disableHUD()
    {
        InputManager.Instance.canUiPopup = false;
        PlayerHUD.SetActive(false);
        CloseAllPopups();
        InputManager.Instance.DisableInputs();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnableHUD()
    {
        InputManager.Instance.canUiPopup = true;
        PlayerHUD.SetActive(true);
        InputManager.Instance.EnableInputs();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void interactableIndicatorChecker(string text)
    {
        if (text == null)
        {
            UITransitionManager.Instance.FadeOut(IiPanel);
        }
        else
        {
            UITransitionManager.Instance.FadeIn(IiPanel);
        }
    }
}
