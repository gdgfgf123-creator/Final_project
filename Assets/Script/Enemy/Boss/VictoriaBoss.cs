using UnityEngine;

public class VictoriaBoss : Boss
{
    [Header("Fire Point Rotation")]
    [SerializeField] private float phaseOneRotateSpeed = 20f;
    [SerializeField] private float firePointRotateSpeed = 45f;

    protected override void Update()
    {
        if (!isDead && firePoint != null)
        {
            float rotationSpeed = IsPhaseTwo
                ? firePointRotateSpeed
                : phaseOneRotateSpeed;

            firePoint.Rotate(
                0f,
                0f,
                rotationSpeed * Time.deltaTime
            );
        }

        base.Update();
    }

    

    protected override void Die()
    {
        Debug.Log("Victoria defeated!");
        base.Die();
    }
}