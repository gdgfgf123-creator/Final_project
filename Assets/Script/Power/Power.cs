using UnityEngine;

public abstract class Power : MonoBehaviour
{
    [SerializeField] protected int slotNumber;
    [SerializeField] protected string powerName;
    [SerializeField] protected int scoreRequirement;

    public bool IsUnlocked { get; private set; }

    public void CheckUnlock(int score)
    {
        if (IsUnlocked)
            return;

        if (score >= scoreRequirement)
        {
            IsUnlocked = true;
            Debug.Log(powerName + " Unlocked!");
        }
    }

    public virtual void Use()
    {
    }

    public virtual void ApplyEffect()
    {
    }
}