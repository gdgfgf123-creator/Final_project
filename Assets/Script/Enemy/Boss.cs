using System.Collections;
using UnityEngine;

public abstract class Boss : Enemy
{
    [SerializeField] private bool enableBossSkills = false;


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

    [Header("Encounter Dialogue")]
    [SerializeField] private VisualNovelDialogue encounterDialogue;

    private int startingHP;
    private bool isAttacking;
    private bool hasShownEncounterDialogue;
    private float nextSkillTime;

    protected bool IsPhaseTwo =>
        currentHP <= startingHP * phaseTwoHealthRatio;

    protected override void Awake()
    {
        base.Awake();
        startingHP = Mathf.Max(1, currentHP);
    }

    private void Start()
    {
        if (hasShownEncounterDialogue)
            return;

        if (encounterDialogue == null)
            encounterDialogue = FindFirstObjectByType<VisualNovelDialogue>();

        if (encounterDialogue == null)
        {
            Debug.LogWarning("VisualNovelDialogue was not found in the scene.", this);
            return;
        }

        hasShownEncounterDialogue = true;
        encounterDialogue.PlayStory(this);
    }

    protected virtual Vector2 GetShootDirection()
    {
        return Vector2.down;
    }

    protected override void Shoot()
    {
        if (isDead)
            return;

        if (!enableBossSkills || Time.time < nextSkillTime)
        {
            ShootRegularVolley();
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

    private void ShootRegularVolley()
    {
        int count = IsPhaseTwo ? phaseTwoBulletCount : cooldownBulletCount;
        float spread = IsPhaseTwo ? phaseTwoSpread : cooldownSpread;

        ShootFanInDirection(GetShootDirection(), count, spread);
    }

    private void ShootFanInDirection(
        Vector2 centerDirection,
        int bulletCount,
        float spreadAngle)
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        centerDirection = centerDirection.normalized;
        int count = Mathf.Max(1, bulletCount);

        float startAngle = count > 1 ? -spreadAngle * 0.5f : 0f;
        float angleStep = 0f;

        if (count > 1)
        {
            int divisions = spreadAngle >= 360f ? count : count - 1;
            angleStep = spreadAngle / divisions;
        }

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 direction =
                Quaternion.Euler(0f, 0f, angle) * centerDirection;

            EnemyBullet bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

            bullet.SetDirection(direction);
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