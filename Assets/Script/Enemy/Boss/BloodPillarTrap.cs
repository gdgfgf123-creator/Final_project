using UnityEngine;

public class BloodPillarTrap : MonoBehaviour
{
    [SerializeField, Min(0)] private int damage = 1;
    [SerializeField, Min(0f)] private float lockDuration = 1f;
    [SerializeField, Min(0f)] private float trapDuration = 2f;
    [SerializeField, Min(0f)] private float riseDelay = 0.15f;
    [SerializeField] private Collider2D hitbox;

    private bool hasHit;

    private void Awake()
    {
        if (hitbox == null)
            hitbox = GetComponent<Collider2D>();

        if (hitbox != null)
            hitbox.enabled = false;
    }

    private void Start()
    {
        Destroy(gameObject, trapDuration);
        Invoke(nameof(EnableHitbox), riseDelay);
    }

    private void EnableHitbox()
    {
        if (hitbox != null)
            hitbox.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        Player target = other.GetComponentInParent<Player>();

        if (target == null)
            return;

        hasHit = true;
        target.TakeDamage(damage);

        PlayerRootedEffect rootedEffect =
            target.GetComponent<PlayerRootedEffect>();

        if (rootedEffect == null)
            rootedEffect = target.gameObject.AddComponent<PlayerRootedEffect>();

        rootedEffect.LockFor(lockDuration);
    }
}