using UnityEngine;

public class Vampire : Enemy
{
    protected override void Shoot()
    {
        if (bulletPrefab == null ||
            firePoint == null ||
            !TryGetAimDirection(out Vector2 aimDirection))
        {
            return;
        }

        EnemyBullet energyBullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        energyBullet.SetDirection(aimDirection);
    }
}