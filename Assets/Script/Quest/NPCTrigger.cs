using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class NPCTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private TextAsset NPCDefaultDialogue;
    [SerializeField] private SubmissionCharacter character;
    private BoxCollider interactCollider;
    [SerializeField] private bool isOneTime = false;

    public string text
    {
        get
        {
            return "Talk";
        }
    }
    public bool interactable
    {
        get
        {
            return !DialogueManager.Instance.dialogueActive && (NPCDefaultDialogue != null || GetCharacterQuest(true).Count != 0)  ;
        }
    }
    // bool playerInside;
    private void Awake()
    {
        interactCollider = GetComponent<BoxCollider>();
        interactCollider.isTrigger = true;
    }

    //check quest
    private List<PlayerQuestData> GetCharacterQuest()
    {
        return QuestRuntimeManager.Instance.GetOngoingQuests().FindAll(q => q.questData.submissionCharacter == character);
    }
    private List<PlayerQuestData> GetCharacterQuest(bool completed)
    {
        return QuestRuntimeManager.Instance.GetOngoingQuests().FindAll(q => q.questData.submissionCharacter == character).FindAll(q => q.questData.requirementsToComplete.AreAllMet());
    }

    //trigger check
    private void StartConversation()
    {
        if (DialogueManager.Instance.dialogueActive) return;
        List<PlayerQuestData> completedQuest = GetCharacterQuest(true);
        if (completedQuest == null || completedQuest.Count == 0)
        {
            DialogueManager.Instance.StartDialogue(DialogueManager.GetStory(NPCDefaultDialogue));
        }
        else
        {
            DialogueManager.Instance.StartDialogue(completedQuest[0].questData.story);
        }
        if (isOneTime) interactCollider.enabled = false;
    }


    // //ColliderTrigger
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInside = true;
    //     }
    // }
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInside = false;
    //     }
    // }

    // private void Update()
    // {
    //     if (InputManager.Instance.InteractWasPressed && !DialogueManager.Instance.dialogueActive && playerInside)
    //     {
    //         StartConversation();
    //     }
    // }

    public void Interact(GameObject interactor)
    {
        StartConversation();
    }

}
