using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerPopUpUiManager : MonoBehaviour
{
    

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
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject questResultPanel;

    [Header("CloseBtns")]
    [SerializeField] private Button JournalCloseBtn;
    [SerializeField] private Button InventoryCloseBtn;
    [SerializeField] private Button PauseMenuCloseBtn;
    [SerializeField] private Button DailyTidalCloseBtn;
    [SerializeField] private Button GameStoreCloseBtn;
    [SerializeField] private Button TutorialCloseBtn;

    [Header("UiCategory")]
    [SerializeField] private GameObject PlayerHUD;

    [Header("QuestResultReference")]
    [SerializeField] private TextMeshProUGUI questResultQuestName;
    [SerializeField] private TextMeshProUGUI questResultQUestReward;
    private bool isQuestCompleteHolding = false;
    public bool isInPCrafting = false;

    

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
            if (isInPCrafting)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
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
        tutorialPanel.SetActive(false);

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
        if (isPopUpOpened && InputManager.Instance.canUiPopup == true)
        {
            OpenPopup(currentPopup);
        }
        else
        {
            OpenPopup(pauseMenuPanel);
        }
        
    }

    public void OpenDialog()
    {
        OpenPopup(dialogPanel);
    }

    public void ForceOpenDailyTidals()
    {
        CloseAllPopups();
        OpenPopup(dailyTidalsPanel);
        AudioController.Instance.PlaySFX("PaperOpen");
    }

    public void OpenDailyTidals()
    {
        OpenPopup(dailyTidalsPanel);
        //AudioController.Instance.PlaySFX("Paper");
        AudioController.Instance.PlaySFX("PaperClose");
        FirstTimeTrigger.TryActivate("t6");
    }

    public void OpenGameStore()
    {
        CloseAllPopups();
        OpenPopup(gameStorePanel);
        //Debug.Log("OpenTheFuckingStore");
    }

    public void CloseGameStore()
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
            GameStoreCloseBtn.onClick.AddListener(CloseGameStore);
        }
        if (TutorialCloseBtn != null)
        {
            TutorialCloseBtn.onClick.AddListener(CloseTutorialPanel);
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

    public void OpenTutorialPanel()
    {
        InputManager.Instance.canUiPopup = false;
        InputManager.Instance.canTakeInputs = false;
        InputManager.Instance.canPause = false;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        StartCoroutine(UnlockCursorNextFrame());

        UITransitionManager.Instance.FadeIn(tutorialPanel);
    }

    private IEnumerator UnlockCursorNextFrame()
    {
        yield return null; // Wait one frame

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTutorialPanel()
    {
        if (isInPCrafting)
            InputManager.Instance.canTakeInputs = true;
        if (currentPopup == null && !isInPCrafting)
        {
            InputManager.Instance.canTakeInputs = true;
            InputManager.Instance.canUiPopup = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        InputManager.Instance.canPause = true;
        UITransitionManager.Instance.FadeOut(tutorialPanel);
    }

    public void OpenQuestResultPanel()
    {
        questResultPanel.SetActive(true);
        AudioController.Instance.PlaySFX("MissionComplete");
        isQuestCompleteHolding = false;
    }

    public void CloseQuestResultPanel()
    {
        UITransitionManager.Instance.FadeOut(questResultPanel);
    }

    public void QuestResultSetup(string name, int amount)
    {
        questResultQuestName.text = name;
        questResultQUestReward.text = "Reward: " + amount + " Coins";
        isQuestCompleteHolding = true;
    }

    public void QuestResultChecker()
    {
        if (isQuestCompleteHolding)
        {
            OpenQuestResultPanel();
        }
    }
   
}
