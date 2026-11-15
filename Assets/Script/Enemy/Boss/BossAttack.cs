using System.Collections;
using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
    public abstract IEnumerator Execute(Boss boss);

    public static void ShootFan(
        EnemyBullet prefab,
        Transform firePoint,
        int bulletCount,
        float spreadAngle)
    {
        if (prefab == null || firePoint == null)
            return;

        int count = Mathf.Max(1, bulletCount);
        float spread = Mathf.Clamp(spreadAngle, 0f, 360f);
        float startAngle = 0f;
        float angleStep = 0f;

        if (count > 1)
        {
            int divisions = spread >= 360f ? count : count - 1;
            startAngle = -spread * 0.5f;
            angleStep = spread / divisions;
        }

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector3 direction =
                firePoint.rotation *
                Quaternion.Euler(0f, 0f, angle) *
                Vector3.down;

            EnemyBullet bullet = Instantiate(
                prefab, firePoint.position, Quaternion.identity
            );

            bullet.SetDirection((Vector2)direction);
        }
    }
}