using System.Collections;
using UnityEngine;

public abstract class Boss : Enemy
{
    [Header("Attack Patterns")]
    [SerializeField] private BossAttack[] attackPatterns;
    [SerializeField, Min(0f)] private float skillCooldown = 3f;

    [Header("Cooldown Shot")]
    [SerializeField, Min(1)] private int cooldownBulletCount = 7;
    [SerializeField, Range(0f, 360f)] private float cooldownSpread = 150f;

    [Header("Contact Damage")]
    [SerializeField, Min(1)] private int contactDamage = 1;

    private bool isAttacking;
    private float nextSkillTime;

    protected override void Shoot()
    {
        if (isDead || isAttacking)
            return;

        if (Time.time < nextSkillTime)
        {
            BossAttack.ShootFan(
                bulletPrefab, firePoint, cooldownBulletCount, cooldownSpread
            );
            return;
        }

        if (attackPatterns == null)
            return;

        int availableCount = 0;

        foreach (BossAttack attack in attackPatterns)
        {
            if (attack != null && attack.isActiveAndEnabled)
                availableCount++;
        }

        if (availableCount == 0)
            return;

        int selectedIndex = Random.Range(0, availableCount);

        foreach (BossAttack attack in attackPatterns)
        {
            if (attack == null || !attack.isActiveAndEnabled)
                continue;

            if (selectedIndex == 0)
            {
                StartCoroutine(RunAttack(attack));
                return;
            }

            selectedIndex--;
        }
    }

    private IEnumerator RunAttack(BossAttack attack)
    {
        isAttacking = true;
        yield return attack.Execute(this);

        nextSkillTime = Time.time + skillCooldown;
        isAttacking = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead)
            return;

        Player player = other.GetComponentInParent<Player>();

        if (player != null)
            player.TakeDamage(contactDamage);
    }

}