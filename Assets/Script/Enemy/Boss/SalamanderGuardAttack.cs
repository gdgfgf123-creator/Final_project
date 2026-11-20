using System.Collections;
using UnityEngine;

public class SalamanderGuardAttack : BossAttack
{
    [Header("Arm Pivot")]
    [SerializeField] private Transform guardPivot; // จุดหมุนกลางบอส
    [SerializeField] private SpriteRenderer guardVisual;
    [SerializeField] private BoxCollider2D guardCollider;

    [Header("Guard")]
    [SerializeField, Min(0f)] private float guardDuration = 0.6f;
    [SerializeField, Min(1f)] private float turnSpeed = 360f;

    [Header("Target")]
    [SerializeField] private Player player;

    private Quaternion restRotation;

    public bool IsGuarding { get; private set; }

    private void Awake()
    {
        if (guardPivot != null)
            restRotation = guardPivot.rotation;

        if (guardCollider != null)
            guardCollider.isTrigger = true;

        SetGuard(false);
    }

    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null || guardPivot == null ||
            guardVisual == null || guardCollider == null)
            yield break;

        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (player == null || !player.isActiveAndEnabled)
            yield break;

        // จำตำแหน่งผู้เล่นครั้งเดียว ไม่หมุนตามหลังจากนี้
        Vector2 toPlayer = player.transform.position - guardPivot.position;
        float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        float currentAngle = guardPivot.eulerAngles.z;

        guardVisual.enabled = true;
        guardCollider.enabled = false;

        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f)
        {
            currentAngle = Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                turnSpeed * Time.deltaTime
            );

            guardPivot.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            yield return null;
        }

        guardPivot.rotation = Quaternion.Euler(0f, 0f, targetAngle);

        // เริ่มกันเมื่อยกแขนหันเข้าที่แล้ว
        SetGuard(true);
        yield return new WaitForSeconds(guardDuration);

        SetGuard(false);
        guardPivot.rotation = restRotation;
    }

    private void SetGuard(bool guarding)
    {
        IsGuarding = guarding;

        if (guardCollider != null)
            guardCollider.enabled = guarding;

        if (guardVisual != null)
            guardVisual.enabled = guarding;
    }

    private void OnDisable()
    {
        SetGuard(false);

        if (guardPivot != null)
            guardPivot.rotation = restRotation;
    }
}