using UnityEngine;

public abstract class Power : MonoBehaviour
{
    [Header("Power Information")]
    [SerializeField] protected int slotNumber;
    [SerializeField] protected string powerName;
    [SerializeField] protected int scoreRequirement;

    [Header("Cooldown")]
    [SerializeField, Min(0f)] protected float cooldown = 5f;

    private float nextUseTime;

    public bool IsUnlocked { get; private set; }

    public int ScoreRequirement => scoreRequirement;

    public float CooldownRemaining =>
        Mathf.Max(0f, nextUseTime - Time.time);

    public float CooldownPercent =>
        cooldown > 0f ? CooldownRemaining / cooldown : 0f;

    public bool IsOnCooldown => CooldownRemaining > 0f;

    public bool CanUse =>
        IsUnlocked && !IsOnCooldown && Time.timeScale > 0f;

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

    protected void StartCooldown()
    {
        nextUseTime = Time.time + cooldown;
    }

    public virtual void Use()
    {
    }

    public virtual void ApplyEffect()
    {
    }
}