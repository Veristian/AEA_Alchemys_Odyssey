using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using System;

public class DialogueManager : Singleton<DialogueManager>
{
    private const string DialogueResourcePath = "Dialogue/"; // Base path for dialogue JSON files in Resources
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;

    [Header("Settings")]
    public float textSpeed = 0.02f;

    private Story story;
    private Coroutine currentLineCoroutine;

    private bool isTyping;
    private bool dialogueActive;
    // void Start()
    // {
        
    //     StartDialogue("TestStory");
    // }

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

    // ================================
    // 🔹 EXTERNAL CALL ENTRY POINT
    // ================================
    public void StartDialogue(string knot = null)
    {
        TextAsset inkJSON = GetInkJSON("TestStory");

        if (inkJSON == null)
        {
            return;
        }

        story = new Story(inkJSON.text);

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
                    if (speakerNameText == null)
                    {
                        Debug.LogWarning("Speaker Name \"" + value + "\" Text reference is not assigned. Please assign a TextMeshProUGUI reference to speakerNameText in the inspector.");
                        return;
                    }
                    speakerNameText.text = value;
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
        // switch (func)
        // {
        //     case "StartQuest":
        //         QuestManager.Instance.StartQuest("Quest1");
        //         break;
        // }
        // note to self: make functions, make quest ongoing, finish quest, teleport player, fade screen.
    }
}