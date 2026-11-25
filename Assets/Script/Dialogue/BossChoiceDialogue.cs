using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossChoiceDialogue : Dialogue
{
    public enum ChoiceType
    {
        Perfect,
        Good,
        Bad
    }

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
    public class ChoiceData
    {
        [Header("Before Choice")]
        public DialogueLine[] introductionLines;

        [TextArea(2, 5)]
        public string question;

        [Header("Perfect Choice")]
        [TextArea(2, 5)]
        public string perfectAnswer;

        public DialogueLine[] perfectDialogue;

        [Header("Good Choice")]
        [TextArea(2, 5)]
        public string goodAnswer;

        public DialogueLine[] goodDialogue;

        [Header("Bad Choice")]
        [TextArea(2, 5)]
        public string badAnswer;

        public DialogueLine[] badDialogue;

        [Header("Choice Order")]
        public ChoiceType choice1 = ChoiceType.Perfect;
        public ChoiceType choice2 = ChoiceType.Good;
        public ChoiceType choice3 = ChoiceType.Bad;
    }

    [Serializable]
    public class BossChoiceData
    {
        public string bossClassName;
        public string bossDisplayName;
        public Sprite bossPortrait;

        public ChoiceData hp80 = new ChoiceData();
        public ChoiceData hp50 = new ChoiceData();
        public ChoiceData hp20 = new ChoiceData();
    }

    private enum DialogueState
    {
        None,
        Introduction,
        Choosing,
        Response
    }

    [Header("Choice UI")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text perfectText;
    [SerializeField] private TMP_Text goodText;
    [SerializeField] private TMP_Text badText;

    [SerializeField] private GameObject choicesPanel;

    [Header("Choice Buttons")]
    [SerializeField] private Button perfectButton;
    [SerializeField] private Button goodButton;
    [SerializeField] private Button badButton;

    [Header("Choice Effects")]
    [SerializeField, Min(0)]
    private int healAmount = 1;

    [SerializeField, Min(0f)]
    private float weaknessDuration = 5f;

    [SerializeField, Min(0f)]
    private float enrageDuration = 5f;

    [Header("Boss Choices")]
    [SerializeField] private BossChoiceData[] bossChoices;

    private Boss currentBoss;
    private BossChoiceData currentBossData;
    private ChoiceData currentChoice;

    private DialogueLine[] activeLines;
    private int currentLineIndex;

    private DialogueState state = DialogueState.None;
    private ChoiceType selectedChoice;

    protected override void Awake()
    {
        base.Awake();

        if (perfectButton != null)
            perfectButton.onClick.AddListener(SelectPerfect);

        if (goodButton != null)
            goodButton.onClick.AddListener(SelectGood);

        if (badButton != null)
            badButton.onClick.AddListener(SelectBad);

        SetChoicesVisible(false);
    }

    public bool StartChoice(Boss boss, int hpPercent)
    {
        if (IsOpen || boss == null || bossChoices == null)
            return false;

        if (!ValidateUI())
            return false;

        BossChoiceData data = Array.Find(
            bossChoices,
            item => item != null &&
            item.bossClassName == boss.GetType().Name
        );

        if (data == null)
        {
            Debug.LogWarning(
                "ไม่พบ Boss Choice ของ " +
                boss.GetType().Name,
                this
            );

            return false;
        }

        ChoiceData choice = GetChoiceData(data, hpPercent);

        if (choice == null)
            return false;

        if (choice.choice1 == choice.choice2 ||
            choice.choice1 == choice.choice3 ||
            choice.choice2 == choice.choice3)
        {
            Debug.LogError(
                "Choice Order ต้องไม่ซ้ำกัน",
                this
            );

            return false;
        }

        currentBoss = boss;
        currentBossData = data;
        currentChoice = choice;

        state = DialogueState.Introduction;

        SetChoicesVisible(false);
        SetupPortraits(data.bossPortrait);

        base.ShowDialogue();

        if (!IsOpen)
            return false;

        BeginDialogue(choice.introductionLines);

        return true;
    }

    private bool ValidateUI()
    {
        bool valid =
            dialoguePanel != null &&
            nextButton != null &&
            questionText != null &&
            choicesPanel != null &&
            perfectText != null &&
            goodText != null &&
            badText != null &&
            perfectButton != null &&
            goodButton != null &&
            badButton != null;

        if (!valid)
        {
            Debug.LogError(
                "Boss Choice UI is not assigned.",
                this
            );
        }

        return valid;
    }

    private ChoiceData GetChoiceData(
        BossChoiceData data,
        int hpPercent)
    {
        switch (hpPercent)
        {
            case 80:
                return data.hp80;

            case 50:
                return data.hp50;

            case 20:
                return data.hp20;

            default:
                return null;
        }
    }

    private void BeginDialogue(DialogueLine[] lines)
    {
        activeLines = lines;
        currentLineIndex = 0;

        SetChoicesVisible(false);

        questionText.gameObject.SetActive(true);

        SetNextVisible(true);
        SetSkipVisible(true);

        if (activeLines == null ||
            activeLines.Length == 0)
        {
            FinishDialogue();
            return;
        }

        ShowCurrentLine();
    }

    public override void NextLine()
    {
        if (!IsOpen ||
            state == DialogueState.Choosing ||
            state == DialogueState.None ||
            activeLines == null)
            return;

        currentLineIndex++;

        if (currentLineIndex >= activeLines.Length)
        {
            FinishDialogue();
            return;
        }

        ShowCurrentLine();
    }

    public override void Skip()
    {
        if (!IsOpen ||
            state == DialogueState.Choosing ||
            state == DialogueState.None)
            return;

        FinishDialogue();
    }

    private void ShowCurrentLine()
    {
        DialogueLine line = activeLines[currentLineIndex];

        if (characterNameText != null)
        {
            characterNameText.text =
                line.characterName == "{BOSS}"
                    ? currentBossData.bossDisplayName
                    : line.characterName;
        }

        questionText.text = line.text;

        UpdatePortraits(
            line.characterName,
            line.showPlayer,
            line.showCharacter,
            line.isPlayerSpeaking,
            line.characterPortrait,
            currentBossData.bossPortrait
        );
    }

    private void FinishDialogue()
    {
        switch (state)
        {
            case DialogueState.Introduction:

                ShowChoices();
                break;

            case DialogueState.Response:

                ApplyChoiceEffect();
                CloseDialogue();
                break;
        }
    }

    private void ShowChoices()
    {
        state = DialogueState.Choosing;

        SetNextVisible(false);
        SetSkipVisible(false);

        questionText.gameObject.SetActive(false);

        if (characterNameText != null)
            characterNameText.text = "Zom";

        perfectText.text =
            GetAnswer(currentChoice.choice1);

        goodText.text =
            GetAnswer(currentChoice.choice2);

        badText.text =
            GetAnswer(currentChoice.choice3);

        UpdatePortraits(
            "Zom",
            true,
            true,
            true,
            null,
            currentBossData.bossPortrait
        );

        SetChoicesVisible(true);
    }

    private string GetAnswer(ChoiceType type)
    {
        switch (type)
        {
            case ChoiceType.Perfect:
                return currentChoice.perfectAnswer;

            case ChoiceType.Good:
                return currentChoice.goodAnswer;

            case ChoiceType.Bad:
                return currentChoice.badAnswer;

            default:
                return "";
        }
    }

    private DialogueLine[] GetResponse(ChoiceType type)
    {
        switch (type)
        {
            case ChoiceType.Perfect:
                return currentChoice.perfectDialogue;

            case ChoiceType.Good:
                return currentChoice.goodDialogue;

            case ChoiceType.Bad:
                return currentChoice.badDialogue;

            default:
                return null;
        }
    }

    private void SetChoicesVisible(bool visible)
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(visible);
    }

    public void SelectPerfect()
    {
        if (currentChoice != null)
            ResolveChoice(currentChoice.choice1);
    }

    public void SelectGood()
    {
        if (currentChoice != null)
            ResolveChoice(currentChoice.choice2);
    }

    public void SelectBad()
    {
        if (currentChoice != null)
            ResolveChoice(currentChoice.choice3);
    }

    private void ResolveChoice(ChoiceType result)
    {
        if (!IsOpen ||
            state != DialogueState.Choosing)
            return;

        selectedChoice = result;

        state = DialogueState.Response;

        SetChoicesVisible(false);

        BeginDialogue(GetResponse(result));
    }

    private void ApplyChoiceEffect()
    {
        Player player = FindFirstObjectByType<Player>();

        switch (selectedChoice)
        {
            case ChoiceType.Perfect:

                if (player != null)
                    player.Heal(healAmount);

                if (currentBoss != null)
                {
                    currentBoss.ApplyWeakness(
                        weaknessDuration
                    );
                }

                Debug.Log("Perfect Choice!");
                break;

            case ChoiceType.Good:

                if (player != null)
                    player.Heal(healAmount);

                Debug.Log("Good Choice!");
                break;

            case ChoiceType.Bad:

                if (currentBoss != null)
                {
                    currentBoss.ApplyEnrage(
                        enrageDuration
                    );
                }

                Debug.Log("Bad Choice!");
                break;
        }
    }

    public override void CloseDialogue()
    {
        Boss finishedBoss = currentBoss;

        SetChoicesVisible(false);

        currentBoss = null;
        currentBossData = null;
        currentChoice = null;

        activeLines = null;
        state = DialogueState.None;

        base.CloseDialogue();

        if (finishedBoss != null)
            finishedBoss.EndBossChoice();
    }

    protected override void OnDestroy()
    {
        if (perfectButton != null)
            perfectButton.onClick.RemoveListener(SelectPerfect);

        if (goodButton != null)
            goodButton.onClick.RemoveListener(SelectGood);

        if (badButton != null)
            badButton.onClick.RemoveListener(SelectBad);

        if (currentBoss != null)
            currentBoss.EndBossChoice();

        base.OnDestroy();
    }
}