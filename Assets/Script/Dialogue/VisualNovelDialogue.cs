using UnityEngine;

public class VisualNovelDialogue : Dialogue
{
    [SerializeField] private string storyText;

    public override void ShowDialogue()
    {
        base.ShowDialogue();
    }

    public override void CloseDialogue()
    {
        base.CloseDialogue();
    }

    public void PlayStory()
    {
    }

    public void NextLine()
    {
    }
}