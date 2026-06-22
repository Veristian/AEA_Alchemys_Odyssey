using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D; 
using TMPro;
using UnityEngine.UI;
[Serializable]
public class StoredData
{
    public IngredientData ingredientData;
    public float contactPoint;

    public StoredData(IngredientData ingredientData, float contactPoint)
    {
        this.ingredientData = ingredientData;
        this.contactPoint = contactPoint;
    }
}
public class PotionManager : Singleton<PotionManager>
{
    [Header("References")]
    [SerializeField] private Camera potionMakingCamera;
    public Camera PotionMakingCamera
    {
        get {return potionMakingCamera ?? Camera.main; }
    }
    [SerializeField] private Transform playArea;
    private Vector3 playAreaOrigin;
    public Transform PlayArea
    {
        get {return playArea ?? this.transform; }
    }
    [SerializeField] private GameObject canvas;

    [SerializeField] private PotionGraph potionGraph;
    private PolygonCollider2D potionGraphCollider;
    private IngredientAcceptor ingredientAcceptor;
    [SerializeField] private PotionGraph potionFutureGraph;
    [SerializeField] private PotionGraph guideGraphTop;
    [SerializeField] private PotionGraph guideGraphBottom;  
    [SerializeField] private Slider heatSlider;  

    [Header("References/Text Info")]
    [SerializeField] private TextMeshProUGUI potionNameText;
    [SerializeField] private TextMeshProUGUI potionDetailText;
  
    [Header("Active Data")]
    [Tooltip("The currently active potion. This is the potion that will be brewed when the player clicks the brew button.")]
    [SerializeField, ReadOnly] private PotionData activePotionTarget;
    [SerializeField, ReadOnly] private List<StoredData> activePotionIngredientsTarget = new List<StoredData>();

    [SerializeField] private List<StoredData> currentActivePotionIngredients = new List<StoredData>();

    [Header("Settings")]
    [SerializeField] private float distanceBetweenGuides = 0.95f;
    [Tooltip("Distance inside the potion until it confirms to add the potion ingredient. Starts at 0 to negative values")]
    [SerializeField] private float potionConfirmDistance = -5f;
    [SerializeField] private LayerMask ingredientObjectLayer;
    [SerializeField] private float splashForce = -100;
    [SerializeField] private float minHeatOffset = -5;
    [SerializeField] private float maxHeatOffset = 5;

    [Header("Debug")]
    [SerializeField] private PotionData setActivePotionDebug;
    [SerializeField] private IngredientData addedDebugIngredient;
    
    [Header("Graph Settings")]
    [SerializeField] private int pointsLength = 25;
    [SerializeField] private Vector2 graphSize = new Vector2(100, 100);
    [SerializeField, Range(0.25f, 0.75f)] private float graphDefaultRest = 0.5f;
    [SerializeField] private Vector3 graphOrigin;
    [Header("Potion Info")]
    [SerializeField] private float restOffset = 0f;
    [SerializeField, Range(0f, 0.4f)] public float bottomPaddingRatio = 0f;
    [SerializeField, Range(0f, 0.4f)] public float topPaddingRatio = 0f;

    private List<PotionIngredientObject> potionIngredientObjects = new List<PotionIngredientObject>();
    private List<AnimationCurve> futurePotionCurves = new List<AnimationCurve>();

    protected override void Awake()
    {
        base.Awake();
        SetGraphProperties();
        SetupPotionGraph();
        SetupPlayArea();
    }    
    private void Start()
    {
        guideGraphTop.SetAnchorTop(true);
        if (playArea != null)
            playAreaOrigin = playArea.transform.position;
        
    }

    private void FixedUpdate()
    {
        DetectAndStorePotionIngredientObjects();
        UpdateFuturePotionGraph();
        UpdateHeat();
        //setup offsets for dynamic graphs
        potionGraph.restOffset = restOffset;
        potionFutureGraph.restOffset = restOffset;
        
        // //setup graph position
        // potionGraph.graphOrigin = graphOrigin;
        // potionFutureGraph.graphOrigin = graphOrigin;
        // guideGraphBottom.graphOrigin = graphOrigin;
        // guideGraphTop.graphOrigin = graphOrigin;


    }

    private void OnEnable()
    {
        if (ingredientAcceptor == null)
        {
            Debug.LogWarning("Ingredient Acceptor is not assigned. Please ensure the PotionGraph has an IngredientAcceptor component.");
            return;
        }
        ingredientAcceptor.OnIngredientAccepted += CheckAndAddIngredientObjectToPotion;
    }
    private void OnDisable()
    {
        if (ingredientAcceptor == null)
        {
            Debug.LogWarning("Ingredient Acceptor is not assigned. Please ensure the PotionGraph has an IngredientAcceptor component.");
            return;
        }
        ingredientAcceptor.OnIngredientAccepted -= CheckAndAddIngredientObjectToPotion;
    }

    #region debug
    //debug methods to set active potion and update graphs
    public void DebugSetActivePotionTarget()
    {
        SetActivePotionTarget(setActivePotionDebug);
    }

    public void DebugTryAddingPotionAtRandomContact()
    {
        AddPotionIngredient(addedDebugIngredient, UnityEngine.Random.Range(0.0f,1.0f));
    }

    public void DebugUpdateGraphs()
    {
        UpdateGuideGraph();
        UpdatePotionGraph();
    }

    public void DebugClearPotionGraph()
    {
        ClearPotionGraph();
    }
#endregion

#region Setters
    private void SetGraphProperties()
    {
        if (guideGraphTop == null || potionFutureGraph == null || guideGraphBottom == null || potionGraph == null)
        {
            Debug.LogWarning("One or more graph references are not assigned. Please assign all graph references in the inspector.");
            return;
        }
        //setup size properties for all graphs
        guideGraphTop.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);
        potionFutureGraph.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);
        guideGraphBottom.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);
        potionGraph.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);

        //setup limits for dynamic graphs
        potionGraph.bottomPaddingRatio = bottomPaddingRatio;
        potionFutureGraph.bottomPaddingRatio = bottomPaddingRatio;
        potionGraph.topPaddingRatio = topPaddingRatio;
        potionFutureGraph.topPaddingRatio = topPaddingRatio;
        
        //setup leeway for guide graphs
        guideGraphTop.restOffset = distanceBetweenGuides/2;
        guideGraphBottom.restOffset = -distanceBetweenGuides/2;

    }

    private void SetupPlayArea()
    {
        if (playArea == null)
        {
            Debug.LogWarning("Play Area reference is not assigned. Please assign a Transform reference to playArea in the inspector.");
            return;
        }
        
        //assign all potion graph under play area
        guideGraphTop.transform.parent = playArea;
        potionFutureGraph.transform.parent = playArea;
        guideGraphBottom.transform.parent = playArea;
        potionGraph.transform.parent = playArea;
    }

    private void SetupPotionGraph()
    {
        if (potionGraph == null)
        {
            Debug.LogWarning("Potion Graph reference is not assigned. Please assign a PotionGraph reference in the inspector.");
            return;
        }
        
        foreach (SpriteShapeController spriteShape in potionGraph.SpriteShapeController)
        {
            if (spriteShape == null) return;
            var col = spriteShape.GetComponent<PolygonCollider2D>();
            if (col == null)
            {
                col = spriteShape.AddComponent<PolygonCollider2D>();
            }
            potionGraphCollider = col;
            potionGraphCollider.isTrigger = true;
            potionGraphCollider.offset = new Vector2(0,potionConfirmDistance);

            var pot = spriteShape.GetComponent<IngredientAcceptor>();
            if (pot == null)
            {
                pot = spriteShape.AddComponent<IngredientAcceptor>();
            }
            ingredientAcceptor = pot;

        }
    }
    // sets the active potion and updates the guide graph to show the ingredients of the new potion.
    private void SetActivePotionTarget(PotionData newPotion)
    {
        activePotionTarget = newPotion;
        activePotionIngredientsTarget = new List<StoredData>(activePotionTarget.ingredients);
        UpdateGuideGraph();

        if (activePotionTarget == null)
        {
            potionNameText.text = "";
            potionDetailText.text = "";
            return;
        }

        if (potionNameText == null || potionDetailText == null)
        {
            Debug.LogWarning("Potion Name Text or Potion Detail Text is not assigned. Please assign TextMeshProUGUI components to potionNameText and potionDetailText.");
            return;
        }
        potionNameText.text = activePotionTarget.potionName;
        potionDetailText.text = activePotionTarget.description;

    }
    
    private void SetGuideGraphCurves(List<AnimationCurve> newCurves)
    {
        guideGraphTop.SetPotionCurves(newCurves);
        guideGraphBottom.SetPotionCurves(newCurves);
    }
    

    private void SetPotionGraphCurves(List<AnimationCurve> newCurves)
    {
        potionGraph.SetPotionCurves(newCurves);
        // potionFutureGraph.SetPotionCurves(newCurves);
    }
    
    
    private void AddPotionGraphCurves(AnimationCurve newCurves)
    {
        potionGraph.AddPotionCurve(newCurves);
        // potionFutureGraph.AddPotionCurve(newCurves);
    }

    //adds an ingredient to the current active potion and updates the potion graph to also add the same ingredent
    private void AddPotionIngredient(IngredientData newIngredient, float contactPoint)
    {
        currentActivePotionIngredients.Add(new StoredData(newIngredient, contactPoint));
        AddPotionGraphCurves(AdjustCurveToContactPoint(newIngredient, contactPoint)); 
        potionGraph.Splash(contactPoint, splashForce);
    }
    
    
#endregion
#region Potion
    private bool CheckPotionBetweenGuides()
    {
        Vector3[] potionGraphPoints = potionGraph.GetPotionPoints();
        Vector3[] guideGraphTopPoints = guideGraphTop.GetPotionPoints();
        Vector3[] guideGraphBottomPoints = guideGraphBottom.GetPotionPoints();

        for (int i = 0; i < potionGraphPoints.Length; i++)
        {
            if (potionGraphPoints[i].y + potionGraph.restOffset > guideGraphTopPoints[i].y + guideGraphTop.restOffset || potionGraphPoints[i].y + potionGraph.restOffset < guideGraphBottomPoints[i].y + guideGraphBottom.restOffset)
            {
                Debug.Log($"Potion point {i} is out of bounds. Potion Y: {potionGraphPoints[i].y + potionGraph.restOffset*graphDefaultRest}, Top Guide Y: {guideGraphTopPoints[i].y + guideGraphTop.restOffset}, Bottom Guide Y: {guideGraphBottomPoints[i].y + guideGraphBottom.restOffset}");
                return false;
            }
        }
        return true;
    }
#endregion
#region public methods
    public void DeactivatePotionInterface()
    {
    
        activePotionTarget = null;
        activePotionIngredientsTarget.Clear();
        UpdateGuideGraph();

        //deactivate UI
        canvas.SetActive(false);
        playArea.gameObject.SetActive(false);
        potionMakingCamera.gameObject.SetActive(false);
    }

    public void ActivatePotionInterface()
    {
        //activate UI
        canvas.SetActive(true);
        playArea.gameObject.SetActive(true);
        potionMakingCamera.gameObject.SetActive(true);
    }
    public void AssignPotionTarget(PotionData potionData)
    {
        SetActivePotionTarget(potionData);
    }
    // brews the active potion using the current ingredients. This method will be called when the player clicks the brew button.
    // brewing will fail if both curves dont match
    public void BrewPotion()
    {
        if (CheckPotionBetweenGuides())
        {
            Debug.Log("Potion brewed successfully!");
            PotionMakingUi.Instance.ResultDisplaySet(true);
            InventoryManager.Instance.AddPotionObject(activePotionTarget, currentActivePotionIngredients);
            foreach (StoredData ingredient in currentActivePotionIngredients)
            {
                InventoryManager.Instance.SubtractIngredient(ingredient.ingredientData);
            }
            ClearPotion();
        }
        else
        {
            Debug.Log("Potion brewing failed. The potion graph does not match the guide graphs.");
            //note to self: add failure logic here
            PotionMakingUi.Instance.ResultDisplaySet(false);
        }
    }
    // adds an ingredient to the active potion and updates the potion graphs to reflect the new ingredient. This method will be called when the player adds an ingredient to the potion. Will reject potion if not used for the same questline
    public void CheckAndAddIngredientObjectToPotion(PotionIngredientObject newIngredient)
    {
        if (activePotionTarget == null)
        {
            AddPotionIngredient(newIngredient.IngredientData, (newIngredient.position.x-graphOrigin.x)/graphSize.x);
        }
        else if (newIngredient.IngredientData.questId == activePotionTarget.questId || (newIngredient.IngredientData.questId == null && activePotionTarget.questId == null))
            AddPotionIngredient(newIngredient.IngredientData, (newIngredient.position.x-graphOrigin.x)/graphSize.x);
        else
        {
            Debug.Log("Ingredient is not allowed in this potion");
        }
    }
    // clears the current active potion and updates the potion graph to reflect the cleared potion. This method will be called when the player clicks the clear button.
    // cleared ingredients will be refunded
    public void ClearPotion()
    {
        // var ingredientDataList = currentActivePotionIngredients
        IngredientHouse.Instance.UpdateIngredientsHousesObjTaken(currentActivePotionIngredients?
        .Select(s => s.ingredientData)
        .ToList() ?? new List<IngredientData>(), true);
        currentActivePotionIngredients.Clear();
        ClearPotionGraph();
    }


    private void UpdateFuturePotionGraph()
    {
        futurePotionCurves.Clear();
        ConvertPotionObjectsToCurve(futurePotionCurves,potionIngredientObjects);
        potionFutureGraph.FixPotionCurveNumber(futurePotionCurves.Count + currentActivePotionIngredients.Count);
        
        for (int i = 0; i < futurePotionCurves.Count + currentActivePotionIngredients.Count; i++)
        {
            if (i >= currentActivePotionIngredients.Count)
            {
                potionFutureGraph.UpdatePotionCurveAtIndex(i,futurePotionCurves[i - currentActivePotionIngredients.Count]);
            }
            else
            {
                potionFutureGraph.UpdatePotionCurveAtIndex(i,potionGraph.PotionCurves[i]);
            }
        }
        //add a way to correct the prev ones
        
    }
#endregion

#region private methods
// updates the guide graph to show the curves of the ingredients for the target potion. This method will be called whenever the active potion is changed 

    private void UpdateHeat()
    {
        if (playArea == null || heatSlider == null) return;
        restOffset = heatSlider.value * (maxHeatOffset - minHeatOffset) + minHeatOffset;
        playArea.transform.position = new Vector3(playArea.transform.position.x,playAreaOrigin.y - (restOffset),playArea.transform.position.z);
    }
    private void UpdateGuideGraph()
    {
        List<AnimationCurve> ingredientCurves = new List<AnimationCurve>();
        for (int i = 0; i < activePotionIngredientsTarget.Count; i++)
        {
            ingredientCurves.Add(AdjustCurveToContactPoint(activePotionIngredientsTarget[i].ingredientData,activePotionIngredientsTarget[i].contactPoint));
        }
        SetGuideGraphCurves(ingredientCurves);
    }
// updates the potion graph to show the curves of the ingredients for the current active potion. This method will be called during debugging
    private void UpdatePotionGraph()
    {
        List<AnimationCurve> ingredientCurves = new List<AnimationCurve>();
        for (int i = 0; i < currentActivePotionIngredients.Count; i++)
        {
            ingredientCurves.Add(AdjustCurveToContactPoint(currentActivePotionIngredients[i].ingredientData, currentActivePotionIngredients[i].contactPoint)); 
        }
        SetPotionGraphCurves(ingredientCurves); 
    }

    private void ClearPotionGraph()
    {
        potionGraph.ClearCurves();
    }
    

    private AnimationCurve AdjustCurveToContactPoint(IngredientData data, float contactPoint)
    {
        AnimationCurve newCurve = new AnimationCurve();
        newCurve.CopyFrom(data.ingredientCurve);
        Vector2[] points = new Vector2[10];
        GraphRender.Instance.ConvertCurveToSpecifiedPointsLength(data.ingredientCurve, points);
        for (int i = 0; i < points.Length; i++)
        {
            points[i].x *= (float)data.ingredientAreaOfEffect/100f;
            points[i].x += ((contactPoint - 0.5f)*100f)/(float)data.ingredientAreaOfEffect;
        }
        GraphRender.Instance.ConvertSpecifiedPointsToCurve(newCurve, points);
        return newCurve;

    }
    private AnimationCurve AdjustCurveToContactPoint(StoredData potionData)
    {
        return AdjustCurveToContactPoint(potionData.ingredientData, potionData.contactPoint);
        // AnimationCurve newCurve = new AnimationCurve();
        // newCurve.CopyFrom(potionData.ingredientData.ingredientCurve);
        // Vector2[] points = new Vector2[10];
        // GraphRender.Instance.ConvertCurveToSpecifiedPointsLength(potionData.ingredientData.ingredientCurve, points);
        // for (int i = 0; i < points.Length; i++)
        // {
        //     points[i].x *= (float)potionData.ingredientData.ingredientAreaOfEffect/100f;
        //     points[i].x += ((potionData.contactPoint - 0.5f)*100f)/(float)potionData.ingredientData.ingredientAreaOfEffect;
        // }
        // GraphRender.Instance.ConvertSpecifiedPointsToCurve(newCurve, points);
        // return newCurve;

    }

    private void DetectAndStorePotionIngredientObjects()
    {
        Collider[] colliders = Physics.OverlapBox(new Vector3(graphOrigin.x + graphSize.x/2, graphOrigin.y + graphSize.y, graphOrigin.z), new Vector3(graphSize.x*.5f,graphSize.y,0), Quaternion.identity, ingredientObjectLayer);
        
        if (colliders.Length == 0)
        {
            potionIngredientObjects = null;
            return;
        }
        potionIngredientObjects = colliders.Select(c => c.GetComponent<PotionIngredientObject>()).ToList();

    }

    private void ConvertPotionObjectsToCurve(List<AnimationCurve> potionCurves, List<PotionIngredientObject> potionIngredientObjects)
    {
        if (potionIngredientObjects == null) return;
        potionCurves.Clear();
        
        for (int i = 0; i < potionIngredientObjects.Count; i++)
        {
            potionCurves.Add(AdjustCurveToContactPoint(potionIngredientObjects[i].ToStoredData(graphSize.x,graphOrigin.x))); 
        }
    }

    
#endregion
}
