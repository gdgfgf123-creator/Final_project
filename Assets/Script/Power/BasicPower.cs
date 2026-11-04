using UnityEngine;

public class BasicPower : Power
{
    [SerializeField] private int damage = 10;

    private void Reset()
    {
        slotNumber = 1;
        powerName = "Basic Power";
        scoreRequirement = 0;
    }

    public override void Use()
    {
        base.Use();
    }
}