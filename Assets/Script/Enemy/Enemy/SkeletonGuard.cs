using UnityEngine;

public class SkeletonGuard : Enemy
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = FindFirstObjectByType<Player>();
    }

    protected override void Shoot()
    {
        if (bulletPrefab == null || firePoint == null ||
            player == null || !player.isActiveAndEnabled)
            return;

        Vector2 direction = player.transform.position - firePoint.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        EnemyBullet bone = Instantiate(
            bulletPrefab, firePoint.position, Quaternion.identity
        );
        bone.SetDirection(direction);
    }
}