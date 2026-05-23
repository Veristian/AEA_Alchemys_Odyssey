using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionIngredientObject : MonoBehaviour
{
    [SerializeField] private IngredientData ingredientData;
    
    public IngredientData IngredientData => ingredientData;
    public Vector3 position => transform.position;

    public StoredData ToStoredData(float sizeX, float originX)
    {
        return new StoredData(ingredientData, (position.x - originX)/sizeX);
    }

    
}
