using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SalamanderGuardAttack : BossAttack
{
    [SerializeField] private Transform guardPivot;
    private Quaternion restPivotRotation;

    [SerializeField] private Transform guardTip;
    private Vector2 restArmDirection;

    [Header("Guard")]
    [SerializeField, Min(0f)] private float guardDuration = 0.6f;
    [SerializeField] private SpriteRenderer guardVisual;

    [Header("Guard Movement")]
    [SerializeField] private Player player;
    [SerializeField, Min(0.1f)] private float guardDistance = 1.4f;
    [SerializeField, Range(0f, 360f)] private float guardAngle = 360f;
    [SerializeField, Min(1f)] private float guardTurnSpeed = 120f;

    [SerializeField, Min(0f)] private float guardRaiseDuration = 0.2f;

    private BoxCollider2D guardCollider;
    private float currentGuardAngle;

    public bool IsGuarding { get; private set; }

    private void Awake()
    {
        guardCollider = GetComponent<BoxCollider2D>();
        guardCollider.isTrigger = true;
        SetGuard(false);

        if (guardPivot != null)
            restPivotRotation = guardPivot.localRotation;
        if (guardPivot != null && guardTip != null && guardPivot.parent != null)
        {
            restArmDirection = guardPivot.parent
                .InverseTransformVector(guardTip.position - guardPivot.position)
                .normalized;
        }
    }

    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null || !isActiveAndEnabled || guardDuration <= 0f)
            yield break;

        if (player == null)
            player = FindFirstObjectByType<Player>();

        float startAngle = currentGuardAngle;
        float targetAngle = GetTargetAngle(boss);
        float elapsed = 0f;

        // แสดงแขนและค่อย ๆ หมุนไปทางผู้เล่น
        if (guardVisual != null)
            guardVisual.enabled = true;

        while (elapsed < guardRaiseDuration &&
               boss != null && isActiveAndEnabled)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / guardRaiseDuration);

            currentGuardAngle = startAngle +
                Mathf.DeltaAngle(startAngle, targetAngle) * progress;

            ApplyGuardPose();
            yield return null;
        }

        if (boss == null || !isActiveAndEnabled)
        {
            SetGuard(false);
            yield break;
        }

        // หันเข้าหาผู้เล่นแล้วเปิดการป้องกันค้างไว้
        currentGuardAngle = targetAngle;
        ApplyGuardPose();
        SetGuard(true);

        yield return new WaitForSeconds(guardDuration);

        SetGuard(false);
    }

    private float GetTargetAngle(Boss boss)
    {
        if (player == null || !player.isActiveAndEnabled ||
            guardPivot == null || guardTip == null || guardPivot.parent == null)
            return 0f;

        Vector2 armToPlayer = guardPivot.parent
            .InverseTransformVector(player.transform.position - guardPivot.position)
            .normalized;

        return Mathf.Clamp(
            Vector2.SignedAngle(restArmDirection, armToPlayer),
            -guardAngle,
            guardAngle
        );
    }

    private void ApplyGuardPose()
    {
        if (guardPivot == null)
            return;

        guardPivot.localRotation =
            Quaternion.Euler(0f, 0f, currentGuardAngle) * restPivotRotation;
    }

    private void SetGuard(bool guarding)
    {
        IsGuarding = guarding;
        guardCollider.enabled = guarding;

        if (guardVisual != null)
            guardVisual.enabled = guarding;
    }

    private void OnDisable()
    {
        if (guardCollider != null)
            SetGuard(false);
    }
}