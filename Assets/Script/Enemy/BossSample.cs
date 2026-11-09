using UnityEngine;

public class BossSample : Boss
{
    [Header("Phase 1")]
    [SerializeField, Min(1)] private int bossBulletCount = 7;
    [SerializeField, Range(0f, 360f)]
    private float bossSpreadAngle = 120f;

    [Header("Phase 2")]
    [SerializeField, Range(0f, 1f)]
    private float samplePhaseTwoRatio = 0.5f;

    [SerializeField, Min(1)] private int secondPhaseBulletCount = 12;
    [SerializeField, Range(0f, 360f)]
    private float secondPhaseSpreadAngle = 240f;

    [SerializeField] private float rotateSpeed = 45f;

    private int initialHP;

    private bool InSecondPhase =>
        currentHP <= initialHP * samplePhaseTwoRatio;

    protected override void Awake()
    {
        base.Awake();
        initialHP = Mathf.Max(1, currentHP);
    }

    protected override void Update()
    {
        if (isDead)
            return;

        if (InSecondPhase && firePoint != null)
        {
            firePoint.Rotate(
                0f,
                0f,
                rotateSpeed * Time.deltaTime
            );
        }

        // ãªé¨Ñ§ËÇÐ Fire Interval ¨Ò¡ Enemy
        base.Update();
    }

    protected override void FixedUpdate()
    {
        // ºÍÊÂ×¹»ÃÐ¨Ó¨Ø´
    }

    protected override void Shoot()
    {
        if (isDead || bulletPrefab == null || firePoint == null)
            return;

        int count = InSecondPhase
            ? secondPhaseBulletCount
            : bossBulletCount;

        float spread = InSecondPhase
            ? secondPhaseSpreadAngle
            : bossSpreadAngle;

        count = Mathf.Max(1, count);
        spread = Mathf.Clamp(spread, 0f, 360f);

        float angleStep = 0f;
        float startAngle = 0f;

        if (count > 1)
        {
            int divisions = spread >= 360f ? count : count - 1;

            angleStep = spread / divisions;
            startAngle = -spread * 0.5f;
        }

        Vector3 baseDirection =
            firePoint.rotation * Vector3.down;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector3 direction =
                Quaternion.Euler(0f, 0f, angle) * baseDirection;

            EnemyBullet bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

            bullet.SetDirection((Vector2)direction);
        }
    }

    protected override void Die()
    {
        Debug.Log("Boss defeated!");
        base.Die();
    }
}