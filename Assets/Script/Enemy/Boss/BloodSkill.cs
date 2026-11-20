using System.Collections;
using UnityEngine;

public class BloodSkill : BossAttack
{
    public enum SkillType
    {
        BloodOrb,
        BloodSpear,
        BloodPillar,
        DualPistols
    }

    [Header("Skill")]
    [SerializeField] private SkillType skillType;
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Player player;

    [Header("Dual Pistols")]
    [SerializeField] private Transform[] pistolFirePoints;
    [SerializeField, Min(1)] private int pistolShotCount = 6;
    [SerializeField, Min(0f)] private float pistolShotInterval = 0.15f;

    [Header("Projectile Pattern")]
    [SerializeField, Min(1)] private int projectileCount = 8;
    [SerializeField, Range(0f, 360f)] private float spreadAngle = 70f;

    [Header("Pistol Visuals")]
    [SerializeField] private GameObject[] pistolVisuals;

    [Header("Blood Pillar")]
    [SerializeField] private BloodPillarTrap bloodPillarPrefab;
    [SerializeField, Min(1)] private int pillarCount = 3;
    [SerializeField, Min(0f)] private float pillarSpacing = 0.8f;
    [SerializeField, Min(0f)]
    private float pillarDistanceBeforePlayer = 1f;
    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null)
            yield break;

        if (player == null)
            player = FindFirstObjectByType<Player>();

        switch (skillType)
        {
            case SkillType.BloodOrb:
                if (bulletPrefab != null)
                    ShootPattern(firePoint, projectileCount, spreadAngle);
                break;

            case SkillType.BloodSpear:
                if (bulletPrefab != null)
                    ShootPattern(firePoint, 1, 0f);
                break;

            case SkillType.BloodPillar:
                SpawnBloodPillars();
                break;

            case SkillType.DualPistols:
                if (bulletPrefab != null)
                    yield return ShootPistols();
                break;
        }
    }

    private void Awake()
    {
        SetPistolsVisible(false);
    }

    private IEnumerator ShootPistols()
    {
        SetPistolsVisible(true);

        int shotCount = Mathf.Max(1, pistolShotCount);

        for (int i = 0; i < shotCount; i++)
        {
            if (pistolFirePoints != null && pistolFirePoints.Length > 0)
            {
                // ยิงจากปืนทั้งสองกระบอกในแต่ละชุด
                foreach (Transform point in pistolFirePoints)
                {
                    if (point != null)
                        ShootPattern(point, 1, 0f);
                }
            }
            else
            {
                ShootPattern(firePoint, 1, 0f);
            }

            if (i < shotCount - 1)
                yield return new WaitForSeconds(pistolShotInterval);
        }

        SetPistolsVisible(false);
    }

    private void SetPistolsVisible(bool visible)
    {
        if (pistolVisuals == null)
            return;

        foreach (GameObject pistol in pistolVisuals)
        {
            if (pistol != null)
                pistol.SetActive(visible);
        }
    }

    private void OnDisable()
    {
        SetPistolsVisible(false);
    }

    private void ShootPattern(Transform point, int count, float spread)
    {
        if (point == null || bulletPrefab == null)
            return;

        Vector2 centerDirection = GetDirectionToPlayer(point.position);
        int total = Mathf.Max(1, count);

        float startAngle = total > 1 ? -spread * 0.5f : 0f;
        float angleStep = 0f;

        if (total > 1)
        {
            int divisions = spread >= 360f ? total : total - 1;
            angleStep = spread / divisions;
        }

        for (int i = 0; i < total; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 direction =
                Quaternion.Euler(0f, 0f, angle) * centerDirection;

            EnemyBullet bullet = Instantiate(
                bulletPrefab,
                point.position,
                Quaternion.identity
            );

            bullet.SetDirection(direction);
        }
    }

    private void SpawnBloodPillars()
    {
        if (bloodPillarPrefab == null ||
            player == null ||
            !player.isActiveAndEnabled)
        {
            return;
        }

        Vector2 origin = firePoint != null
            ? firePoint.position
            : transform.position;

        Vector2 direction = GetDirectionToPlayer(origin);
        Vector2 targetPosition =
            (Vector2)player.transform.position
            - direction * pillarDistanceBeforePlayer;

        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        int total = Mathf.Max(1, pillarCount);

        for (int i = 0; i < total; i++)
        {
            float offset = (i - (total - 1) * 0.5f) * pillarSpacing;
            Vector2 spawnPosition = targetPosition + perpendicular * offset;

            Instantiate(
                bloodPillarPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }
    }

    private Vector2 GetDirectionToPlayer(Vector2 origin)
    {
        if (player == null || !player.isActiveAndEnabled)
            return Vector2.down;

        Vector2 direction = (Vector2)player.transform.position - origin;

        return direction.sqrMagnitude > 0.001f
            ? direction.normalized
            : Vector2.down;
    }
}