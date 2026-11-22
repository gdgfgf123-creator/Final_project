using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : BasicAttack
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.up;
    private bool hasHit;

    protected override void Awake()
    {
        base.Awake();

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

    public void SetDamage(int amount)
    {
        damage = Mathf.Max(0, amount);
    }

    private void FixedUpdate()
    {
        if (hasHit)
            return;

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        SalamanderGuardAttack salamanderGuard =
            other.GetComponentInParent<SalamanderGuardAttack>();

        if (salamanderGuard != null)
        {
            if (salamanderGuard.IsGuarding)
                RemoveBullet();

            return;
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy == null)
            return;

        RemoveBullet();
        enemy.TakeDamage(damage);
    }

    private void RemoveBullet()
    {
        hasHit = true;
        Destroy(gameObject);
    }
}