using System.Collections;
using UnityEngine;

public class BelleAttack : BossAttack
{
    public enum AttackType
    {
        DeathSkull,
        MagicSpear,
        FallingBlades,
        DarkVortex,
        ReaperSlash
    }

    [Header("Attack Settings")]
    [SerializeField] private AttackType attackType;

    [SerializeField] private EnemyBullet projectilePrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField, Min(1)] private int bulletCount = 12;
    [SerializeField, Min(1)] private int repeatCount = 3;
    [SerializeField] private float attackInterval = 0.3f;
    [SerializeField] private float spreadAngle = 90f;

    [Header("Falling Blades")]
    [SerializeField] private float rainWidth = 4f;
    [SerializeField] private float rainHeight = 6f;

    [Header("Dark Vortex")]
    [SerializeField] private float spiralAngle = 25f;

    private Player player;

    public override IEnumerator Execute(Boss boss)
    {
        if (projectilePrefab == null || boss == null)
            yield break;

        player = FindFirstObjectByType<Player>();

        switch (attackType)
        {
            case AttackType.DeathSkull:
                yield return DeathSkull();
                break;

            case AttackType.MagicSpear:
                yield return MagicSpear();
                break;

            case AttackType.FallingBlades:
                yield return FallingBlades();
                break;

            case AttackType.DarkVortex:
                yield return DarkVortex();
                break;

            case AttackType.ReaperSlash:
                yield return ReaperSlash();
                break;
        }
    }

    private Vector3 FirePosition =>
        firePoint != null
            ? firePoint.position
            : transform.position;

    private Vector2 GetPlayerDirection()
    {
        if (player == null)
            return Vector2.down;

        return (
            player.transform.position -
            FirePosition
        ).normalized;
    }

    // Skill 1: กระสุนกะโหลกรอบตัว

    private IEnumerator DeathSkull()
    {
        for (int wave = 0; wave < repeatCount; wave++)
        {
            float offset = wave * 10f;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle =
                    360f / bulletCount * i + offset;

                Vector2 direction =
                    Quaternion.Euler(0f, 0f, angle) *
                    Vector2.up;

                SpawnBullet(FirePosition, direction);
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    // Skill 2: หอกเวทมนตร์

    private IEnumerator MagicSpear()
    {
        for (int wave = 0; wave < repeatCount; wave++)
        {
            ShootBelleFan(
                GetPlayerDirection(),
                bulletCount,
                spreadAngle
            );

            yield return new WaitForSeconds(attackInterval);
        }
    }

    // Skill 3: ใบมีดตกจากด้านบน

    private IEnumerator FallingBlades()
    {
        for (int wave = 0; wave < repeatCount; wave++)
        {
            if (player == null)
                yield break;

            Vector3 center = player.transform.position;

            for (int i = 0; i < bulletCount; i++)
            {
                float x = Random.Range(
                    -rainWidth / 2f,
                    rainWidth / 2f
                );

                Vector3 spawnPosition = new Vector3(
                    center.x + x,
                    center.y + rainHeight,
                    FirePosition.z
                );

                SpawnBullet(
                    spawnPosition,
                    Vector2.down
                );
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    // Skill 4: ลูกบอลเวทมนตร์หมุนเป็นเกลียว

    private IEnumerator DarkVortex()
    {
        float rotation = 0f;

        for (int wave = 0; wave < repeatCount; wave++)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                float angle =
                    360f / bulletCount * i + rotation;

                Vector2 direction =
                    Quaternion.Euler(0f, 0f, angle) *
                    Vector2.up;

                SpawnBullet(
                    FirePosition,
                    direction
                );
            }

            rotation += spiralAngle;

            yield return new WaitForSeconds(attackInterval);
        }
    }

    // Skill 5: คลื่นเคียว

    private IEnumerator ReaperSlash()
    {
        for (int wave = 0; wave < repeatCount; wave++)
        {
            ShootBelleFan(
                GetPlayerDirection(),
                bulletCount,
                spreadAngle
            );

            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void ShootBelleFan(
        Vector2 center,
        int count,
        float spread)
    {
        count = Mathf.Max(1, count);

        for (int i = 0; i < count; i++)
        {
            float angle = count > 1
                ? Mathf.Lerp(
                    -spread / 2f,
                    spread / 2f,
                    (float)i / (count - 1)
                )
                : 0f;

            Vector2 direction =
                Quaternion.Euler(0f, 0f, angle) *
                center;

            SpawnBullet(
                FirePosition,
                direction
            );
        }
    }

    private void SpawnBullet(
        Vector3 position,
        Vector2 direction)
    {
        EnemyBullet bullet = Instantiate(
            projectilePrefab,
            position,
            Quaternion.identity
        );

        bullet.SetDirection(direction.normalized);
    }
}