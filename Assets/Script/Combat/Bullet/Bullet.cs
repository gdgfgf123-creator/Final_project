using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float lifeTime = 3f;
    [SerializeField] protected int damage = 10;

    protected Rigidbody2D rb;
    protected Vector2 direction = Vector2.up;
    protected bool hasHit;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        GetComponent<Collider2D>().isTrigger = true;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public virtual void Initialize(
        Vector2 shootDirection,
        int damageAmount)
    {
        SetDirection(shootDirection);
        SetDamage(damageAmount);
    }

    public virtual void SetDirection(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
    }

    public void SetDamage(int amount)
    {
        damage = Mathf.Max(0, amount);
    }

    protected virtual void FixedUpdate()
    {
        if (hasHit)
            return;

        rb.MovePosition(
            rb.position +
            direction * speed * Time.fixedDeltaTime
        );
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        SalamanderGuardAttack guard =
            other.GetComponentInParent<SalamanderGuardAttack>();

        if (guard != null && guard.IsGuarding)
        {
            RemoveBullet();
            return;
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy == null)
            return;

        RemoveBullet();
        enemy.TakeDamage(damage);
    }

    protected void RemoveBullet()
    {
        hasHit = true;
        Destroy(gameObject);
    }

    protected void RotateToDirection(float angleOffset)
    {
        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            angle + angleOffset
        );
    }
}