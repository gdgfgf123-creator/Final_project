using UnityEngine;

public class Belle : Boss
{
    private Player player;

    [Header("Belle Movement")]
    [SerializeField] private float floatAmplitude = 0.3f;
    [SerializeField] private float floatFrequency = 2f;

    [SerializeField] private Transform visual;

    private Vector3 visualStartPosition;

    protected override void Awake()
    {
        base.Awake();

        player = FindFirstObjectByType<Player>();

        if (visual != null)
            visualStartPosition = visual.localPosition;
    }

    private void LateUpdate()
    {
        if (isDead || visual == null)
            return;

        float offset =
            Mathf.Sin(Time.time * floatFrequency) *
            floatAmplitude;

        visual.localPosition =
            visualStartPosition +
            Vector3.up * offset;
    }

    protected override Vector2 GetShootDirection()
    {
        if (player == null)
            return Vector2.down;

        return (
            player.transform.position -
            transform.position
        ).normalized;
    }
}