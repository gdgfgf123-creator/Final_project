using UnityEngine;

public abstract class Power : MonoBehaviour
{
    [SerializeField] protected int slotNumber;
    [SerializeField] protected string powerName;
    [SerializeField] protected int scoreRequirement;

    public virtual void Use()
    {
    }

    public virtual void ApplyEffect()
    {
    }
}