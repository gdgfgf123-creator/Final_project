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
    [SerializeField] private Text characterNameText; // Text (Legacy)
    [SerializeField] private Text dialogueBodyText;  // Text (Legacy)
    [SerializeField] private Button nextButton;

    [Header("Dialogue Lines")]
    [SerializeField] private List<DialogueLine> lines = new List<DialogueLine>();

    private int currentLineIndex;
    private float previousTimeScale = 1f;
    private bool isOpen;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
        else
            Debug.LogError("ยังไม่ได้ใส่ Next Button ใน Inspector", this);
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
    private string currentBossName = "Boss";

    public void PlayStory(Boss boss)
    {
        if (boss != null)
            currentBossName = boss.GetType().Name;

        PlayStory();
    }
    public void PlayStory()
    {

        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning("ยังไม่มี Dialogue Lines", this);
            return;
        }

        if (dialoguePanel == null ||
            characterNameText == null ||
            dialogueBodyText == null ||
            nextButton == null)
        {
            Debug.LogError("กรุณาใส่ Dialogue Panel, Name Text, Body Text และ Next Button ใน Inspector", this);
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
        ShowCurrentLine();
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

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        DialogueLine line = lines[currentLineIndex];

        characterNameText.text = line.characterName == "{BOSS}"
            ? currentBossName
            : line.characterName;

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