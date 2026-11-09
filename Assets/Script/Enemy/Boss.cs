using System.Collections;
using UnityEngine;

public abstract class Boss : Enemy
{
    [Header("Phase 1 Skills")]
    [SerializeField] private BossAttack[] attackPatterns;

    [Header("Phase 2 Skills")]
    [SerializeField] private BossAttack[] phaseTwoPatterns;

    [SerializeField, Range(0f, 1f)]
    private float phaseTwoHealthRatio = 0.5f;

    [Header("Skill Cooldown")]
    [SerializeField, Min(0f)] private float skillCooldown = 3f;

    [Header("Cooldown Shot")]
    [SerializeField, Min(1)] private int cooldownBulletCount = 7;
    [SerializeField, Range(0f, 360f)] private float cooldownSpread = 150f;

    [Header("Phase 2 Cooldown Shot")]
    [SerializeField, Min(1)] private int phaseTwoBulletCount = 5;
    [SerializeField, Range(0f, 360f)] private float phaseTwoSpread = 100f;

    [Header("Contact Damage")]
    [SerializeField, Min(1)] private int contactDamage = 1;

    private int startingHP;
    private bool isAttacking;
    private float nextSkillTime;

    protected bool IsPhaseTwo =>
        currentHP <= startingHP * phaseTwoHealthRatio;

    protected override void Awake()
    {
        base.Awake();
        startingHP = Mathf.Max(1, currentHP);
    }

    protected override void Shoot()
    {
        if (isDead || isAttacking)
            return;

        if (Time.time < nextSkillTime)
        {
            int count = IsPhaseTwo
                ? phaseTwoBulletCount
                : cooldownBulletCount;

            float spread = IsPhaseTwo
                ? phaseTwoSpread
                : cooldownSpread;

            BossAttack.ShootFan(
                bulletPrefab, firePoint, count, spread
            );

            return;
        }

        BossAttack[] patterns = attackPatterns;

        if (IsPhaseTwo &&
            phaseTwoPatterns != null &&
            phaseTwoPatterns.Length > 0)
        {
            patterns = phaseTwoPatterns;
        }

        if (patterns == null)
            return;

        int availableCount = 0;

        foreach (BossAttack attack in patterns)
        {
            if (attack != null && attack.isActiveAndEnabled)
                availableCount++;
        }

        if (availableCount == 0)
            return;

        int selectedIndex = Random.Range(0, availableCount);

        foreach (BossAttack attack in patterns)
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