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

    [Header("Victory Dialogue")]
    [SerializeField] private VisualNovelDialogue victoryDialogue;

    [Header("Boss Choice")]
    [SerializeField] private BossChoiceDialogue bossChoiceDialogue;

    [Header("Choice Effects")]
    [SerializeField] private float weaknessMultiplier = 1.35f;
    [SerializeField] private float enrageMultiplier = 2f;

    private readonly int[] choiceThresholds = { 80, 50, 20 };

    private int nextChoiceIndex;
    private int startingHP;

    private bool isAttacking;
    private bool isDefeated;
    private bool isChoosing;
    private bool hasShownEncounterDialogue;

    private float nextSkillTime;
    private float weaknessEndTime;
    private float enrageEndTime;

    private Coroutine attackCoroutine;

    public bool IsChoosing => isChoosing;
    public bool IsDefeated => isDefeated;
    public bool IsWeak => Time.time < weaknessEndTime;
    public bool IsEnraged => Time.time < enrageEndTime;

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
            encounterDialogue = FindBossDialogue();

        if (victoryDialogue == null)
            victoryDialogue = encounterDialogue;

        if (bossChoiceDialogue == null)
        {
            bossChoiceDialogue =
                FindFirstObjectByType<BossChoiceDialogue>();
        }

        hasShownEncounterDialogue = true;

        if (encounterDialogue != null)
            encounterDialogue.PlayStory(this);
    }

    private VisualNovelDialogue FindBossDialogue()
    {
        VisualNovelDialogue[] dialogues =
            FindObjectsByType<VisualNovelDialogue>(
                FindObjectsSortMode.None
            );

        foreach (VisualNovelDialogue dialogue in dialogues)
        {
            if (!(dialogue is OpeningDialogue))
                return dialogue;
        }

        return null;
    }

    protected virtual Vector2 GetShootDirection()
    {
        return Vector2.down;
    }

    protected override void Shoot()
    {
        if (isDead ||
            isDefeated ||
            isChoosing ||
            Time.timeScale == 0f)
        {
            return;
        }

        if (isAttacking ||
            !enableBossSkills ||
            Time.time < nextSkillTime)
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
        {
            ShootRegularVolley();
            return;
        }

        int availableCount = 0;

        foreach (BossAttack attack in patterns)
        {
            if (attack != null && attack.isActiveAndEnabled)
                availableCount++;
        }

        if (availableCount == 0)
        {
            ShootRegularVolley();
            return;
        }

        int selectedIndex = Random.Range(0, availableCount);

        foreach (BossAttack attack in patterns)
        {
            if (attack == null || !attack.isActiveAndEnabled)
                continue;

            if (selectedIndex == 0)
            {
                attackCoroutine = StartCoroutine(RunAttack(attack));
                return;
            }

            selectedIndex--;
        }
    }

    private void ShootRegularVolley()
    {
        int count = IsPhaseTwo
            ? phaseTwoBulletCount
            : cooldownBulletCount;

        float spread = IsPhaseTwo
            ? phaseTwoSpread
            : cooldownSpread;

        ShootFanInDirection(
            GetShootDirection(),
            count,
            spread
        );
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

        float startAngle =
            count > 1 ? -spreadAngle * 0.5f : 0f;

        float angleStep = 0f;

        if (count > 1)
        {
            int divisions =
                spreadAngle >= 360f ? count : count - 1;

            angleStep = spreadAngle / divisions;
        }

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector2 direction =
                Quaternion.Euler(0f, 0f, angle) *
                centerDirection;

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
        attackCoroutine = null;
    }

    // =========================
    // Boss Choice
    // =========================

    public override void TakeDamage(int damage)
    {
        if (isDead ||
            isDefeated ||
            isChoosing ||
            Time.timeScale == 0f ||
            damage <= 0)
        {
            return;
        }

        if (IsWeak)
        {
            damage = Mathf.RoundToInt(
                damage * weaknessMultiplier
            );
        }

        if (nextChoiceIndex < choiceThresholds.Length &&
            bossChoiceDialogue != null)
        {
            int percent = choiceThresholds[nextChoiceIndex];

            int thresholdHP = Mathf.Max(
                1,
                Mathf.CeilToInt(startingHP * percent / 100f)
            );

            if (currentHP - damage <= thresholdHP)
            {
                if (bossChoiceDialogue.StartChoice(this, percent))
                {
                    isChoosing = true;
                    nextChoiceIndex++;

                    int remainingDamage = Mathf.Max(
                        0,
                        currentHP - thresholdHP
                    );

                    if (remainingDamage > 0)
                        base.TakeDamage(remainingDamage);

                    return;
                }
            }
        }

        base.TakeDamage(damage);
    }

    public void EndBossChoice()
    {
        isChoosing = false;
    }

    public void ApplyWeakness(float duration)
    {
        weaknessEndTime = Time.time + duration;
    }

    public void ApplyEnrage(float duration)
    {
        enrageEndTime = Time.time + duration;
    }
    private IEnumerator RunBossChoice(int hpPercent)
    {
        isChoosing = true;

        if (bossChoiceDialogue == null)
            bossChoiceDialogue =
                FindFirstObjectByType<BossChoiceDialogue>();

        if (bossChoiceDialogue != null &&
            bossChoiceDialogue.StartChoice(this, hpPercent))
        {
            while (bossChoiceDialogue.IsPlaying)
                yield return null;
        }

        isChoosing = false;
    }
    protected override float GetFireInterval()
    {
        return IsEnraged
            ? base.GetFireInterval() /
              Mathf.Max(1f, enrageMultiplier)
            : base.GetFireInterval();
    }

    // =========================
    // Boss Defeated
    // =========================

    protected override void Die()
    {
        if (isDead || isDefeated)
            return;

        isDefeated = true;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        if (victoryDialogue == null)
            victoryDialogue = FindBossDialogue();

        if (victoryDialogue != null)
        {
            victoryDialogue.PlayVictoryStory(this);

            while (victoryDialogue != null &&
                   victoryDialogue.IsPlaying)
            {
                yield return null;
            }
        }

        base.Die();
    }

    protected override void FixedUpdate()
    {
        if (isDead || isDefeated || isChoosing)
            return;

        base.FixedUpdate();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead ||
            isDefeated ||
            isChoosing ||
            Time.timeScale == 0f)
        {
            return;
        }

        Player player = other.GetComponentInParent<Player>();

        if (player != null)
            player.TakeDamage(contactDamage);
    }
}