using UnityEngine;

public class ResearcherA : Enemy
{
    [SerializeField, Range(0f, 90f)] private float paperSpread = 25f;
    [SerializeField] private Animator animator;

    protected override void Shoot()
    {
        if (bulletPrefab == null || !TryGetAimDirection(out Vector2 aimDirection))
            return;

        if (animator != null)
            animator.SetTrigger("Slip");

        for (int i = -1; i <= 1; i++)
        {
            Vector2 direction = Quaternion.Euler(0f, 0f, i * paperSpread)
                * (Vector3)aimDirection;

            EnemyBullet paper = Instantiate(
                bulletPrefab, firePoint.position, Quaternion.identity
            );

            paper.SetDirection(direction);
        }
    }
}