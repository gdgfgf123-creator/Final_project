using UnityEngine;

public class BatEnemy : Enemy
{
    [SerializeField, Min(0.1f)] private float dashSpeed = 8f;
    [SerializeField, Min(0.1f)] private float dashDuration = 0.6f;
    [SerializeField, Min(0.1f)] private float hitRange = 0.6f;
    [SerializeField, Min(1)] private int dashDamage = 1;

    private Player target;
    private Vector2 dashDirection;
    private float dashEndsAt;
    private bool hitThisDash;

    protected override void Shoot()
    {
        if (Time.time < dashEndsAt)
            return;

        target = FindFirstObjectByType<Player>();
        if (target == null || !target.isActiveAndEnabled)
            return;

        Vector2 offset = (Vector2)target.transform.position - rb.position;
        dashDirection = offset.normalized;
        dashEndsAt = Time.time + dashDuration;
        hitThisDash = false;
    }

    protected override void FixedUpdate()
    {
        if (isDead || Time.time >= dashEndsAt)
            return;

        if (!hitThisDash && target != null && target.isActiveAndEnabled)
        {
            Vector2 offset = (Vector2)target.transform.position - rb.position;

            if (offset.sqrMagnitude <= hitRange * hitRange)
            {
                hitThisDash = true;
                target.TakeDamage(dashDamage);
            }
        }

        rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
    }
}