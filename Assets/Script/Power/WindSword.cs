using UnityEngine;

public class WindSword : Power
{
    [Header("Sword Wave")]
    [SerializeField] private SwordWaveProjectile swordWavePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int damage = 20;

    private Camera mainCamera;

    private void Reset()
    {
        slotNumber = 2;
        powerName = "Wind Sword";
        scoreRequirement = 5000;
        cooldown = 1f;
    }

    private void Awake()
    {
        mainCamera = Camera.main;

        if (firePoint == null)
            firePoint = transform;
    }

    public override void Use()
    {
        if (!CanUse || swordWavePrefab == null || mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, firePoint.position);

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector2 direction =
            (Vector2)ray.GetPoint(distance) -
            (Vector2)firePoint.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        SwordWaveProjectile wave = Instantiate(
            swordWavePrefab,
            firePoint.position,
            Quaternion.identity
        );

        wave.Initialize(direction.normalized, damage);

        StartCooldown();
    }
}