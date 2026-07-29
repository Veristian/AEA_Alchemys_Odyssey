using UnityEngine;
using UnityEngine.UI;

public class DialogUiManager : Singleton<DialogUiManager>
{
    [Header("Chat Containers")]
    [SerializeField] private Transform ActiveChatBox;
    [SerializeField] private Transform InActiveChatBox;

    [Header("MainCharacter")]
    [SerializeField] private Image MainCharacterSprite;
    [SerializeField] private RectTransform MainCharacterChatBox;
    [SerializeField] private GameObject MainCharacterChatBlock;
    

    [Header("SubCharacter")]
    [SerializeField] private Image SubCharacterSprite;
    [SerializeField] private RectTransform SubCharacterChatBox;
    [SerializeField] private GameObject SubCharacterChatBlock;

    [Header("Colors")]
    [SerializeField] private Color ActiveColor = Color.white;
    [SerializeField] private Color InActiveColor = Color.gray;

    [Header("Buttons")]
    [SerializeField] private Button skipBtn;

    private void Start()
    {
        if (skipBtn != null)
        {
            skipBtn.onClick.AddListener(SkipDialogueTrigger);
        }
    }
    public void MainCharacterSpeak()
    {
        SetSpeaker(
            MainCharacterSprite, MainCharacterChatBox, MainCharacterChatBlock,
            SubCharacterSprite, SubCharacterChatBox,SubCharacterChatBlock
        );
    }

    public void SubCharacterSpeak()
    {
        SetSpeaker(
            SubCharacterSprite, SubCharacterChatBox,SubCharacterChatBlock,
            MainCharacterSprite, MainCharacterChatBox,MainCharacterChatBlock
        );
    }

    private void SetSpeaker(
        Image activeSprite, RectTransform activeBox, GameObject activeBlock,
        Image inactiveSprite, RectTransform inactiveBox,GameObject inactiveBlock)
    {
        // Sprite color
        activeSprite.color = ActiveColor;
        inactiveSprite.color = InActiveColor;

        // Move chat boxes to containers
        activeBox.SetParent(ActiveChatBox, false);
        inactiveBox.SetParent(InActiveChatBox, false);

        // Set Chat Block
        activeBlock.SetActive(false);
        inactiveBlock.SetActive(true);

        // Reset position inside container
        activeBox.anchoredPosition = Vector2.zero;
        inactiveBox.anchoredPosition = Vector2.zero;
    }

    private void SkipDialogueTrigger()
    {
        DialogueManager.Instance.SkipDialogue();
    }

}