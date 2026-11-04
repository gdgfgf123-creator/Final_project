using System.Collections.Generic;
using UnityEngine;

public class BossChoiceDialogue : Dialogue
{
    [SerializeField]
    private List<BossChoice> choices =
        new List<BossChoice>();

    [SerializeField] private int triggerHPThreshold;

    public override void ShowDialogue()
    {
        base.ShowDialogue();
    }

    public override void CloseDialogue()
    {
        base.CloseDialogue();
    }

    public void StartDialogue()
    {
    }

    public void PauseBossCombat()
    {
    }

    public void ShowChoices()
    {
    }

    public void ResolveChoice()
    {
    }

    public void ShowVictoryDialogue()
    {
    }
}