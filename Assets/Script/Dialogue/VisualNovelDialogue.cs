using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualNovelDialogue : Dialogue
{
    [Serializable]
    public class DialogueLine
    {
        public string characterName;

        [TextArea(2, 4)]
        public string text;
    }

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Text dialogueBodyText;
    [SerializeField] private Button nextButton;

    [Header("Lines")]
    [SerializeField] private List<DialogueLine> lines = new List<DialogueLine>
    {
        new DialogueLine
        {
            characterName = "DATURA",
            text = "คืนนี้ เลือดของเจ้าจะเป็นของข้า"
        }
    };

    private int currentLineIndex;
    private float previousTimeScale = 1f;
    private bool isOpen;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
    }

    public override void ShowDialogue()
    {
        PlayStory();
    }

    public override void CloseDialogue()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Time.timeScale = previousTimeScale;
    }

    public void PlayStory()
    {
        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning("VisualNovelDialogue has no dialogue lines.", this);
            return;
        }

        if (dialoguePanel == null || characterNameText == null || dialogueBodyText == null)
        {
            Debug.LogError(
                "Assign Dialogue Panel, Character Name Text, and Dialogue Body Text in the Inspector.",
                this
            );
            return;
        }

        if (!isOpen)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        isOpen = true;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayCurrentLine();
    }

    public void NextLine()
    {
        if (!isOpen)
            return;

        currentLineIndex++;

        if (currentLineIndex >= lines.Count)
        {
            CloseDialogue();
            return;
        }

        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        DialogueLine line = lines[currentLineIndex];

        characterNameText.text = line.characterName;
        dialogueBodyText.text = line.text;
    }

    private void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextLine);

        if (isOpen)
            Time.timeScale = previousTimeScale;
    }
}
