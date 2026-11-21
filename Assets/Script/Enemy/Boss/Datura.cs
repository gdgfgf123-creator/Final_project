using System.Collections;
using UnityEngine;

public class Datura : Boss
{
    [Header("Aiming")]
    [SerializeField] private Player player;

    [Header("Transformation")]
    [SerializeField] private SpriteRenderer bodyVisual;
    [SerializeField] private Sprite phaseTwoSprite;
    [SerializeField, Min(0f)] private float transformationDuration = 1f;
    [SerializeField, Min(1f)] private float phaseTwoMoveSpeedMultiplier = 1.5f;

    [Header("Blood Orb Shield")]
    [SerializeField] private EnemyBullet bloodOrbPrefab;
    [SerializeField, Min(1f)] private float shieldCoverage = 1.2f;

    private float phaseOneMoveSpeed;
    private bool transformationStarted;
    private bool isTransforming;
    private EnemyBullet shieldOrb;

    protected override void Awake()
    {
        base.Awake();

        phaseOneMoveSpeed = moveSpeed;

        if (bodyVisual == null)
            bodyVisual = GetComponentInChildren<SpriteRenderer>();
    }

    public override void TakeDamage(int damage)
    {
        if (isDead || isTransforming || damage <= 0)
            return;

        bool wasPhaseTwo = IsPhaseTwo;
        base.TakeDamage(damage);

        if (!isDead &&
            !transformationStarted &&
            !wasPhaseTwo &&
            IsPhaseTwo)
        {
            transformationStarted = true;
            StartCoroutine(TransformToPhaseTwo());
        }
    }

    protected override void Shoot()
    {
        if (isTransforming)
            return;

        base.Shoot();
    }

    protected override void FixedUpdate()
    {
        if (isTransforming)
            return;

        moveSpeed = IsPhaseTwo
            ? phaseOneMoveSpeed * phaseTwoMoveSpeedMultiplier
            : phaseOneMoveSpeed;

        base.FixedUpdate();
    }

    protected override Vector2 GetShootDirection()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (player == null ||
            !player.isActiveAndEnabled ||
            firePoint == null)
        {
            return Vector2.down;
        }

        Vector2 direction =
            (Vector2)player.transform.position -
            (Vector2)firePoint.position;

        return direction.sqrMagnitude > 0.001f
            ? direction.normalized
            : Vector2.down;
    }

    private IEnumerator TransformToPhaseTwo()
    {
        isTransforming = true;
        SpawnBloodOrbShield();

        yield return new WaitForSeconds(transformationDuration);

        if (bodyVisual != null && phaseTwoSprite != null)
            bodyVisual.sprite = phaseTwoSprite;

        moveSpeed = phaseOneMoveSpeed * phaseTwoMoveSpeedMultiplier;

        if (shieldOrb != null)
            Destroy(shieldOrb.gameObject);

        shieldOrb = null;
        isTransforming = false;
    }

    private void SpawnBloodOrbShield()
    {
        if (bloodOrbPrefab == null || bodyVisual == null || phaseTwoSprite == null)
        {
            Debug.LogWarning(
                "Datura: Assign Blood Orb Prefab, Body Visual, and Phase Two Sprite.",
                this
            );
            return;
        }

        shieldOrb = Instantiate(
            bloodOrbPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );

        shieldOrb.enabled = false;

        foreach (Collider2D orbCollider in
                 shieldOrb.GetComponentsInChildren<Collider2D>())
        {
            orbCollider.enabled = false;
        }

        Rigidbody2D orbRigidbody = shieldOrb.GetComponent<Rigidbody2D>();
        if (orbRigidbody != null)
            orbRigidbody.simulated = false;

        SpriteRenderer orbVisual =
            shieldOrb.GetComponentInChildren<SpriteRenderer>();

        if (orbVisual == null)
            return;

        // วางลูก Orb ให้ตรงกลางร่าง Phase 2
        Vector3 targetCenter =
            bodyVisual.transform.TransformPoint(phaseTwoSprite.bounds.center);

        shieldOrb.transform.position += targetCenter - orbVisual.bounds.center;

        // คำนวณให้ Orb ใหญ่พอครอบรูป Phase 2
        Vector3 worldSize = Vector3.Scale(
            phaseTwoSprite.bounds.size,
            bodyVisual.transform.lossyScale
        );

        float targetSize = worldSize.magnitude * shieldCoverage;
        float orbSize = Mathf.Max(orbVisual.bounds.size.x, orbVisual.bounds.size.y);

        if (orbSize > 0.001f)
            shieldOrb.transform.localScale *= targetSize / orbSize;

        // ให้วาด Orb อยู่หน้าภาพบอส
        orbVisual.sortingLayerID = bodyVisual.sortingLayerID;
        orbVisual.sortingOrder = bodyVisual.sortingOrder + 1;
    }

    protected override void Die()
    {
        Debug.Log("Datura defeated!");
        base.Die();
    }
}