using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected int currentHP = 30;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 3f;

    [Header("Attack")]
    [SerializeField] protected EnemyBullet bulletPrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] private float fireInterval = 1f;

    protected Rigidbody2D rb;
    protected bool isDead;

    private float startX;
    private int moveDirection = 1;
    private float nextFireTime;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        startX = rb.position.x;
        nextFireTime = Time.time + fireInterval;
    }

    protected virtual void Update()
    {
        if (isDead || Time.timeScale == 0f) return;

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireInterval;
            Shoot();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isDead) return;

        float leftX = startX - moveDistance;
        float rightX = startX + moveDistance;

        float nextX = rb.position.x +
            moveDirection * moveSpeed * Time.fixedDeltaTime;

        if (nextX >= rightX)
        {
            nextX = rightX;
            moveDirection = -1;
        }
        else if (nextX <= leftX)
        {
            nextX = leftX;
            moveDirection = 1;
        }

        rb.MovePosition(new Vector2(nextX, rb.position.y));
    }

    protected virtual void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        EnemyBullet bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        bullet.SetDirection(Vector2.down);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}