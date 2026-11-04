using UnityEngine;

public class GraveShield : Power
{
    [SerializeField] private int duration = 2;

    private void Reset()
    {
        slotNumber = 3;
        powerName = "Grave Shield";
        scoreRequirement = 10000;
    }

    public override void Use()
    {
        base.Use();
    }

    public void BlockIncomingDamage()
    {
    }
}