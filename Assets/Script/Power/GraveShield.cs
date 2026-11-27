using UnityEngine;

public class GraveShield : Power
{
    [Header("Shield Settings")]
    [SerializeField] private GameObject shieldVisual;

    [SerializeField, Min(0.1f)]
    private float shieldDuration = 3f;

    private float shieldEndTime;

    public bool IsShieldActive { get; private set; }

    private void Reset()
    {
        slotNumber = 3;
        powerName = "Grave Shield";
        scoreRequirement = 10000;
        cooldown = 10f;
    }

    private void Awake()
    {
        DeactivateShield();
    }

    public override void Use()
    {
        if (!CanUse || IsShieldActive)
            return;

        ApplyEffect();
        StartCooldown();
    }

    public override void ApplyEffect()
    {
        IsShieldActive = true;

        shieldEndTime = Time.time + shieldDuration;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        Debug.Log("Grave Shield Activated!");
    }

    private void Update()
    {
        if (IsShieldActive && Time.time >= shieldEndTime)
            DeactivateShield();
    }

    private void DeactivateShield()
    {
        IsShieldActive = false;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    private void OnDisable()
    {
        DeactivateShield();
    }
}