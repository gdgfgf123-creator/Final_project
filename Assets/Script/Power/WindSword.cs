using UnityEngine;

public class WindSword : Power
{
    [SerializeField] private int damage = 100;
    [SerializeField] private int hitCount = 2;

    private void Reset()
    {
        slotNumber = 2;
        powerName = "Wind Sword";
        scoreRequirement = 5000;
    }

    public override void Use()
    {
        base.Use();
    }

    public void HitTargetTwice()
    {
    }
}