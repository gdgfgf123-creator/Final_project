using UnityEngine;

public class DemonFlower : Enemy
{
    [SerializeField] private float startAngle = 45f;

    protected override void FixedUpdate()
    {
        // ดอกไม้อยู่กับที่ในกระถาง
    }

    protected override void Shoot()
    {
        if (bulletPrefab == null || !TryGetAimDirection(out Vector2 aimDirection))
            return;

        for (int i = 0; i < 4; i++)
        {
            Vector2 direction =
                Quaternion.Euler(0f, 0f, i * 90f) * (Vector3)aimDirection;

            EnemyBullet fireball = Instantiate(
                bulletPrefab, firePoint.position, Quaternion.identity
            );

            fireball.SetDirection(direction);
        }
    }
}