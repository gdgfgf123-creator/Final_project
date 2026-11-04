using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.down;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
    }

    private void FixedUpdate()
    {
        if (hasHit) return;

        rb.MovePosition(
            rb.position +
            direction * speed * Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        Player player = other.GetComponentInParent<Player>();

        if (player == null) return;

        hasHit = true;
        player.TakeDamage(damage);
        Destroy(gameObject);
    }
}