using UnityEngine;

public class ResearcherB : Enemy
{
    [SerializeField, Min(0.1f)] private float explosionRange = 1.5f;
    [SerializeField, Min(1)] private int explosionDamage = 2;
    [SerializeField] private GameObject explosionEffect;

    private Player target;

    protected override void Awake()
    {
        base.Awake();
        target = FindFirstObjectByType<Player>();
    }

    protected override void FixedUpdate()
    {
        if (isDead || target == null || !target.isActiveAndEnabled)
            return;

        Vector2 playerPosition = target.transform.position;

        if ((playerPosition - rb.position).sqrMagnitude <= explosionRange * explosionRange)
        {
            isDead = true;

            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            target.TakeDamage(explosionDamage);
            Destroy(gameObject);
            return;
        }

        rb.MovePosition(Vector2.MoveTowards(
            rb.position, playerPosition, moveSpeed * Time.fixedDeltaTime
        ));
    }

    protected override void Shoot()
    {
        // ระเบิดเมื่อเข้าระยะใน FixedUpdate
    }
}