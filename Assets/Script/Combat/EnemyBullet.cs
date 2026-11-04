using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected int damage = 1;

    protected Rigidbody2D rb;
    protected Vector2 direction = Vector2.down;
    protected bool hasHit;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 shootDirection)
    {
        if (shootDirection.sqrMagnitude > 0.001f)
            direction = shootDirection.normalized;
    }

    protected virtual void FixedUpdate()
    {
        if (hasHit) return;

        rb.MovePosition(
            rb.position +
            direction * speed * Time.fixedDeltaTime
        );
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        Player player = other.GetComponentInParent<Player>();

        if (player == null) return;

        hasHit = true;
        player.TakeDamage(damage);

        Destroy(gameObject);
    }
}