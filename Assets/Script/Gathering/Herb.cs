using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[RequireComponent(typeof(SphereCollider))]
public class Herb : MonoBehaviour, IInteractable
{
    [Header("Save")]
    [SerializeField]
    private string herbID;

    [Header("References")]
    [SerializeField] private IngredientData ingredientData;
    [SerializeField] private GameObject herbModel;
    [SerializeField] private ParticleSystem collectedEffect;

    private bool isTaken;
    private int dayTaken;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        LoadState();

        if (collectedEffect == null)
        {
            collectedEffect = GetComponentInChildren<ParticleSystem>(true);
        }
    }

    private void OnEnable()
    {
        if (HerbManager.Instance == null)
            return;

        LoadState();
    }

    private void LoadState()
    {
        HerbStateData state = HerbManager.Instance.GetState(herbID);

        isTaken = state.isTaken;
        dayTaken = state.dayTaken;

        if (isTaken && dayTaken < DayManager.Instance.Day)
        {
            RefreshHerb();
        }
        else
        {
            herbModel?.SetActive(!isTaken);
        }
    }

    public string text => "Collect";

    public bool interactable => !isTaken;

    public void Interact(GameObject interactor)
    {
        TakeHerb();
    }

    private void TakeHerb()
    {
        if (isTaken)
            return;

        isTaken = true;
        dayTaken = DayManager.Instance.Day;

        HerbManager.Instance.SetState(herbID, true, dayTaken);

        herbModel?.SetActive(false);

        if (ingredientData != null)
        {
            InventoryManager.Instance.AddIngredient(ingredientData);

            PickupPopupManager.Instance?.ShowPickup(ingredientData);

            if (collectedEffect != null)
                collectedEffect.Play();

            AudioController.Instance.PlaySFX("PickUp");
        }
    }

    private void RefreshHerb()
    {
        isTaken = false;
        dayTaken = 0;

        HerbManager.Instance.SetState(herbID, false, 0);

        herbModel?.SetActive(true);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        // Don't assign IDs to the prefab asset itself
        if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            return;

        if (string.IsNullOrEmpty(herbID))
        {
            herbID = GUID.Generate().ToString();
            EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }
    
#endif
}