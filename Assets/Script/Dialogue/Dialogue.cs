using UnityEngine;

public abstract class Dialogue : MonoBehaviour
{
    [SerializeField] protected string dialogueText;

    public virtual void ShowDialogue()
    {
    }

    public virtual void CloseDialogue()
    {
    }
}