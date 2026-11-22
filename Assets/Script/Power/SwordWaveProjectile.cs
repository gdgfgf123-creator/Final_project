using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SwordWaveProjectile : MonoBehaviour
{
    [SerializeField, Min(0f)] private float speed = 12f;
    [SerializeField, Min(0.1f)] private float lifeTime = 3f;

    private Rigidbody2D rb;
    private Vector2 direction;
    private int damage;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector2 shootDirection, int damageAmount)
    {
        direction = shootDirection.normalized;
        damage = Mathf.Max(0, damageAmount);

        ProjectileFaceDirection faceDirection =
            GetComponent<ProjectileFaceDirection>();

        if (faceDirection != null)
            faceDirection.FaceDirection(direction);
    }

    private void FixedUpdate()
    {
        if (hasHit)
            return;

        rb.MovePosition(
            rb.position + direction * speed * Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy == null)
            return;

        hasHit = true;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}