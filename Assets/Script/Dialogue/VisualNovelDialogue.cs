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
    [SerializeField] private Text characterNameText;
    [SerializeField] private Text dialogueBodyText;
    [SerializeField] private Button nextButton;

    [Header("Dialogue Lines")]
    [SerializeField] private List<DialogueLine> lines = new List<DialogueLine>();

    private int currentLineIndex;
    private string currentBossName = "Boss";

    protected override void Awake()
    {
        base.Awake();

        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
        else
            Debug.LogError("ยังไม่ได้ใส่ Next Button", this);
    }

    public override void ShowDialogue()
    {
        PlayStory();
    }

    public override void CloseDialogue()
    {
        base.CloseDialogue();
    }

    public void PlayStory(Boss boss)
    {
        if (boss != null)
            currentBossName = boss.GetType().Name;

        PlayStory();
    }

    public void PlayStory()
    {
        if (IsOpen)
            return;

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
            Debug.LogError("กรุณาใส่ Dialogue UI ให้ครบ", this);
            return;
        }

        currentLineIndex = 0;

        base.ShowDialogue();

        ShowCurrentLine();
    }

    public void NextLine()
    {
        if (!IsOpen)
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

    protected override void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextLine);

        base.OnDestroy();
    }
}