using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PotionGraph potionGraph;
    [SerializeField] private PotionGraph guideGraphTop;
    [SerializeField] private PotionGraph guideGraphBottom;    
    [Header("Active Data")]
    [Tooltip("The currently active potion. This is the potion that will be brewed when the player clicks the brew button.")]
    [SerializeField, ReadOnly] private PotionData activePotionTarget;
    [SerializeField, ReadOnly] private List<IngredientData> activePotionIngredientsTarget = new List<IngredientData>();

    [SerializeField] private List<IngredientData> currentActivePotionIngredients = new List<IngredientData>();

    [Header("Settings")]
    [SerializeField] private float distanceBetweenGuides = 0.95f;

    [Header("Debug")]
    [SerializeField] private PotionData setActivePotionDebug;

    private void Start()
    {
        guideGraphTop.SetAnchorTop(true);
    }

#region debug
//debug methods to set active potion and update graphs
    public void DebugSetActivePotionTarget()
    {
        SetActivePotionTarget(setActivePotionDebug);
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
    // sets the active potion and updates the guide graph to show the ingredients of the new potion.
    private void SetActivePotionTarget(PotionData newPotion)
    {
        activePotionTarget = newPotion;
        activePotionIngredientsTarget = new List<IngredientData>(activePotionTarget.ingredients);
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
    }
    private void AddPotionGraphCurves(AnimationCurve newCurves)
    {
        potionGraph.AddPotionCurve(newCurves);
    }

    //adds an ingredient to the current active potion and updates the potion graph to also add the same ingredent
    private void AddPotionIngredient(IngredientData newIngredient)
    {
        currentActivePotionIngredients.Add(newIngredient);
        AddPotionGraphCurves(newIngredient.ingredientCurve);
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
            //note to self: add success logic here
        }
        else
        {
            Debug.Log("Potion brewing failed. The potion graph does not match the guide graphs.");
            //note to self: add failure logic here
        }
        //note to self: add brew logic here
    }
    // adds an ingredient to the active potion and updates the potion graphs to reflect the new ingredient. This method will be called when the player adds an ingredient to the potion.
    public void AddIngredientToPotion(IngredientData newIngredient)
    {
        AddPotionIngredient(newIngredient);
    }
    // clears the current active potion and updates the potion graph to reflect the cleared potion. This method will be called when the player clicks the clear button.
    // cleared ingredients will be refunded
    public void ClearPotion()
    {
        currentActivePotionIngredients.Clear();
        ClearPotionGraph();

        //note to self: add refund logic here
    }
#endregion

#region private methods
// updates the guide graph to show the curves of the ingredients for the target potion. This method will be called whenever the active potion is changed or when an ingredient is added to the potion.   
    private void UpdateGuideGraph()
    {
        List<AnimationCurve> ingredientCurves = new List<AnimationCurve>();
        for (int i = 0; i < activePotionIngredientsTarget.Count; i++)
        {
            ingredientCurves.Add(activePotionIngredientsTarget[i].ingredientCurve);
        }
        SetGuideGraphCurves(ingredientCurves);
    }
// updates the potion graph to show the curves of the ingredients for the current active potion. This method will be called whenever an ingredient is added to the potion or when the potion is brewed.
    private void UpdatePotionGraph()
    {
        List<AnimationCurve> ingredientCurves = new List<AnimationCurve>();
        for (int i = 0; i < currentActivePotionIngredients.Count; i++)
        {
            ingredientCurves.Add(currentActivePotionIngredients[i].ingredientCurve);
        }
        SetPotionGraphCurves(ingredientCurves);
    }

    private void ClearPotionGraph()
    {
        potionGraph.ClearCurves();
    }
#endregion
}
