using UnityEngine;

public class VictoriaBoss : Boss
{
    [Header("Phase 2 Rotation")]
    [SerializeField] private float firePointRotateSpeed = 45f;

    protected override void Update()
    {
        if (!isDead && IsPhaseTwo && firePoint != null)
        {
            firePoint.Rotate(
                0f,
                0f,
                firePointRotateSpeed * Time.deltaTime
            );
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        // วิกตอเลียยืนประจำจุด
    }

    protected override void Die()
    {
        Debug.Log("Victoria defeated!");
        base.Die();
    }
}