using UnityEngine;

public class ZombieGuard : Enemy
{
    [SerializeField, Min(0.1f)] private float attackRange = 1.5f;
    [SerializeField, Min(1)] private int clubDamage = 1;

    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = FindFirstObjectByType<Player>();
    }

    protected override void FixedUpdate()
    {
        if (isDead || player == null || !player.isActiveAndEnabled)
            return;

        Vector2 playerPosition = player.transform.position;
        float distance = Vector2.Distance(rb.position, playerPosition);

        float stopDistance = Mathf.Max(0f, attackRange - 0.2f);
        float step = Mathf.Min(
            moveSpeed * Time.fixedDeltaTime,
            distance - stopDistance
        );

        if (step <= 0f)
            return;

        rb.MovePosition(Vector2.MoveTowards(rb.position, playerPosition, step));
    }

    protected override void Shoot()
    {
        if (isDead || player == null || !player.isActiveAndEnabled)
            return;

        Vector2 playerPosition = player.transform.position;

        if ((playerPosition - rb.position).sqrMagnitude <= attackRange * attackRange)
            player.TakeDamage(clubDamage);
    }
}