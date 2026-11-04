using UnityEngine;

public class WaveBullet : EnemyBullet
{
    [Header("Wave")]
    [SerializeField] private float waveWidth = 0.6f;
    [SerializeField] private float waveFrequency = 1f;

    private Vector2 startPosition;
    private float elapsedTime;

    protected override void Start()
    {
        base.Start();
        startPosition = rb.position;
    }

    protected override void FixedUpdate()
    {
        if (hasHit) return;

        elapsedTime += Time.fixedDeltaTime;

        Vector2 sideDirection =
            new Vector2(-direction.y, direction.x);

        float waveOffset = Mathf.Sin(
            elapsedTime * waveFrequency * Mathf.PI * 2f
        ) * waveWidth;

        Vector2 forwardPosition =
            startPosition + direction * speed * elapsedTime;

        rb.MovePosition(
            forwardPosition + sideDirection * waveOffset
        );
    }
}