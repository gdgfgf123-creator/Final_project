using UnityEngine;

public class FlameBow : Power
{
    [SerializeField] private int damage = 700;

    private void Reset()
    {
        slotNumber = 3;
        powerName = "Flame Bow";
        scoreRequirement = 25000;
    }

    public override void Use()
    {
        base.Use();
    }

    public void ShootFireArrow()
    {
    }
}