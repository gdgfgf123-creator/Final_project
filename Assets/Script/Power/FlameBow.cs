using UnityEngine;

public class FlameBow : Power
{
    [Header("Flame Bow")]
    [SerializeField] private FlameArrowProjectile arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int damage = 40;

    [Header("Spread Shot")]
    [SerializeField, Min(1)] private int arrowCount = 3;
    [SerializeField] private float spreadAngle = 60f;

    private Camera mainCamera;

    private void Reset()
    {
        slotNumber = 4;
        powerName = "Flame Bow";
        scoreRequirement = 25000;
        cooldown = 5f;
    }

    private void Awake()
    {
        mainCamera = Camera.main;

        if (firePoint == null)
            firePoint = transform;
    }

    public override void Use()
    {
        if (!CanUse || arrowPrefab == null || mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(
            Vector3.forward,
            firePoint.position
        );

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector2 direction =
            (Vector2)ray.GetPoint(distance) -
            (Vector2)firePoint.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        int count = Mathf.Max(1, arrowCount);

        for (int i = 0; i < count; i++)
        {
            float angle = count > 1
                ? Mathf.Lerp(
                    -spreadAngle / 2f,
                    spreadAngle / 2f,
                    (float)i / (count - 1)
                )
                : 0f;

            Vector2 shootDirection =
                Quaternion.Euler(0f, 0f, angle) * direction;

            FlameArrowProjectile arrow = Instantiate(
                arrowPrefab,
                firePoint.position,
                Quaternion.identity
            );

            arrow.Initialize(shootDirection, damage);
        }

        StartCooldown();
    }
}