using System.Collections.Generic;
using UnityEngine;

public class BossChoiceDialogue : Dialogue
{
    [SerializeField] private List<BossChoice> choices = new List<BossChoice>();
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
        ShowDialogue();
    }

    public void PauseBossCombat()
    {
        Time.timeScale = 0f;
    }

    public void ShowChoices()
    {
        // เชื่อมปุ่มตัวเลือกตรงนี้เมื่อสร้าง UI ตัวเลือกบอส
    }

    public void ResolveChoice()
    {
        // จัดการตัวเลือกที่ผู้เล่นเลือกตรงนี้
    }

    public void ShowVictoryDialogue()
    {
        ShowDialogue();
    }
}