using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisualNovelDialogue : Dialogue
{
    [Serializable]
    public class DialogueLine
    {
        public string characterName;

        [TextArea(3, 10)]
        public string text;

        public bool showPlayer = true;
        public bool showCharacter = true;
        public bool isPlayerSpeaking;

        public Sprite characterPortrait;
    }

    [Serializable]
    public class BossDialogueData
    {
        public string bossClassName;
        public string bossDisplayName;
        public Sprite bossPortrait;

        [Header("Before Battle")]
        public List<DialogueLine> dialogueLines =
            new List<DialogueLine>();

        [Header("After Battle")]
        public List<DialogueLine> victoryDialogueLines =
            new List<DialogueLine>();
    }

    [Header("Dialogue UI")]
    [SerializeField] private TMP_Text dialogueBodyText;

    [Header("Dialogue Lines")]
    [SerializeField]
    private List<DialogueLine> lines =
        new List<DialogueLine>();

    [Header("Boss Dialogues")]
    [SerializeField]
    private List<BossDialogueData> bossDialogues =
        new List<BossDialogueData>();

    private List<DialogueLine> activeLines;

    private int currentLineIndex;

    private string currentBossName = "Boss";
    private Sprite currentBossPortrait;

    public override void ShowDialogue()
    {
        PlayStory();
    }

    public void PlayStory()
    {
        if (IsOpen)
            return;

        currentBossName = "Boss";
        currentBossPortrait = null;

        BeginDialogue(lines);
    }

    public void PlayStory(Boss boss)
    {
        PlayBossDialogue(boss, false);
    }

    public void PlayVictoryStory(Boss boss)
    {
        PlayBossDialogue(boss, true);
    }

    private void PlayBossDialogue(
        Boss boss,
        bool isVictory)
    {
        if (IsOpen || boss == null)
            return;

        string bossType = boss.GetType().Name;

        BossDialogueData data = bossDialogues.Find(
            item => item != null &&
            item.bossClassName == bossType
        );

        if (data == null)
        {
            Debug.LogWarning(
                "ไม่พบ Dialogue ของ " + bossType,
                this
            );

            return;
        }

        currentBossName =
            string.IsNullOrEmpty(data.bossDisplayName)
                ? bossType
                : data.bossDisplayName;

        currentBossPortrait = data.bossPortrait;

        List<DialogueLine> selectedLines = isVictory
            ? data.victoryDialogueLines
            : data.dialogueLines;

        BeginDialogue(selectedLines);
    }

    private void BeginDialogue(
        List<DialogueLine> selectedLines)
    {
        if (selectedLines == null ||
            selectedLines.Count == 0)
            return;

        if (dialoguePanel == null ||
            characterNameText == null ||
            dialogueBodyText == null ||
            nextButton == null)
        {
            Debug.LogError(
                "กรุณาใส่ Dialogue UI ให้ครบ",
                this
            );

            return;
        }

        activeLines = selectedLines;
        currentLineIndex = 0;

        SetupPortraits(currentBossPortrait);

        base.ShowDialogue();

        if (!IsOpen)
            return;

        ShowCurrentLine();
    }

    public override void NextLine()
    {
        if (!IsOpen || activeLines == null)
            return;

        currentLineIndex++;

        if (currentLineIndex >= activeLines.Count)
        {
            CloseDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        DialogueLine line = activeLines[currentLineIndex];

        characterNameText.text =
            line.characterName == "{BOSS}"
                ? currentBossName
                : line.characterName;

        dialogueBodyText.text = line.text;

        UpdatePortraits(
            line.characterName,
            line.showPlayer,
            line.showCharacter,
            line.isPlayerSpeaking,
            line.characterPortrait,
            currentBossPortrait
        );
    }

    public override void CloseDialogue()
    {
        activeLines = null;

        base.CloseDialogue();
    }
}