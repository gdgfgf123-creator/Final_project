using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SalamanderGuardAttack : BossAttack
{
    [Header("Guard")]
    [SerializeField, Min(0f)] private float guardDuration = 0.6f;
    [SerializeField] private SpriteRenderer guardVisual;

    [Header("Guard Movement")]
    [SerializeField] private Player player;
    [SerializeField, Min(0.1f)] private float guardDistance = 1.4f;
    [SerializeField, Range(0f, 75f)] private float guardAngle = 35f;
    [SerializeField, Min(1f)] private float guardTurnSpeed = 360f;

    [Header("Counterattack")]
    [SerializeField] private EnemyBullet fireBulletPrefab;
    [SerializeField] private Transform salamanderFirePoint;
    [SerializeField, Min(1)] private int fireCount = 12;
    [SerializeField, Range(0f, 180f)] private float fireSpread = 100f;

    [SerializeField, Min(0f)] private float guardRaiseDuration = 0.2f;

    private BoxCollider2D guardCollider;
    private float currentGuardAngle;

    public bool IsGuarding { get; private set; }

    private void Awake()
    {
        guardCollider = GetComponent<BoxCollider2D>();
        guardCollider.isTrigger = true;
        SetGuard(false);
    }

    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null || !isActiveAndEnabled)
            yield break;

        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (guardDuration > 0f)
        {
            float startAngle = currentGuardAngle;
            float targetAngle = GetTargetAngle(boss);
            float elapsed = 0f;

            // โชว์แขนระหว่างยก แล้วเปิด Collider เมื่อเข้าที่
            if (guardVisual != null)
                guardVisual.enabled = true;

            while (elapsed < guardRaiseDuration &&
                   boss != null && isActiveAndEnabled)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / guardRaiseDuration);

                currentGuardAngle = Mathf.SmoothStep(
                    startAngle, targetAngle, progress
                );

                ApplyGuardPose();
                yield return null;
            }

            if (boss == null || !isActiveAndEnabled)
            {
                SetGuard(false);
                yield break;
            }

            currentGuardAngle = targetAngle;
            ApplyGuardPose();
            SetGuard(true);

            yield return new WaitForSeconds(guardDuration);

            SetGuard(false);
        }

        if (boss == null || !isActiveAndEnabled)
            yield break;

        ShootFan(fireBulletPrefab, salamanderFirePoint, fireCount, fireSpread);
    }

    private float GetTargetAngle(Boss boss)
    {
        if (player == null || !player.isActiveAndEnabled)
            return 0f;

        Vector3 playerPosition =
            boss.transform.InverseTransformPoint(player.transform.position);

        if (playerPosition.x < -0.1f)
            return -guardAngle;

        if (playerPosition.x > 0.1f)
            return guardAngle;

        return 0f;
    }

    private void ApplyGuardPose()
    {
        Quaternion direction =
            Quaternion.Euler(0f, 0f, currentGuardAngle);

        // ตำแหน่งยังอยู่เฉียงซ้ายหรือขวาตามมุมที่เลือก
        Vector3 position = direction * Vector3.down * guardDistance;
        position.z = transform.localPosition.z;
        transform.localPosition = position;

        // หมุนด้านยาวของกล่องให้ขวางทางกระสุน
        transform.localRotation =
            Quaternion.Euler(0f, 0f, currentGuardAngle + 90f);
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