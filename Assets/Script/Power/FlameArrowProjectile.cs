using UnityEngine;

public class FlameArrowProjectile : Bullet
{
    [Header("Rotation")]
    [SerializeField] private float angleOffset;

    [Header("Homing")]
    [SerializeField] private float homingDelay = 0.3f;
    [SerializeField] private float rotateSpeed = 360f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float searchInterval = 0.2f;

    private Enemy target;
    private float spawnTime;
    private float nextSearchTime;

    private void Reset()
    {
        speed = 15f;
        lifeTime = 4f;
        damage = 40;
    }

    protected override void Awake()
    {
        base.Awake();

        spawnTime = Time.time;
    }

    public override void SetDirection(Vector2 shootDirection)
    {
        base.SetDirection(shootDirection);

        RotateToDirection(angleOffset);
    }

    protected override void FixedUpdate()
    {
        if (hasHit)
            return;

        if (Time.time >= spawnTime + homingDelay)
        {
            if (target == null || !target.isActiveAndEnabled)
            {
                if (Time.time >= nextSearchTime)
                {
                    FindNearestEnemy();
                    nextSearchTime = Time.time + searchInterval;
                }
            }

            if (target != null && target.isActiveAndEnabled)
                FollowTarget();
        }

        base.FixedUpdate();
    }

    private void FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(
            FindObjectsSortMode.None
        );

        target = null;

        float closestDistance =
            detectionRange * detectionRange;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null ||
                !enemy.isActiveAndEnabled)
                continue;

            float distance =
                ((Vector2)enemy.transform.position -
                 rb.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = enemy;
            }
        }
    }

    private void FollowTarget()
    {
        Vector2 targetDirection =
            (Vector2)target.transform.position -
            rb.position;

        if (targetDirection.sqrMagnitude < 0.001f)
            return;

        float currentAngle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg;

        float targetAngle =
            Mathf.Atan2(
                targetDirection.y,
                targetDirection.x
            ) * Mathf.Rad2Deg;

        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            rotateSpeed * Time.fixedDeltaTime
        );

        Vector2 newDirection = new Vector2(
            Mathf.Cos(newAngle * Mathf.Deg2Rad),
            Mathf.Sin(newAngle * Mathf.Deg2Rad)
        );

        SetDirection(newDirection);
    }
}