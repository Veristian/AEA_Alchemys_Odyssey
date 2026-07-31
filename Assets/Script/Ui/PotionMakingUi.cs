using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PotionMakingUi : MonoBehaviour
{
    public static PotionMakingUi Instance;

    [System.Serializable]
    public class Tab
    {
        public Button tabButton;
        public GameObject container;
        public Image tabImage;
    }

    public Tab[] tabs;

    private int currentTabIndex = -1;

    [Header("Recipe Panel Section")]
    public GameObject RecipePanel;
    public Button RecipeOpenBtn;
    public Button RecipeCloseBtn;
    [SerializeField] public GameObject RecipeTargetGO;
    public Button ResetRecipeTargetBtn;
    public PotionData emptyPotionTarget;

    [Header("Brewing Result Section")]
    [SerializeField] private GameObject ResultDisplayGO;
    [SerializeField] private Image ResultItemBox;
    [SerializeField] private TextMeshProUGUI ResultText;
    [SerializeField] private TextMeshProUGUI ResultItemName;
    [SerializeField] private Image ResultItemImage;
    [SerializeField] private Image ResultBackgroundImage;
    [SerializeField] private Button ResultDisplayCloseBtn;
    [SerializeField] private GameObject CauldronGO;

    [Header("Brew Potion Animation")]
    [SerializeField] private GameObject[] ItemToHidnWhenBrew;

    [Header("Potion Info")]
    [SerializeField] private PotionInventoryData potion;

    public PotionInventoryData Potion
    {
        get { return potion; }
    }

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

        if (RecipeOpenBtn != null)
        {
            RecipeOpenBtn.onClick.AddListener(OpenRecipePanel);

        }
        if (RecipeCloseBtn != null)
        {
            RecipeCloseBtn.onClick.AddListener(CloseRecipePanel);
        }
        if (ResultDisplayCloseBtn != null)
        {
            ResultDisplayCloseBtn.onClick.AddListener(CloseResultPanel);
        }
        if (ResetRecipeTargetBtn != null)
        {
            ResetRecipeTargetBtn.onClick.AddListener(ResetRecipeTargetItem);
        }
    }

    private void Start()
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

        // Open first tab by default
        OpenTab(0);
    }

    // Function 1: Open selected tab
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
    
    private void OpenRecipePanel()
    {
        if (RecipePanel!=null)
        {
            //RecipePanel.SetActive(true);
            UITransitionManager.Instance.FadeIn(RecipePanel.gameObject);
            RecipeHouse.Instance.SetupRecipes();
        }

    }

    private void CloseRecipePanel()
    {
        if (RecipePanel!=null)
            //RecipePanel.SetActive(false);
            UITransitionManager.Instance.FadeOut(RecipePanel.gameObject);
    }

    private void OpenRecipeTarget()
    {
        if (RecipeTargetGO != null)
        {
            //RecipeTargetGO.SetActive(true);
            UITransitionManager.Instance.FadeIn(RecipeTargetGO.gameObject);
        }
    }
    private void CloseRecipeTarget()
    {
        if (RecipeTargetGO != null)
        {
            //RecipeTargetGO.SetActive(false);
            UITransitionManager.Instance.FadeOut(RecipeTargetGO.gameObject);
        }
    }

    public void SetRecipeTarget()
    {
        CloseRecipePanel();
        OpenRecipeTarget();
    }

    private void OpenResultPanel()
    {
        if (ResultText.text == "SUCCESS")
        {
            AudioController.Instance.PlaySFX("PotionSuccess");
        }
        else
        {
            AudioController.Instance.PlaySFX("PotionFail");
        }
        //ResultDisplayGO.SetActive(true);
        UITransitionManager.Instance.FadeIn(ResultDisplayGO.gameObject);
        Animator ResultAnim = ResultDisplayGO.GetComponent<Animator>();
        ResultAnim.SetTrigger("OpenCloseBtn");
    }
    private void CloseResultPanel()
    {
        //ResultDisplayGO.SetActive(false);
        UITransitionManager.Instance.FadeOut(ResultDisplayGO.gameObject);
        ShowUiComponent();
        Animator CauldronAnim = CauldronGO.GetComponent<Animator>();
        CauldronAnim.enabled = false;
        CauldronAnim.enabled = true;
        
        PlayerPopUpUiManager.Instance.QuestResultChecker();
    }

    public void ResultDisplaySet(bool result, Sprite PotionImage, string ItemName)
    {
        StartCauldronAnim();

        if (result)
        {
            ResultBackgroundImage.color = new Color(0.6f, 0.6f, 0.6f, 1f); ;
            ResultItemBox.color = Color.white;
            ResultItemImage.gameObject.SetActive(true);
            ResultItemImage.sprite = PotionImage;
            ResultItemName.text = ItemName.ToString();
            ResultText.text = "SUCCESS";
            //ResultItemName.text = "Potion Brewed";

            
        }
        else
        {
            ResultBackgroundImage.color = new Color(0.58f, 0f, 0f, 1f); // dark red
            ResultItemBox.color = Color.gray;
            ResultItemImage.gameObject.SetActive(false);
            ResultText.text = "FAILED";
            ResultItemName.text = "No Potion Brewed";
        }
    }

    private void StartCauldronAnim()
    {
        Animator CauldronAnim = CauldronGO.GetComponent<Animator>();
        CauldronAnim.SetTrigger("StartAnim");
        HideUiComponent();
    }

    public void CauldronBrewAnimCompleted()
    {
        Animator CauldronAnim = CauldronGO.GetComponent<Animator>();
        CauldronAnim.ResetTrigger("StartAnim");
        OpenResultPanel();

    }
    public void CauldronResetAnimCompleted()
    {
        Animator CauldronAnim = CauldronGO.GetComponent<Animator>();
        CauldronAnim.ResetTrigger("StartReset");
        ShowUiComponent();
    }

    public void ResetCauldron()
    {
        Animator CauldronAnim = CauldronGO.GetComponent<Animator>();
        CauldronAnim.SetTrigger("StartReset");
        HideUiComponent();
    }

    private void HideUiComponent()
    {
        foreach (var t in ItemToHidnWhenBrew)
        {
            //t.SetActive(false);
            UITransitionManager.Instance.FadeOut(t);
        }
    }

    private void ShowUiComponent()
    {
        foreach (var t in ItemToHidnWhenBrew)
        {
            //t.SetActive(false);
            UITransitionManager.Instance.FadeIn(t);
        }
    }

    public void ResetRecipeTargetItem()
    {
        CloseRecipeTarget();
        PotionManager.Instance.AssignPotionTarget(null);
    }

}
