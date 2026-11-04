using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Attack")]
    [SerializeField, Min(1)]
    private int bulletCount = 7;

    [SerializeField, Range(0f, 360f)]
    private float spreadAngle = 120f;

    // ไม่ใช้การเดินซ้าย–ขวาของ Enemy
    protected override void FixedUpdate()
    {
    }

    protected override void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        int count = Mathf.Max(1, bulletCount);

        // วงกลมเต็มไม่สร้างกระสุนซ้ำที่มุมเริ่มกับมุมสุดท้าย
        float angleStep = count <= 1
            ? 0f
            : spreadAngle / (spreadAngle >= 360f ? count : count - 1);

        float startAngle = count <= 1 ? 0f : -spreadAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector3 direction =
                Quaternion.Euler(0f, 0f, angle) * Vector3.down;

            EnemyBullet bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

            bullet.SetDirection(
                new Vector2(direction.x, direction.y)
            );
        }
    }

    protected override void Die()
    {
        Debug.Log("Boss defeated!");
        base.Die();
    }
}