using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Text characterNameText;
    [SerializeField] private TMP_Text dialogueBodyText;
    [SerializeField] private Button nextButton;

    [Header("Character Portraits")]
    [SerializeField] private Image playerImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite playerPortrait;

    [Header("Speaking Effect")]
    [SerializeField, Range(0f, 1f)]
    private float inactiveBrightness = 0.5f;

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

    public bool IsPlaying => IsOpen;

    protected override void Awake()
    {
        base.Awake();

        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
    }

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

        currentBossName = string.IsNullOrEmpty(data.bossDisplayName)
            ? bossType
            : data.bossDisplayName;

        currentBossPortrait = data.bossPortrait;

        BeginDialogue(data.dialogueLines);
    }

    private void BeginDialogue(List<DialogueLine> selectedLines)
    {
        if (selectedLines == null || selectedLines.Count == 0)
            return;

        if (dialoguePanel == null ||
            characterNameText == null ||
            dialogueBodyText == null ||
            nextButton == null)
        {
            Debug.LogError("กรุณาใส่ Dialogue UI ให้ครบ", this);
            return;
        }

        activeLines = selectedLines;
        currentLineIndex = 0;

        if (playerImage != null)
        {
            playerImage.sprite = playerPortrait;
            playerImage.enabled = false;
        }

        if (characterImage != null)
        {
            characterImage.sprite = currentBossPortrait;
            characterImage.enabled = false;
        }

        base.ShowDialogue();

        ShowCurrentLine();
    }

    public void NextLine()
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

        UpdatePortraits(line);
    }

    private void UpdatePortraits(DialogueLine line)
    {
        string speaker = line.characterName?.Trim();

        bool isZom =
            line.isPlayerSpeaking ||
            string.Equals(
                speaker, "Zom",
                StringComparison.OrdinalIgnoreCase
            ) ||
            speaker == "ซอม";

        if (playerImage != null)
        {
            playerImage.sprite = playerPortrait;

            playerImage.enabled =
                line.showPlayer && playerPortrait != null;

            playerImage.color = isZom
                ? Color.white
                : GetInactiveColor();
        }

        if (characterImage != null)
        {
            if (!isZom)
            {
                if (line.characterPortrait != null)
                {
                    characterImage.sprite =
                        line.characterPortrait;
                }
                else if (speaker == "{BOSS}")
                {
                    characterImage.sprite =
                        currentBossPortrait;
                }
            }

            characterImage.enabled =
                line.showCharacter &&
                characterImage.sprite != null;

            characterImage.color = isZom
                ? GetInactiveColor()
                : Color.white;
        }
    }

    private Color GetInactiveColor()
    {
        return new Color(
            inactiveBrightness,
            inactiveBrightness,
            inactiveBrightness,
            1f
        );
    }

    public override void CloseDialogue()
    {
        base.CloseDialogue();
        activeLines = null;
    }

    protected override void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextLine);

        base.OnDestroy();
    }
    public void PlayVictoryStory(Boss boss)
    {
        if (IsOpen || boss == null)
            return;

        string bossType = boss.GetType().Name;

        BossDialogueData data = bossDialogues.Find(
            item => item != null &&
            item.bossClassName == bossType
        );

        if (data == null)
            return;

        currentBossName = string.IsNullOrEmpty(data.bossDisplayName)
            ? bossType
            : data.bossDisplayName;

        currentBossPortrait = data.bossPortrait;

        BeginDialogue(data.victoryDialogueLines);
    }
}