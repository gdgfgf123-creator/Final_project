using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossChoiceDialogue : Dialogue
{
    [Serializable]
    public class ChoiceData
    {
        [TextArea(2, 5)]
        public string question;

        public string perfectAnswer;
        public string goodAnswer;
        public string badAnswer;
    }

    [Serializable]
    public class BossChoiceData
    {
        public string bossClassName;

        public ChoiceData hp80;
        public ChoiceData hp50;
        public ChoiceData hp20;
    }

    [Header("Choice UI")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text perfectText;
    [SerializeField] private TMP_Text goodText;
    [SerializeField] private TMP_Text badText;

    [Header("Buttons")]
    [SerializeField] private Button perfectButton;
    [SerializeField] private Button goodButton;
    [SerializeField] private Button badButton;

    [Header("Boss Choices")]
    [SerializeField] private BossChoiceData[] bossChoices;

    private Boss currentBoss;

    protected override void Awake()
    {
        base.Awake();

        if (perfectButton != null)
            perfectButton.onClick.AddListener(SelectPerfect);

        if (goodButton != null)
            goodButton.onClick.AddListener(SelectGood);

        if (badButton != null)
            badButton.onClick.AddListener(SelectBad);
    }

    public bool StartChoice(Boss boss, int hpPercent)
    {
        if (IsOpen || boss == null || bossChoices == null)
            return false;

        if (dialoguePanel == null ||
            questionText == null ||
            perfectText == null ||
            goodText == null ||
            badText == null ||
            perfectButton == null ||
            goodButton == null ||
            badButton == null)
        {
            Debug.LogError("Boss Choice UI is not assigned.", this);
            return false;
        }

        BossChoiceData data = Array.Find(
            bossChoices,
            item => item != null &&
            item.bossClassName == boss.GetType().Name
        );

        if (data == null)
        {
            Debug.LogWarning(
                "Boss Choice not found: " + boss.GetType().Name
            );

            return false;
        }

        ChoiceData choice = null;

        switch (hpPercent)
        {
            case 80:
                choice = data.hp80;
                break;

            case 50:
                choice = data.hp50;
                break;

            case 20:
                choice = data.hp20;
                break;
        }

        if (choice == null)
            return false;

        currentBoss = boss;

        questionText.text = choice.question;
        perfectText.text = choice.perfectAnswer;
        goodText.text = choice.goodAnswer;
        badText.text = choice.badAnswer;

        base.ShowDialogue();

        return IsOpen;
    }

    public void SelectPerfect()
    {
        ResolveChoice(0);
    }

    public void SelectGood()
    {
        ResolveChoice(1);
    }

    public void SelectBad()
    {
        ResolveChoice(2);
    }

    private void ResolveChoice(int result)
    {
        if (!IsOpen)
            return;

        Player player = FindFirstObjectByType<Player>();

        switch (result)
        {
            case 0:
                if (player != null)
                    player.Heal(1);

                if (currentBoss != null)
                    currentBoss.ApplyWeakness(5f);

                break;

            case 1:
                if (player != null)
                    player.Heal(1);

                break;

            case 2:
                if (currentBoss != null)
                    currentBoss.ApplyEnrage(5f);

                break;
        }

        CloseDialogue();
    }

    public override void CloseDialogue()
    {
        if (currentBoss != null)
            currentBoss.EndBossChoice();

        currentBoss = null;

        base.CloseDialogue();
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