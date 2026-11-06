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

        for (int i = 0; i < count; i++)
        {
            float angle = 0f;

            if (count > 1)
            {
                float position = (float)i / (count - 1);
                angle = Mathf.Lerp(
                    -spreadAngle * 0.5f,
                    spreadAngle * 0.5f,
                    position
                );
            }

            Vector3 direction =
                Quaternion.Euler(0f, 0f, angle) * Vector3.down;

            EnemyBullet bullet = Instantiate(
                prefab, firePoint.position, Quaternion.identity
            );

            bullet.SetDirection((Vector2)direction);
        }
    }
}