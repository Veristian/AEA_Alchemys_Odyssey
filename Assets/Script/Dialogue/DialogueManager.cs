using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using System;
using System.Linq;
using UnityEngine.UI;
[Serializable]
public class CharacterSprite
{
    public Sprite sprite;
    public SubmissionCharacter character;
}

public class DialogueManager : Singleton<DialogueManager>
{
    private const string DialogueResourcePath = "Dialogue/"; // Base path for dialogue JSON files in Resources
    [Header("UI Reference")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI rightSpeakerNameText;
    public TextMeshProUGUI leftSpeakerNameText;
    public Image leftSprite;
    public Image rightSprite;
    [Header("Character")]
    public List<CharacterSprite> characterSprites;
    // private HashSet<SubmissionCharacter> shownCharacters = new HashSet<SubmissionCharacter>();
    private SubmissionCharacter? currentSpeaker = null;

    [Header("Settings")]
    public float textSpeed = 0.02f;
    public float fadeDuration = 0.3f;

    private Story story;
    private Coroutine currentLineCoroutine;

    public bool isTyping { get; private set; }
    public bool dialogueActive { get; private set; }

    public static TextAsset GetInkJSON(string name)
    {
        TextAsset inkJSON = Resources.Load<TextAsset>(DialogueResourcePath + name);
        if (inkJSON == null)
        {
            Debug.LogWarning("Ink file not found at: " + DialogueResourcePath + name + ". Please ensure the JSON file is placed in a Resources folder and the path is correct.");
            return null;
        }
        return inkJSON;
    }
    public static Story GetStory(string name)
    {
        TextAsset inkJSON = Resources.Load<TextAsset>(DialogueResourcePath + name);
        if (inkJSON == null)
        {
            Debug.LogWarning("Ink file not found at: " + DialogueResourcePath + name + ". Please ensure the JSON file is placed in a Resources folder and the path is correct.");
            return null;
        }
        return new Story(inkJSON.text);
    }
    public static Story GetStory(TextAsset text)
    {
        return new Story(text.text);
    }

    public void StartDialogue(string name)
    {
        StartDialogue(GetStory(name), null);
    }
    public void StartDialogue(Story story, string knot = null)
    {
        this.story = story;
        Debug.Log(story);
        if (!string.IsNullOrEmpty(knot))
        {
            if (story.KnotContainerWithName(knot) != null)
                story.ChoosePathString(knot);
            else
                Debug.Log("Knot not found, starting from beginning");
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        dialogueActive = true;

        SetPlayerControl(false);

        ContinueStory();
    }

    // ================================
    // 🔹 CONTINUE STORY
    // ================================
    public void ContinueStory()
    {
        if (story == null) return;

        if (!story.canContinue)
        {
            EndDialogue();
            return;
        }

        string line = story.Continue().Trim();

        ParseTags(story.currentTags);

        if (currentLineCoroutine != null)
            StopCoroutine(currentLineCoroutine);
        if (dialogueText == null)
        {
            Debug.Log(line);
            Debug.LogWarning("Dialogue Text reference is not assigned. Please assign a TextMeshProUGUI reference to dialogueText in the inspector.");
            return;
        }
        currentLineCoroutine = StartCoroutine(TypeLine(line));
    }

    // ================================
    // 🔹 END
    // ================================
    void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);

        SetPlayerControl(true);
    }

    // ================================
    // 🔹 TYPEWRITER
    // ================================
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    // ================================
    // 🔹 INPUT
    // ================================
    private void Update()
    {
        if (!dialogueActive) return;

        if (InputManager.Instance.MouseLeftWasReleased)
        {
            if (isTyping)
            {
                StopCoroutine(currentLineCoroutine);
                dialogueText.text = story.currentText;
                isTyping = false;
            }
            else
            {
                ContinueStory();
            }
        }
    }

    // ================================
    // 🔹 PLAYER FREEZE
    // ================================
    void SetPlayerControl(bool enabled)
    {
        InputManager.Instance.canMove = enabled;
    }

    // ================================
    // 🔹 TAG PARSER
    // ================================
    void ParseTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            string[] split = tag.Split(':');
            if (split.Length < 2) continue;

            string key = split[0].Trim();
            string value = split[1].Trim();

            switch (key)
            {
                case "speaker":
                    if (leftSpeakerNameText == null || rightSpeakerNameText == null)
                    {
                        Debug.LogWarning("Speaker Name \"" + value + "\" Text reference is not assigned. Please assign a TextMeshProUGUI reference to leftSpeakerNameText or rightSpeakerNameText in the inspector.");
                        return;
                    }
                    if (HandleCharacterTag(value))
                    {
                        leftSpeakerNameText.text = value;
                    }
                    else
                    {
                        rightSpeakerNameText.text = value;
                    }
                    break;

                case "speed":
                    ApplySettings(value);
                    break;

                case "func":
                    HandleFunction(value);
                    break;
            }
        }
    }

    // ================================
    // 🔹 SETTINGS
    // ================================
    void ApplySettings(string value)
    {
        switch (value)
        {
            case "fast": textSpeed = 0.01f; break;
            case "slow": textSpeed = 0.05f; break;
        }
    }

    // ================================
    // 🔹 FUNCTIONS
    // ================================
    void HandleFunction(string func)
    {
        string[] parts = func.Split('@');

        string funcName = parts[0].Trim();
        string parameter = (parts.Length >= 2) ? parts[1].Trim() : "";

        switch (funcName)
        {
            case "submitquest":
                QuestRuntimeManager.Instance.SubmitQuest(parameter);
                break;
            case "givequest":
                QuestRuntimeManager.Instance.GiveQuest(parameter);
                break;
            case "teleport":
                SceneLoadingManager.Instance.Teleport(parameter);
                break;
            case "setactivetrue":
                GameObject.Find(parameter).SetActive(true);
                break;
            case "setactivefalse":
                GameObject.Find(parameter).SetActive(false);
                break;

        }
    }

    // ================================
    // 🔹 CHARACTER TAG
    // Format: char: Kenny
    // ================================
    bool HandleCharacterTag(string value)
    {
        if (Enum.TryParse(value, out SubmissionCharacter character))
        {
            currentSpeaker = character;
            if (currentSpeaker == SubmissionCharacter.Chemy)
            {
                ShowCharacter(character, true);
                return true;
            }
            else
            {
                ShowCharacter(character, false);
                return false;

            }
        }
        else
        {
            Debug.LogWarning("Invalid character enum: " + value);
        }
        return false;
    }    // ================================
         // 🔹 SHOW CHARACTER
         // ================================
    void ShowCharacter(SubmissionCharacter character, bool isMainCharacter)
    {
        foreach (CharacterSprite cs in characterSprites)
        {
            if (cs.character == character)
            {
                // bool firstTime = !shownCharacters.Contains(character);

                if (isMainCharacter)
                {
                    ChangeSprite(cs.sprite, "left");
                    DialogUiManager.Instance.MainCharacterSpeak();
                }
                else
                {
                    ChangeSprite(cs.sprite, "right");
                    DialogUiManager.Instance.SubCharacterSpeak();
                }
                // if (firstTime)
                //     shownCharacters.Add(character);

                // UpdateSpeakerHighlight();

                return;
            }
        }

        Debug.LogWarning("Character sprite not found: " + character);
    }
    // ================================
    // 🔹 CHANGE SPRITE + FADE
    // ================================
    void ChangeSprite(Sprite sprite, string position)
    {
        Image target = null;

        switch (position.ToLower())
        {
            case "left":
                target = leftSprite;
                break;
            case "right":
                target = rightSprite;
                break;
        }

        if (target == null)
        {
            Debug.LogWarning("Invalid position: " + position);
            return;
        }

        target.sprite = sprite;

        // if (fadeIn)
        // {
        //     Color c = target.color;
        //     c.a = 0f;
        //     target.color = c;

        //     StartCoroutine(FadeIn(target));
        // }
    }
    // // ================================
    // // 🔹 FADE IN
    // // ================================
    // IEnumerator FadeIn(Image renderer)
    // {
    //     float time = 0f;
    //     Color c = renderer.color;

    //     while (time < fadeDuration)
    //     {
    //         time += Time.deltaTime;
    //         float t = time / fadeDuration;

    //         c.a = Mathf.Lerp(0f, 1f, t);
    //         renderer.color = c;

    //         yield return null;
    //     }

    //     c.a = 1f;
    //     renderer.color = c;
    // }
    // void UpdateSpeakerHighlight()
    // {
    //     if (currentSpeaker == null) return;

    //     SubmissionCharacter speaker = currentSpeaker.Value;

    //     foreach (CharacterSprite cs in characterSprites)
    //     {
    //         if (!shownCharacters.Contains(cs.character))
    //             continue;

    //         Image target = null;

    //         if (leftSprite.sprite == cs.sprite)
    //             target = leftSprite;
    //         else if (rightSprite.sprite == cs.sprite)
    //             target = rightSprite;

    //         if (target == null) continue;

    //         Color c = target.color;

    //         if (cs.character == speaker)
    //         {
    //             c = Color.white;
    //             c.a = 1f;
    //         }
    //         else
    //         {
    //             c = new Color(0.5f, 0.5f, 0.5f, 1f); // dimmed
    //                                                  // c.a = 1f;
    //         }

    //         target.color = c;
    //     }
    // }

    // void SetImageTransparent()
    // {
    //     if (leftSprite == null || rightSprite == null) return;
    //     leftSprite.color = new Color(255, 255, 255, 0);
    //     rightSprite.color = new Color(255, 255, 255, 0);
    // }
}