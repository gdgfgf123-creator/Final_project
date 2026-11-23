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

        public bool isPlayerSpeaking;
        public Sprite characterPortrait;
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

    private int currentLineIndex;
    private string currentBossName = "Boss";

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

    public override void CloseDialogue()
    {
        base.CloseDialogue();
    }

    public void PlayStory(Boss boss)
    {
        if (IsOpen)
            return;

        if (boss != null)
            currentBossName = boss.GetType().Name;

        PlayStory();
    }

    public void PlayStory()
    {
        if (IsOpen)
            return;

        if (lines == null || lines.Count == 0)
            return;

        if (dialoguePanel == null ||
            characterNameText == null ||
            dialogueBodyText == null ||
            nextButton == null)
        {
            Debug.LogError("กรุณาใส่ Dialogue UI ให้ครบ", this);
            return;
        }

        currentLineIndex = 0;

        if (playerImage != null)
        {
            playerImage.sprite = playerPortrait;
            playerImage.enabled = playerPortrait != null;
        }

        if (characterImage != null)
        {
            characterImage.sprite = null;
            characterImage.enabled = false;
        }

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

        UpdatePortraits(line);
    }

    private void UpdatePortraits(DialogueLine line)
    {
        if (playerImage != null)
        {
            playerImage.color = line.isPlayerSpeaking
                ? Color.white
                : GetInactiveColor();
        }

        if (characterImage != null)
        {
            if (!line.isPlayerSpeaking &&
                line.characterPortrait != null)
            {
                characterImage.sprite = line.characterPortrait;
                characterImage.enabled = true;
            }

            characterImage.color = line.isPlayerSpeaking
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

    protected override void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextLine);

        base.OnDestroy();
    }
}