using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D; 

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
public class PotionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PotionGraph potionGraph;
    private PolygonCollider2D potionGraphCollider;
    private IngredientAcceptor ingredientAcceptor;
    [SerializeField] private PotionGraph potionFutureGraph;
    [SerializeField] private PotionGraph guideGraphTop;
    [SerializeField] private PotionGraph guideGraphBottom;    
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
    [Header("Debug")]
    [SerializeField] private PotionData setActivePotionDebug;
    [SerializeField] private IngredientData addedDebugIngredient;
    
    [Header("Graph Settings")]
    [SerializeField] private int pointsLength = 25;
    [SerializeField] private Vector2 graphSize = new Vector2(100, 100);
    [SerializeField, Range(0.25f, 0.75f)] private float graphDefaultRest = 0.5f;
    [SerializeField] private Transform graphOrigin;

    private List<PotionIngredientObject> potionIngredientObjects = new List<PotionIngredientObject>();
    private List<AnimationCurve> futurePotionCurves = new List<AnimationCurve>();

    private void Awake()
    {
        SetGraphProperties();
        SetupPotionGraph();
    }    
    private void Start()
    {
        guideGraphTop.SetAnchorTop(true);
    }

    private void FixedUpdate()
    {
        DetectAndStorePotionIngredientObjects();
        UpdateFuturePotionGraph();
    }

    private void OnEnable()
    {
        ingredientAcceptor.OnIngredientAccepted += CheckAndAddIngredientObjectToPotion;
    }
    private void OnDisable()
    {
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
        guideGraphTop.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);
        potionFutureGraph.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);
        guideGraphBottom.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);
        potionGraph.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);
    }

    private void SetupPotionGraph()
    {
        foreach (SpriteShapeController spriteShape in potionGraph.SpriteShapeController)
        {
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
    }
    
    private void SetGuideGraphCurves(List<AnimationCurve> newCurves)
    {
        guideGraphTop.SetPotionCurves(newCurves, distanceBetweenGuides);
        guideGraphBottom.SetPotionCurves(newCurves, -distanceBetweenGuides);
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
    }
    
    
#endregion
#region Potion
    private bool CheckPotionBetweenGuides()
    {
        for (int i = 0; i < potionGraph.GetPotionPoints().Length; i++)
        {
            if (potionGraph.GetPotionPoints()[i].y > guideGraphTop.GetPotionPoints()[i].y || potionGraph.GetPotionPoints()[i].y < guideGraphBottom.GetPotionPoints()[i].y)
            {
                return false;
            }
        }
        return true;
    }
#endregion
#region public methods
    // brews the active potion using the current ingredients. This method will be called when the player clicks the brew button.
    // brewing will fail if both curves dont match
    public void BrewPotion()
    {
        if (CheckPotionBetweenGuides())
        {
            Debug.Log("Potion brewed successfully!");
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
        }
    }
    // adds an ingredient to the active potion and updates the potion graphs to reflect the new ingredient. This method will be called when the player adds an ingredient to the potion. Will reject potion if not used for the same questline
    public void CheckAndAddIngredientObjectToPotion(PotionIngredientObject newIngredient)
    {
        if (activePotionTarget == null)
        {
            AddPotionIngredient(newIngredient.IngredientData, (newIngredient.position.x-graphOrigin.position.x)/graphSize.x);
        }
        else if (newIngredient.IngredientData.quest_id == activePotionTarget.quest_id || (newIngredient.IngredientData.quest_id == null && activePotionTarget.quest_id == null))
            AddPotionIngredient(newIngredient.IngredientData, (newIngredient.position.x-graphOrigin.position.x)/graphSize.x);
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
        .ToList() ?? new List<IngredientData>());
        currentActivePotionIngredients.Clear();
        ClearPotionGraph();
        //note to self: add update ingredient count here
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
        Collider[] colliders = Physics.OverlapBox(new Vector3(graphOrigin.position.x + graphSize.x/2, graphOrigin.position.y + graphSize.y/2, graphOrigin.position.z), graphSize*0.5f, Quaternion.identity, ingredientObjectLayer);
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
            potionCurves.Add(AdjustCurveToContactPoint(potionIngredientObjects[i].ToStoredData(graphSize.x,graphOrigin.position.x))); 
        }
    }

    
#endregion
}
