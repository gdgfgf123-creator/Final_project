// WindSword.cs
using UnityEngine;

public class WindSword : Power
{
    [Header("Sword Wave")]
    [SerializeField] private SwordWaveProjectile swordWavePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField, Min(0)] private int damage = 20;
    [SerializeField, Min(0f)] private float cooldown = 1f;
    [SerializeField] private bool testMode = true;
    private Camera mainCamera;
    private float nextUseTime;

    private void Reset()
    {
        slotNumber = 2;
        powerName = "Wind Sword";
        scoreRequirement = 5000;
    }

    private void Awake()
    {
        mainCamera = Camera.main;

        if (firePoint == null)
            firePoint = transform;
    }

    public override void Use()
    {
        if (!IsUnlocked && !testMode)
            return;

        if (Time.timeScale == 0f ||
            Time.time < nextUseTime ||
            swordWavePrefab == null ||
            mainCamera == null)
        {
            return;
        }

        Vector2 direction = GetAimDirection();

        if (direction.sqrMagnitude < 0.001f)
            return;

        nextUseTime = Time.time + cooldown;

        SwordWaveProjectile wave = Instantiate(
            swordWavePrefab,
            firePoint.position,
            Quaternion.identity
        );

        wave.Initialize(direction, damage);
    }

    private Vector2 GetAimDirection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, firePoint.position);

        if (!plane.Raycast(ray, out float distance))
            return Vector2.up;

        Vector2 targetPosition = ray.GetPoint(distance);
        Vector2 direction = targetPosition - (Vector2)firePoint.position;

        return direction.sqrMagnitude > 0.001f
            ? direction.normalized
            : Vector2.up;
    }
}